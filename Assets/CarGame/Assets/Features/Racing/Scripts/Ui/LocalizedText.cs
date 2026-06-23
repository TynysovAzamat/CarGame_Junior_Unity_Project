using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string localizationKey;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();

        if (string.IsNullOrEmpty(localizationKey)) localizationKey = gameObject.name;
    }

    private void OnEnable()
    {
        UpdateText();
        LocalizationManager.OnLanguageChanged += UpdateText;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (_textMesh != null && !string.IsNullOrEmpty(localizationKey))
        {
            if (localizationKey == "CarSettingsText") return;
            _textMesh.text = LocalizationManager.GetTranslation(localizationKey);
        } 
    }
}
