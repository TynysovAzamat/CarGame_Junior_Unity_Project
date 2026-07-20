using UnityEngine;
using UnityEngine.UI;
using System;
public class AudioSettingsPanel : SettingsPanelBase
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundsSlider;

    public event Action<float > OnMasterVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;
    public event Action<float> OnSoundsVolumeChanged;
    public override void InitializeData()
    {
        base.InitializeData();
        ClearListeners();

        masterSlider.onValueChanged.AddListener(val => OnMasterVolumeChanged?.Invoke(val));
        musicSlider.onValueChanged.AddListener(val => OnMusicVolumeChanged?.Invoke(val));
        soundsSlider.onValueChanged.AddListener(val => OnSoundsVolumeChanged?.Invoke(val));
    }
    public void SetupSliders(float master, float music, float sounds)
    {
        masterSlider.value = master;
        musicSlider.value = music;
        soundsSlider.value = sounds;
    }
    private void ClearListeners()
    {
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        soundsSlider.onValueChanged.RemoveAllListeners();
    }
    private void OnDestroy()
    {
        ClearListeners();
        if (closeButton != null) closeButton.onClick.RemoveAllListeners();
    }
}
