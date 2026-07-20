using System;
using UnityEngine;
public class AudioSettings_Model
{
    public event Action<float> OnMasterVolumeChanged;
    public event Action<float> OnSoundsVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    public float MasterVolume { get; private set; }
    public float SoundsVolume { get; private set; }
    public float MusicVolume { get; private set; }

    public void Load()
    {
        MasterVolume = PlayerPrefs.GetFloat("Volume_Master", 1f);
        SoundsVolume = PlayerPrefs.GetFloat("Volume_Sounds", 0.3f);
        MusicVolume = PlayerPrefs.GetFloat("Volume_Music", 0.3f);
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        PlayerPrefs.SetFloat("Volume_Master", value);
        OnMasterVolumeChanged?.Invoke(value);
    }

    public void SetSoundsVolume(float value)
    {
        SoundsVolume = value;
        PlayerPrefs.SetFloat("Volume_Sounds", value);
        OnSoundsVolumeChanged?.Invoke(value);
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = value;
        PlayerPrefs.SetFloat("Volume_Music", value);
        OnMusicVolumeChanged?.Invoke(value);
    }
}

