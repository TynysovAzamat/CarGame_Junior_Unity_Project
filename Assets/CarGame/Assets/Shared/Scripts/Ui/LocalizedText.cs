using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    [Header("Localization")]
    [SerializeField] private TMP_FontAsset englishFont;
    [SerializeField] private TMP_FontAsset russianFont;

    [Header("Dynamic Car Text")]
    [SerializeField] private float _dynamicSpeed;
    [SerializeField] private float _dynamicTurnSpeed;
    [SerializeField] private bool _hasDynamicStats;

    [SerializeField] private string localizationKey;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();

        if (string.IsNullOrEmpty(localizationKey)) localizationKey = gameObject.name;

        if (englishFont == null && _textMesh != null)
        {
            englishFont = _textMesh.font;
        }
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
        if (_textMesh == null && string.IsNullOrEmpty(localizationKey)) return;
        
        if (LocalizationManager.CurrentLanguageId == 1 && russianFont != null)
        {
            _textMesh.font = russianFont;
        }
        else if (englishFont != null)
        {
            _textMesh.font = englishFont;
        }

        if (localizationKey == "CarStatsText")
        {
            string localizedTemplate = LocalizationManager.GetTranslation(localizationKey);

            float speed = _hasDynamicStats ? _dynamicSpeed : 150f;
            float turnSpeed = _hasDynamicStats ? _dynamicTurnSpeed : 45f;

            _textMesh.text = string.Format(localizedTemplate, speed, turnSpeed);
        }
        else
        {
            _textMesh.text = LocalizationManager.GetTranslation(localizationKey);
        }
        
    }

    public void SetDynamicKey(string newKey)
    {
        localizationKey = newKey;
        UpdateText();
    }

    public void SetDynamicStats(float speed, float turnSpeed)
    {
        _hasDynamicStats = true;
        _dynamicSpeed = speed;
        _dynamicTurnSpeed = turnSpeed;
        UpdateText();
    }
}
