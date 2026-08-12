using System;
using UnityEngine;

public class AudioSettings_Model
{
    public event Action<float> OnMasterVolumeChanged;
    public event Action<float> OnSoundsVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    private const string MASTER_KEY = "Volume_Master";
    private const string MUSIC_KEY = "Volume_Music";
    private const string SOUNDS_KEY = "Volume_Sounds";

    private const float DEFAULT_MASTER = 1f;
    private const float DEFAULT_MUSIC = 0.3f;
    private const float DEFAULT_SOUNDS = 0.3f;

    public float MasterVolume { get; private set; }
    public float SoundsVolume { get; private set; }
    public float MusicVolume { get; private set; }

    public void Load ()
    {
        MasterVolume = PlayerPrefs.GetFloat (MASTER_KEY, DEFAULT_MASTER);
        SoundsVolume = PlayerPrefs.GetFloat (SOUNDS_KEY, DEFAULT_SOUNDS);
        MusicVolume = PlayerPrefs.GetFloat (MUSIC_KEY, DEFAULT_MUSIC);
    }

    public void SetMasterVolume (float value)
    {
        MasterVolume = Mathf.Clamp01 (value);
        PlayerPrefs.SetFloat (MASTER_KEY, MasterVolume);
        OnMasterVolumeChanged?.Invoke (MasterVolume);
    }

    public void SetSoundsVolume (float value)
    {
        SoundsVolume = Mathf.Clamp01 (value);
        PlayerPrefs.SetFloat (SOUNDS_KEY, SoundsVolume);
        OnSoundsVolumeChanged?.Invoke (SoundsVolume);
    }

    public void SetMusicVolume (float value)
    {
        MusicVolume = Mathf.Clamp01 (value);
        PlayerPrefs.SetFloat (MUSIC_KEY, MusicVolume);
        OnMusicVolumeChanged?.Invoke (MusicVolume);
    }
}