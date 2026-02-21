using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Managers;

public class SceneManager
{
    public static SceneManager Instance { get; private set; } = new SceneManager();

    public Scene CurrentScene { get; private set; }
    private readonly Dictionary<string, Scene> _scenes = [];

    private SceneManager() { }

    public void LoadScene(string sceneName)
    {
        if (_scenes.TryGetValue(sceneName, out var scene))
        {
            CurrentScene?.Unload();
            CurrentScene = scene;
            scene.Load();
        }
    }

    public void AddScene<T>(string sceneName) where T : Scene, new()
    {
        _scenes[sceneName] = new T();
    }

    public void Update(GameTime gameTime)
    {
        CurrentScene?.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        CurrentScene?.Draw(spriteBatch);
    }
}