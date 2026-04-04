using System.Collections.Generic;
using WaddleAndGrapple.Engine;
using WaddleAndGrapple.Engine.Components;
using WaddleAndGrapple.Engine.Components.Physics;
using WaddleAndGrapple.Engine.Managers;
using WaddleAndGrapple.Engine.Utils;
using Microsoft.Xna.Framework;

namespace WaddleAndGrapple.Game;

// ── State Machine ─────────────────────────────────────────────────────────────
public enum EnemyState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    ReturningToSpawn,
    Dead,
}

// ─────────────────────────────────────────────────────────────────────────────

public class Enemy : GameObject
{
    // ── Physics Constants ─────────────────────────────────────────────────────
    private const float Gravity      = 1200f;  // px/s²
    private const float MaxFallSpeed = 700f;   // px/s

    // ── Collider Size ─────────────────────────────────────────────────────────
    private const int EnemyWidth  = 80;
    private const int EnemyHeight = 120;

    // ── Temporary Ground (ลบเมื่อ tiles พร้อม) ───────────────────────────────
    private const float TempGroundY = 400f;

    // ── Sprite Scale ──────────────────────────────────────────────────────────
    public const float DisplayScale = 2f;

    // ── Movement Speeds ───────────────────────────────────────────────────────
    public float PatrolSpeed { get; set; } = 100f;
    public float ChaseSpeed  { get; set; } = 200f;
    public float ReturnSpeed { get; set; } = 220f;

    // ── AI Ranges ─────────────────────────────────────────────────────────────
    public float PatrolRadius   { get; set; } = 150f; // ระยะ patrol ซ้าย/ขวาจาก spawn
    public float DetectionRange { get; set; } = 250f; // ระยะมองเห็น player
    public float AttackRange    { get; set; } = 60f;  // ระยะ melee
    public float LeashRange     { get; set; } = 400f; // ระยะสูงสุดก่อน return to spawn

    // ── Combat ────────────────────────────────────────────────────────────────
    public float AttackCooldown { get; set; } = 1.5f;
    private float _attackTimer;

    // ── Attack Animation Duration ─────────────────────────────────────────────
    // 6 frames × 0.083 s — ปรับตามจำนวน frame จริงใน spritesheet
    private const float AttackAnimDuration = 7 * 0.083f;
    private float _attackAnimTimer;

    // ── Velocity ──────────────────────────────────────────────────────────────
    public float VelocityX;
    public float VelocityY;

    // ── Ground Status ─────────────────────────────────────────────────────────
    public bool IsGrounded      { get; set; }
    public int  FacingDirection { get; set; } = 1; // +1 = ขวา, -1 = ซ้าย

    // ── State Machine ─────────────────────────────────────────────────────────
    public EnemyState State { get; private set; } = EnemyState.Idle;

    // ── Spawn / Patrol ────────────────────────────────────────────────────────
    private Vector2 _spawnPosition;
    private int     _patrolDirection = 1;

    // ── Player Reference ──────────────────────────────────────────────────────
    private Player _player;

    // ── Components ────────────────────────────────────────────────────────────
    private SpriteRenderer   _spriteRenderer;
    private Animator         _animator;
    private EnemyBoxCollider _collider;
    private List<Rectangle>  _solidRects = [];

    // ═════════════════════════════════════════════════════════════════════════

    public override void Initialize()
    {
        _spawnPosition   = Position;
        _patrolDirection = 1;

        Scale       = new Vector2(DisplayScale, DisplayScale);
        _animator   = AddComponent<Animator>();
        _spriteRenderer            = GetComponent<SpriteRenderer>();
        _spriteRenderer.LayerDepth = 0.5f;

        // TODO: แทนที่ "Enemy/Enemy-SpriteSheet" ด้วย path spritesheet จริง
        //       และปรับ rows/columns/totalFrames ให้ตรงกับไฟล์
        var f = new AnimationFactory(
            ResourceManager.Instance.GetTexture("elephant-animation"),
            rows: 8, columns: 8
        );

        _animator.AddAnimation("standing",   f.CreateFromRow(row: 0, totalFrames: 1, frameDuration: 0.083f));
        _animator.AddAnimation("idle",   f.CreateFromRow(row: 1, totalFrames: 7, frameDuration: 0.10f, isLooping: false));
        _animator.AddAnimation("walk",   f.CreateFromRow(row: 2, totalFrames: 8, frameDuration: 0.083f));
        _animator.AddAnimation("run",   f.CreateFromRow(row: 2, totalFrames: 8, frameDuration: 0.065f));
        _animator.AddAnimation("attack", f.CreateFromRow(row: 3, totalFrames: 7, frameDuration: 0.083f, isLooping: false));
        _animator.AddAnimation("dead",   f.CreateFromRow(row: 4, totalFrames: 7, frameDuration: 0.10f, isLooping: false));
        _animator.AddAnimation("fallingdown", f.CreateFromRow(row: 5, totalFrames: 4, frameDuration: 0.083f));
        _animator.AddAnimation("stunned", f.CreateFromRow(row: 6, totalFrames: 5, frameDuration: 0.10f));
        _animator.AddAnimation("gettingup", f.CreateFromRow(row: 7, totalFrames: 4, frameDuration: 0.10f));

        _animator.Play("standing");

        _collider = AddComponent<EnemyBoxCollider>();
        UpdateColliderBounds();
    }

