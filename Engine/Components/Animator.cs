using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ComputerGameFinal.Engine.Components;

class Animator : Component
{
    private readonly Dictionary<string, Animation> _animations = [];
    private Animation _currentAnimation;

    public void AddAnimation(string name, Animation animation)
    {
        _animations[name] = animation;
    }

    public void Play(string name)
    {
        if (_animations.TryGetValue(name, out var animation))
        {
            _currentAnimation = animation;
        }
    }

    public override void Update(GameTime gameTime)
    {
        _currentAnimation?.Update(gameTime);
    }
}