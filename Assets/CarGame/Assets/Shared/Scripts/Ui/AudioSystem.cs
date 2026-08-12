using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioSystem : MonoBehaviour
{
    private static AudioSystem instance;
    public static AudioSystem Instance => instance;


    [Header ("Master & Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    private const string MASTER_PARAM = "Volume_Master";


    [Header ("Music")]
    [SerializeField] private AudioClip _menuMusic;

    private const string MUSIC_PARAM = "Volume_Music";
    private AudioSource _musicSource;
    public AudioClip MenuMusic => _menuMusic;


    [Header ("SFX")]
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;
    [SerializeField] private int _sfxPoolSize = 12;
    [SerializeField] private float _hoverSfxCooldown = 0.07f;


    private const string SOUNDS_PARAM = "Volume_Sounds";

    private float _lastHoverSfxTime = -999f;
    private AudioSource[] _sfxPool;
    private int _sfxRoundIndex;

    private void Awake ()
    {
        if (instance != null && instance != this)
        {
            Destroy (gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad (gameObject);

        _musicSource = GetComponent<AudioSource> ();
        InitSfxPool ();
        ApplySavedVolumes ();
    }

    private void InitSfxPool ()
    {
        _sfxPool = new AudioSource[_sfxPoolSize];
        for (int i = 0; i < _sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource> ();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = _sfxMixerGroup;
            _sfxPool[i] = src;
        }
    }
    private void OnEnable ()
    {
        AudioEvents.OnMasterVolumeChanged += SetMasterVolume;
        AudioEvents.OnMusicVolumeChanged += SetMusicVolume;
        AudioEvents.OnSoundsVolumeChanged += SetSoundsVolume;
        AudioEvents.OnMenuMusicRequested += PlayMenuMusic;
        AudioEvents.OnMapMusicRequested += PlayMusic;
        AudioEvents.OnSfxReuested += PlaySfx;
        AudioEvents.OnHoverSfxRequested += PlayHoverSfx;
    }

    private void OnDisable ()
    {
        AudioEvents.OnMasterVolumeChanged -= SetMasterVolume;
        AudioEvents.OnMusicVolumeChanged -= SetMusicVolume;
        AudioEvents.OnSoundsVolumeChanged -= SetSoundsVolume;
        AudioEvents.OnMenuMusicRequested -= PlayMenuMusic;
        AudioEvents.OnMapMusicRequested -= PlayMusic;
        AudioEvents.OnSfxReuested -= PlaySfx;
        AudioEvents.OnHoverSfxRequested -= PlayHoverSfx;
    }

    private void ApplySavedVolumes ()
    {
        float masterVolume = PlayerPrefs.GetFloat ("Volume_Master", 1f);
        float musicVolume = PlayerPrefs.GetFloat ("Volume_Music", 0.3f);
        float soundsVolume = PlayerPrefs.GetFloat ("Volume_Sounds", 0.3f);

        SetMasterVolume (masterVolume);
        SetMusicVolume (musicVolume);
        SetSoundsVolume (soundsVolume);
    }

    private void PlayMenuMusic ()
    {
        PlayMusic (_menuMusic);
    }


    private void PlayMusic (AudioClip clip)
    {
        if (_musicSource.clip == clip && _musicSource.isPlaying)
            return;

        _musicSource.loop = true;
        _musicSource.clip = clip;
        _musicSource.playOnAwake = true;
        _musicSource.Play ();
    }

    private void PlaySfx (AudioClip clip)
    {
        var source = GetFreeSfxSource ();
        source.pitch = UnityEngine.Random.Range (0.85f, 1.15f);
        source.PlayOneShot (clip);
    }

    private void PlayHoverSfx (AudioClip clip)
    {
        if (Time.unscaledTime - _lastHoverSfxTime < _hoverSfxCooldown) return;
        _lastHoverSfxTime = Time.unscaledTime;

        var source = GetFreeSfxSource ();
        source.pitch = UnityEngine.Random.Range (0.85f, 1.15f);
        source.PlayOneShot (clip);
    }

    private AudioSource GetFreeSfxSource ()
    {
        foreach (var src in _sfxPool)
        {
            if (!src.isPlaying) return src;
        }

        _sfxRoundIndex = (_sfxRoundIndex + 1) % _sfxPool.Length;
        return _sfxPool[_sfxRoundIndex];
    }
    private void SetMasterVolume (float volume)
    {
        SetMixerVolume (MASTER_PARAM, volume);
    }

    private void SetMusicVolume (float volume)
    {
        SetMixerVolume (MUSIC_PARAM, volume);
    }

    private void SetSoundsVolume (float volume)
    {
        SetMixerVolume (SOUNDS_PARAM, volume);
    }

    private void SetMixerVolume (string parameterName, float linearValue)
    {
        if (_audioMixer == null) return;

        float clamped = Mathf.Clamp (linearValue, 0.0001f, 1.2f);
        float decibels = Mathf.Log10 (clamped) * 20f;
        Debug.Log ($"[AudioSystem] param={parameterName}, linearValue={linearValue}, decibels={decibels}");

        bool success = _audioMixer.SetFloat (parameterName, decibels);
        if (!success)
        {
            Debug.LogError ($"[AudioSystem] Failed to set mixer parameter '{parameterName}'. Check that it's exposed in the AudioMixer with this exact name.");
        }
    }
}