    // ── Update Loop ───────────────────────────────────────────────────────────
    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (WorldTime.IsFrozen) return;
        if (State == EnemyState.Dead) return;

        // Attack cooldown
        if (_attackTimer     > 0f) _attackTimer     -= dt;
        if (_attackAnimTimer > 0f) _attackAnimTimer -= dt;

        // AI decision → ตั้ง VelocityX
        UpdateAI();

        // Physics
        ApplyGravity(dt);
        MoveAndCollide(dt);

        // Animation + sprite flip
        SyncAnimation();
        Rotation = FacingDirection == -1
            ? QuaternionUtils.Euler(0, 180, 0)
            : Vector3.Zero;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // AI
    // ══════════════════════════════════════════════════════════════════════════

    private void UpdateAI()
    {
        if (_player == null) return;

        float distToSpawn  = System.MathF.Abs(Position.X - _spawnPosition.X);
        float distToPlayer = Vector2.Distance(Position, _player.Position);
        bool  playerInSight = distToPlayer <= DetectionRange;

        // Leash: ออกไกลเกิน LeashRange → กลับ spawn ก่อนทำอะไรทั้งนั้น
        if (distToSpawn > LeashRange && State != EnemyState.ReturningToSpawn)
        {
            ChangeState(EnemyState.ReturningToSpawn);
        }

        switch (State)
        {
            // ── Idle: เริ่ม patrol ทันที ──────────────────────────────────────
            case EnemyState.Idle:
                ChangeState(EnemyState.Patrolling);
                break;

            // ── Patrol: เดินไปมาในรัศมี PatrolRadius ─────────────────────────
            case EnemyState.Patrolling:
                if (playerInSight)
                {
                    ChangeState(EnemyState.Chasing);
                    break;
                }
                HandlePatrol();
                break;

            // ── Chase: วิ่งตาม player ─────────────────────────────────────────
            case EnemyState.Chasing:
                if (!playerInSight)
                {
                    // ยังอยู่ในเขต patrol → กลับ patrol; ออกนอกเขต → คืน spawn
                    ChangeState(distToSpawn <= PatrolRadius
                        ? EnemyState.Patrolling
                        : EnemyState.ReturningToSpawn);
                    break;
                }
                if (distToPlayer <= AttackRange)
                {
                    TryMeleeAttack();
                    break;
                }
                ChasePlayer();
                break;

            // ── Attacking: หยุดนิ่ง รอ animation จบ ─────────────────────────
            case EnemyState.Attacking:
                VelocityX = 0f;
                if (_attackAnimTimer <= 0f)
                    ChangeState(playerInSight ? EnemyState.Chasing : EnemyState.Patrolling);
                break;

            // ── Return to Spawn ───────────────────────────────────────────────
            case EnemyState.ReturningToSpawn:
                HandleReturnToSpawn();
                if (distToSpawn < 10f)
                {
                    VelocityX = 0f;
                    ChangeState(EnemyState.Patrolling);
                }
                break;
        }
    }

    private void HandlePatrol()
    {
        float patrolLeft  = _spawnPosition.X - PatrolRadius;
        float patrolRight = _spawnPosition.X + PatrolRadius;

        if (Position.X <= patrolLeft)  _patrolDirection =  1;
        if (Position.X >= patrolRight) _patrolDirection = -1;

        FacingDirection = _patrolDirection;
        VelocityX       = _patrolDirection * PatrolSpeed;
    }

    private void ChasePlayer()
    {
        float dir = _player.Position.X > Position.X ? 1f : -1f;
        FacingDirection = (int)dir;
        VelocityX       = dir * ChaseSpeed;
    }

    private void TryMeleeAttack()
    {
        if (_attackTimer > 0f) return; // ยังอยู่ใน cooldown

        _attackTimer     = AttackCooldown;
        _attackAnimTimer = AttackAnimDuration;
        ChangeState(EnemyState.Attacking);

        // เผชิญหน้ากับ player ก่อน attack
        FacingDirection = _player.Position.X > Position.X ? 1 : -1;

        // Instant kill
        _player.Die();
    }

