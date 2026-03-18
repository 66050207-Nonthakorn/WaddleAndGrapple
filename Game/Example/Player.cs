using ComputerGameFinal.Engine;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Components.Physics;
using ComputerGameFinal.Engine.Managers;
using ComputerGameFinal.Engine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ComputerGameFinal.Game.Example;

public class Player : GameObject
{
    private const float MoveSpeed = 300f;

    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    public override void Initialize()
    {
        _spriteRenderer = AddComponent<SpriteRenderer>();
        _spriteRenderer.LayerDepth = 0.5f;
        // _spriteRenderer.Texture = ResourceManager.Instance.GetTexture("bird");

        Animation walk = new Animation(ResourceManager.Instance.GetTexture("mario_walk"), 3, 1, 0.2f);
        Animation idle = new Animation(ResourceManager.Instance.GetTexture("mario_walk"), 1, 1, 0.2f);
        
        _animator = AddComponent<Animator>();
        _animator.AddAnimation("mario_walk", walk);
        _animator.AddAnimation("mario_idle", idle);
        _animator.Play("mario_idle");
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 speed = Vector2.Zero;

        if (InputManager.Instance.IsKeyDown(Keys.A))
        {
            speed.X = -MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.D))
        {
            speed.X = MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.W))
        {
            speed.Y = -MoveSpeed;
        }
        if (InputManager.Instance.IsKeyDown(Keys.S))
        {
            speed.Y = MoveSpeed;
        }

        if (speed != Vector2.Zero)
        {
            _animator.Play("mario_walk");

            if (speed.X < 0)
            {;
                base.Rotation = QuaternionUtils.Euler(0, 180, 0); 
            }
            else
            {
                base.Rotation = QuaternionUtils.Euler(0, 0, 0);
            }
        }
        else
        {
            _animator.Play("mario_idle");
        }

        Position += speed * dt;
    }
}