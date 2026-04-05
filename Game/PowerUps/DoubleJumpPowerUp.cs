using Microsoft.Xna.Framework;
using WaddleAndGrapple.Game.Example;

namespace WaddleAndGrapple.Game;

/// <summary>
/// เก็บ item → เกจเต็ม (GaugeRatio = 1)
/// กด double jump → เกจ drain รวดเร็วใน DrainDuration วินาที → ability หาย
/// </summary>
public class DoubleJumpPowerUp : PowerUp
{
    public override Color ItemColor => new Color(0, 220, 255);
    protected override string SpriteName => "Collectibles/DoubleJump";

    private const float DrainDuration = 0.4f;  // วิที่เกจ drain หลังกด jump

    // GaugeRatio: 1=เต็ม, 0=หมด — ใช้โดย PowerUpBarRenderer
    private float _gaugeRatio = 1f;
    public override float GaugeRatio => _gaugeRatio;

    private bool _draining = false;
    private float _drainTimer = 0f;

    public DoubleJumpPowerUp()
    {
        Duration = 0f; // ไม่นับเวลา — จัดการเองใน UpdateEffect
    }

    protected override void OnActivate(Player player)
    {
        player.HasDoubleJump     = true;
        player.HasUsedDoubleJump = false;
        _gaugeRatio = 1f;
        _draining   = false;
        _drainTimer = 0f;
    }

    protected override void OnDeactivate(Player player)
    {
        player.HasDoubleJump     = false;
        player.HasUsedDoubleJump = false;
        _gaugeRatio = 0f;
    }

    public new void UpdateEffect(Player player, float dt)
    {
        if (!IsActive) return;

        // ตรวจว่า player เพิ่งกด double jump → เริ่ม drain
        if (!_draining && player.HasUsedDoubleJump)
        {
            _draining   = true;
            _drainTimer = 0f;
        }

        if (_draining)
        {
            _drainTimer += dt;
            _gaugeRatio  = System.Math.Max(0f, 1f - _drainTimer / DrainDuration);
            if (_drainTimer >= DrainDuration)
                Deactivate(player);
        }
    }
}
