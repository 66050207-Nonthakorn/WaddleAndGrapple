using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WaddleAndGrapple.Engine.Components;

public abstract class Component
{
    public GameObject GameObject { get; internal set; }
    public bool Enabled { get; set; } = true;

    public virtual void Initialize() { }
    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(SpriteBatch spriteBatch) { }
}
