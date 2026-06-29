using TMPro;
using UnityEngine;
public class LanguageSettingsPanel : SettingsPanelBase
{
    [SerializeField] private TMP_Dropdown languageCombobox;
    public override void InitializeData()
    {
        base.InitializeData();

        languageCombobox.value = LocalizationManager.CurrentLanguageId;

        languageCombobox.onValueChanged.RemoveAllListeners();
        languageCombobox.onValueChanged.AddListener(index => LocalizationManager.SetLanguage(index));
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
}
