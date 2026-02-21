using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components;

public class Animation(List<Texture2D> frames, float frameDuration) : Component
{
    public Texture2D CurrentFrame { get; set; } = frames.Count > 0 ? frames[0] : null;
    public float FrameDuration { get; set; } = frameDuration;

    private readonly List<Texture2D> _frames = frames;
    private float _timer = 0f;
    private int _currentFrameIndex = 0;

    public override void Update(GameTime gameTime)
    {
        if (_frames.Count == 0)
            return;

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= FrameDuration)
        {
            _timer -= FrameDuration;
            _currentFrameIndex = (_currentFrameIndex + 1) % _frames.Count;
            CurrentFrame = _frames[_currentFrameIndex];
        }
    }
}