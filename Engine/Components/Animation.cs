using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ComputerGameFinal.Engine.Components;

public class Animation : Component
{
    /// <summary>The sprite sheet texture.</summary>
    public Texture2D Sheet { get; }

    /// <summary>Source rectangle of the current frame on the sheet.</summary>
    public Rectangle CurrentSourceRect => _frames[_currentFrameIndex];

    public float FrameDuration { get; set; }
    public bool IsLooping { get; set; } = true;
    public bool IsFinished { get; private set; }

    private readonly List<Rectangle> _frames;
    private float _timer;
    private int _currentFrameIndex;

    /// <summary>
    /// Slices a sprite sheet into frames by column and row count.
    /// Frames are read left-to-right, top-to-bottom.
    /// </summary>
    public Animation(Texture2D sheet, int columns, int rows, float frameDuration)
    {
        Sheet = sheet;
        FrameDuration = frameDuration;
        _frames = [];

        int frameWidth  = sheet.Width  / columns;
        int frameHeight = sheet.Height / rows;

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < columns; col++)
                _frames.Add(new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight));
    }

    public Animation(Texture2D sheet, List<Rectangle> frames, float frameDuration)
    {
        Sheet = sheet;
        _frames = frames;
        FrameDuration = frameDuration;
    }

    public override void Update(GameTime gameTime)
    {
        if (_frames.Count == 0 || IsFinished)
            return;

        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_timer >= FrameDuration)
        {
            _timer -= FrameDuration;
            int next = _currentFrameIndex + 1;
            if (next >= _frames.Count)
            {
                if (IsLooping) _currentFrameIndex = 0;
                else           IsFinished = true;
            }
            else
            {
                _currentFrameIndex = next;
            }
        }
    }

    public void Reset()
    {
        _currentFrameIndex = 0;
        _timer = 0f;
        IsFinished = false;
    }
}