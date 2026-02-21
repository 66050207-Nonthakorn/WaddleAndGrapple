using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace ComputerGameFinal.Engine.Managers;

public class AudioManager
{
    public static AudioManager Instance { get; private set; } = new AudioManager();
    
    public static float Volume
    {
        get => MediaPlayer.Volume;
        set => MediaPlayer.Volume = Math.Clamp(value, 0f, 1f);
    }
    
    private readonly Dictionary<string, SoundEffect> _soundsEffects = [];
    private readonly Dictionary<string, Song> _songs = [];
    
    private AudioManager() { }

    public void LoadSound(string name, SoundEffect sound)
    {
        _soundsEffects[name] = sound;
    }
    
    public void PlaySound(string name)
    {
        if (_soundsEffects.TryGetValue(name, out var sound))
        {
            sound.Play();
        }
    }

    public void LoadSong(string name, Song song)
    {
        _songs[name] = song;
    }

    public void PlaySong(string name, bool isRepeating = true)
    {
        if (_songs.TryGetValue(name, out var song))
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = isRepeating;
            MediaPlayer.Play(song);
        }
    }
}