using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ComputerGameFinal.Engine.Components;
using System.Linq;

namespace ComputerGameFinal.Engine;

public abstract class GameObject
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Rotation { get; set; } = 0f;
    public Vector2 Scale { get; set; } = Vector2.One;

    public bool Active { get; set; } = true;
    public string Tag { get; set; }

    private readonly List<Component> _components = [];

    public abstract void Initialize();
    public abstract void Update(GameTime gameTime);

    public T AddComponent<T>() where T : Component, new()
    {
        T component = new T();
        _components.Add(component);

        component.GameObject = this;
        component.Initialize();
        
        return component;
    }

    public T GetComponent<T>() where T : Component
    {
        return _components.OfType<T>().FirstOrDefault();
    }

    public void RemoveComponent<T>() where T : Component
    {
        T component = GetComponent<T>();
        if (component != null)
        {
            _components.Remove(component);
        }
    }

    public void InitializeComponents()
    {
        foreach (var component in _components)
        {
            component.Initialize();
        }
    }

    public void UpdateComponents(GameTime gameTime)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Update(gameTime);
            }   
        }
    }

    public void DrawComponents(SpriteBatch spriteBatch)
    {
        if (!Active) return;
        foreach (var component in _components)
        {
            if (component.Enabled)
            {
                component.Draw(spriteBatch);
            }
        }
    }
}