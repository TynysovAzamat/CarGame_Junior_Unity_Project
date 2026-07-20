using UnityEngine;
public class AudioSettings_GameState : IGameState
{
    private readonly AudioSettingsPanel _view;
    private readonly AudioSettings_Model _model;

    public AudioSettings_GameState(AudioSettingsPanel view, AudioSettings_Model model)
    {
        _view = view;
        _model = model;
    }

    public void Enter()
    {
        if (_model == null || _view == null)
        {
            Debug.LogError("[AudioSettings_GameState] Model or View is null. Cannot enter state.");
            return;
        }
        _model.Load();

        
        _view.InitializeData();


        _view.SetupSliders(_model.MasterVolume, _model.MusicVolume, _model.SoundsVolume);

        _view.OnSoundsVolumeChanged += _model.SetSoundsVolume;
        _view.OnMasterVolumeChanged += _model.SetMasterVolume;
        _view.OnMusicVolumeChanged += _model.SetMusicVolume;

        _model.OnMasterVolumeChanged += AudioEvents.RaiseMusicVolumeChanged;
        _model.OnMusicVolumeChanged += AudioEvents.RaiseMusicVolumeChanged;
        _model.OnSoundsVolumeChanged += AudioEvents.RaiseSoundsVolumeChanged;
    }

    public void Exit()
    {
        if (_view != null)
        {
            _view.OnMasterVolumeChanged -= _model.SetMasterVolume;
            _view.OnMusicVolumeChanged -= _model.SetMusicVolume;
            _view.OnSoundsVolumeChanged -= _model.SetSoundsVolume;
        }

        if (_model != null )
        {
            _model.OnMusicVolumeChanged -= AudioEvents.RaiseMusicVolumeChanged;
            _model.OnSoundsVolumeChanged-= AudioEvents.RaiseSoundsVolumeChanged;
            _model.OnMasterVolumeChanged-= AudioEvents.RaiseMasterVolumeChanged;
        }
    }
}
