using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioSystem : MonoBehaviour
{
    private static AudioSystem instance;
    public static AudioSystem Instance => instance;

    [SerializeField] private AudioClip _menuMusic;
    [SerializeField] private AudioMixer _audioMixer;

    private const string MASTER_PARAM = "MasterVolume";
    private const string MUSIC_PARAM = "MusicVolume";
    private const string SOUNDS_PARAM = "SoundsVolume";

    private AudioSource _musicSource;

    public AudioClip MenuMusic => _menuMusic;
    private void Awake ()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        _musicSource = GetComponent<AudioSource>();
        ApplySavedVolumes ();
    }

    private void OnEnable ()
    {
        AudioEvents.OnMasterVolumeChanged += SetMasterVolume;
        AudioEvents.OnMusicVolumeChanged += SetMusicVolume;
        AudioEvents.OnSoundsVolumeChanged += SetSoundsVolume;
        AudioEvents.OnMenuMusicRequested += PlayMenuMusic;
        AudioEvents.OnMapMusicRequested += PlayMusic;
    }

    private void OnDisable ()
    {
        AudioEvents.OnMasterVolumeChanged -= SetMasterVolume;
        AudioEvents.OnMusicVolumeChanged -= SetMusicVolume;
        AudioEvents.OnSoundsVolumeChanged -= SetSoundsVolume;
        AudioEvents.OnMenuMusicRequested -= PlayMenuMusic;
        AudioEvents.OnMapMusicRequested -= PlayMusic;
    }

    private void ApplySavedVolumes ()
    {
        float masterVolume = PlayerPrefs.GetFloat("Volume_Master", 1f);
        float musicVolume = PlayerPrefs.GetFloat("Volume_Music", 0.3f);
        float soundsVolume = PlayerPrefs.GetFloat("Volume_Sounds", 0.3f);
    }

    private void PlayMenuMusic ()
    {
        PlayMusic(_menuMusic);
    }


    private void PlayMusic (AudioClip clip)
    {
        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.loop = true;
        _musicSource.clip = clip;
        _musicSource.playOnAwake = true;
        _musicSource.Play();
    }

    private void SetMasterVolume(float volume)
    {
        SetMixerVolume(MASTER_PARAM, volume);
    }

    private void SetMusicVolume(float volume)
    {
        SetMixerVolume (MUSIC_PARAM, volume);
    }

    private void SetSoundsVolume(float volume)
    {
        SetMixerVolume(SOUNDS_PARAM, volume);
    }

    private void SetMixerVolume(string parameterName, float linearValue)
    {
        if (_audioMixer == null) return;

        float clamped = Mathf.Clamp (linearValue, 0.0001f, 1.2f);
        float decibels = Mathf.Log10 (clamped) * 20f;
        _audioMixer.SetFloat(parameterName, decibels);
    }
}
