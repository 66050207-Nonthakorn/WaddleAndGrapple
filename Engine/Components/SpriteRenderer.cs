using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components;

public class SpriteRenderer : Component
{
    public Texture2D Texture
    {
        get { return _texture; }
        set
        {
            _texture = value;
            Origin = new Vector2(
                _texture.Bounds.Center.X,
                _texture.Bounds.Center.Y
            );
        }
    }
    private Texture2D _texture;

    public Color Tint { get; set; } = Color.White;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public float LayerDepth { get; set; } = 0;

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            spriteBatch.Draw(Texture, base.GameObject.Position, null, Tint, 
                base.GameObject.Rotation, Origin, base.GameObject.Scale, SpriteEffects.None, LayerDepth);
        }
    }
}