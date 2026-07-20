using System;
using UnityEngine;

public static class AudioEvents
{
    public static event Action OnMenuMusicRequested;
    public static event Action<AudioClip> OnMapMusicRequested;
    public static event Action<float> OnMasterVolumeChanged;
    public static event Action<float> OnMusicVolumeChanged;
    public static event Action<float> OnSoundsVolumeChanged;

    public static void RequestMenuMusic ()
    {
        OnMenuMusicRequested?.Invoke ();
    }

    public static void RequestMapMusic (AudioClip audioClip)
    {
        OnMapMusicRequested?.Invoke (audioClip);
    }

    public static void RaiseMasterVolumeChanged (float value)
    {
        OnMasterVolumeChanged?.Invoke (value);
    }

    public static void RaiseMusicVolumeChanged (float value)
    {
        OnMusicVolumeChanged?.Invoke (value);
    }

    public static void RaiseSoundsVolumeChanged (float value)
    {
        OnSoundsVolumeChanged?.Invoke (value);
    }
}
