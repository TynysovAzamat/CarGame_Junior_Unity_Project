using UnityEngine;
using UnityEngine.UI;
public class AudioSettingsPanel : SettingsPanelBase
{
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundsSlider;

    public override void InitializeData()
    {
       
        masterSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.RemoveAllListeners();
        soundsSlider.onValueChanged.RemoveAllListeners();

        LoadSettings();

        masterSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Master", val));
        musicSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Music", val));
        soundsSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Sounds", val));
    }

    private void LoadSettings()
    {
        masterSlider.value = PlayerPrefs.GetFloat("Volume_Master", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("Volume_Music", 0.3f);
        soundsSlider.value = PlayerPrefs.GetFloat("Volume_Sounds", 0.3f);
    }
}
