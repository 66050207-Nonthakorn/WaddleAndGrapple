using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ComputerGameFinal.Engine.Components;
using ComputerGameFinal.Engine.Managers;

namespace ComputerGameFinal.Engine.UI;

public class Button : Component
{
    public Vector2 Size { get; set; } = new Vector2(200, 50);
    public Action OnClick { get; set; }

    public Color OutlineColor { get; set; } = Color.Red;
    public int OutlineThickness { get; set; } = 1;
    public bool IsShowOutline { get; set; } = false;

    public override void Update(GameTime gameTime)
    {
        var mousePosition = InputManager.Instance.GetMousePosition();
        var buttonRectangle = new Rectangle((int)base.GameObject.Position.X, (int)base.GameObject.Position.Y, (int)Size.X, (int)Size.Y);

        if (buttonRectangle.Contains(mousePosition))
        {
            if (InputManager.Instance.IsMouseButtonDown(0))
            {            
                OnClick?.Invoke();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsShowOutline) return;

        Texture2D dummyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        dummyTexture.SetData([OutlineColor]);
        
        // Top outline
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)GameObject.Position.X - OutlineThickness,
                (int)GameObject.Position.Y - OutlineThickness,
                (int)Size.X + OutlineThickness * 2,
                OutlineThickness
            ),
            OutlineColor
        );

        // Bottom outline
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)GameObject.Position.X - OutlineThickness,
                (int)(GameObject.Position.Y + Size.Y),
                (int)Size.X + OutlineThickness * 2,
                OutlineThickness
            ),
            OutlineColor
        );

        // Left outline
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)GameObject.Position.X - OutlineThickness,
                (int)GameObject.Position.Y - OutlineThickness,
                OutlineThickness,
                (int)Size.Y + OutlineThickness * 2
            ),
            OutlineColor
        );

        // Right outline
        spriteBatch.Draw(
            dummyTexture,
            new Rectangle(
                (int)(GameObject.Position.X + Size.X),
                (int)GameObject.Position.Y - OutlineThickness,
                OutlineThickness,
                (int)Size.Y + OutlineThickness * 2
            ),
            OutlineColor
        );
    }
}