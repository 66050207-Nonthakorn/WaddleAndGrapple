using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ComputerGameFinal.Game;

public class Player : GameObject
{
    // ── Flappy bird physics constants ──────────────────────────────────────
    private const float Gravity    = 1200f;   // pixels/s² downward
    private const float FlapForce  = -450f;   // pixels/s  upward impulse
    private const float MaxFallSpeed = 600f;  // terminal velocity

    private float _velocityY = 0f;

    private SpriteRenderer _spriteRenderer;

    public override void Initialize()
    {
        _spriteRenderer = AddComponent<SpriteRenderer>();
        _spriteRenderer.Texture = ResourceManager.Instance.GetTexture("bird");
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Flap on Space or left mouse click
        if (InputManager.Instance.IsKeyPressed(Keys.Space) ||
            InputManager.Instance.IsMouseButtonPressed(0))
        {
            _velocityY = FlapForce;
        }

        // Apply gravity
        _velocityY = MathHelper.Clamp(_velocityY + Gravity * dt, -9999f, MaxFallSpeed);

        // Move
        Position += new Vector2(0, _velocityY * dt);

        // Tilt sprite to match velocity (-30° flapping, up to +90° nose-diving)
        Rotation = MathHelper.Clamp(_velocityY / MaxFallSpeed * MathHelper.PiOver2,
                                    -MathHelper.Pi / 6f,
                                     MathHelper.PiOver2);
    }
}