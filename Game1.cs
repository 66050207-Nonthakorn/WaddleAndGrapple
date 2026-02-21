using System;
using System.Diagnostics;
using ComputerGameFinal.Engine.Managers;
using ComputerGameFinal.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ResourceManager = ComputerGameFinal.Engine.Managers.ResourceManager;

namespace ComputerGameFinal;

public class Game1 : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        SceneManager.Instance.AddScene<MainScene>("main");
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ResourceManager.Instance.LoadAll(Content);

        SceneManager.Instance.LoadScene("main");
    }

    protected override void Update(GameTime gameTime)
    {
        InputManager.Instance.Update();
        SceneManager.Instance.CurrentScene.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        
        _spriteBatch.Begin(SpriteSortMode.FrontToBack);
        SceneManager.Instance.CurrentScene.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