    private void HandleReturnToSpawn()
    {
        float dir = _spawnPosition.X > Position.X ? 1f : -1f;
        FacingDirection = (int)dir;
        VelocityX       = dir * ReturnSpeed;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Animation Sync
    // ══════════════════════════════════════════════════════════════════════════

    private void SyncAnimation()
    {
        switch (State)
        {
            case EnemyState.Idle:
                _animator.Play("idle");
                break;
            case EnemyState.Patrolling:
                _animator.Play("walk");
                break;
            case EnemyState.ReturningToSpawn:
            case EnemyState.Chasing:
                _animator.Play("run");
                break;
            case EnemyState.Attacking:
                _animator.Play("attack");
                break;
            case EnemyState.Dead:
                _animator.Play("dead");
                break;
            default:
                _animator.Play("idle");
                break;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Physics (เหมือน Player)
    // ══════════════════════════════════════════════════════════════════════════

    private void ApplyGravity(float dt)
    {
        VelocityY += Gravity * dt;
        if (VelocityY > MaxFallSpeed) VelocityY = MaxFallSpeed;
    }

    private void MoveAndCollide(float dt)
    {
        IsGrounded = false;

        // ── Horizontal ────────────────────────────────────────────────────────
        Position = new Vector2(Position.X + VelocityX * dt, Position.Y);
        UpdateColliderBounds();

        foreach (var solid in _solidRects)
        {
            if (!_collider.Bounds.Intersects(solid)) continue;

            if (VelocityX > 0f)
            {
                Position = new Vector2(solid.Left - EnemyWidth / 2f, Position.Y);
                // ชนผนังขณะ patrol → สลับทิศ
                if (State == EnemyState.Patrolling) _patrolDirection = -1;
            }
            else if (VelocityX < 0f)
            {
                Position = new Vector2(solid.Right + EnemyWidth / 2f, Position.Y);
                if (State == EnemyState.Patrolling) _patrolDirection = 1;
            }
            VelocityX = 0f;
            UpdateColliderBounds();
        }

        // ── Vertical ──────────────────────────────────────────────────────────
        Position = new Vector2(Position.X, Position.Y + VelocityY * dt);
        UpdateColliderBounds();

        foreach (var solid in _solidRects)
        {
            bool hit = _collider.Bounds.Left   < solid.Right
                    && _collider.Bounds.Right  > solid.Left
                    && _collider.Bounds.Top    < solid.Bottom
                    && _collider.Bounds.Bottom >= solid.Top;
            if (!hit) continue;

            if (VelocityY > 0f)
            {
                Position   = new Vector2(Position.X, solid.Top - EnemyHeight / 2f);
                IsGrounded = true;
            }
            else if (VelocityY < 0f)
            {
                Position = new Vector2(Position.X, solid.Bottom + EnemyHeight / 2f);
            }
            VelocityY = 0f;
            UpdateColliderBounds();
        }

        // ── Temp Ground ───────────────────────────────────────────────────────
        if (_solidRects.Count == 0)
        {
            float groundTopY = TempGroundY - EnemyHeight / 2f;
            if (Position.Y >= groundTopY)
            {
                Position   = new Vector2(Position.X, groundTopY);
                VelocityY  = 0f;
                IsGrounded = true;
            }
        }
    }

    private void UpdateColliderBounds()
    {
        if (_collider == null) return;
        _collider.Bounds = new Rectangle(
            (int)(Position.X - EnemyWidth  / 2f),
            (int)(Position.Y - EnemyHeight / 2f),
            EnemyWidth,
            EnemyHeight
        );
    }

    // ══════════════════════════════════════════════════════════════════════════
    // State Machine
    // ══════════════════════════════════════════════════════════════════════════

    private void ChangeState(EnemyState newState)
    {
        if (State == newState) return;
        State = newState;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // Public API
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>ส่ง Player reference จาก Level เพื่อให้ Enemy ติดตาม</summary>
    public void SetPlayer(Player player) => _player = player;

    /// <summary>ส่ง solid rectangles จาก Level (เหมือน Player.SetSolids)</summary>
    public void SetSolids(List<Rectangle> solids) => _solidRects = solids;

    public Rectangle ColliderBounds => _collider?.Bounds ?? Rectangle.Empty;

    /// <summary>เรียกจาก hazard/trap หรือ Player เมื่อต้องการกำจัด enemy</summary>
    public void Die()
    {
        if (State == EnemyState.Dead) return;
        VelocityX = 0f;
        VelocityY = 0f;
        ChangeState(EnemyState.Dead);
    }
}

// ── Concrete BoxCollider สำหรับ Enemy ────────────────────────────────────────
internal sealed class EnemyBoxCollider : BoxCollider { }
