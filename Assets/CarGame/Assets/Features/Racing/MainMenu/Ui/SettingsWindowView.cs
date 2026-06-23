using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
public class SettingsWindowView : MonoBehaviour
{
    [Header("Animation Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRectTransform;

    [Header("Sound Category")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundsSlider;

    [Header("Language Category")]
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Navigation")]
    [SerializeField] private Button closeButton;

    public event Action OnClosePressed;

    private void Awake()
    {
        closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());

        masterSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Master", val));
        musicSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Music", val));
        soundsSlider.onValueChanged.AddListener(val => PlayerPrefs.SetFloat("Volume_Sounds", val));

        languageDropdown.onValueChanged.AddListener(index => LocalizationManager.SetLanguage(index));
    }

    public void Open()
    {
        gameObject.SetActive(true);

        masterSlider.value = PlayerPrefs.GetFloat("Volume_Master", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("Volume_Music", 1f);
        soundsSlider.value = PlayerPrefs.GetFloat("Volume_Sounds", 1f);
        languageDropdown.value = LocalizationManager.CurrentLanguageId;

        canvasGroup.alpha = 0f;
        if (panelRectTransform != null) panelRectTransform.localScale = Vector3.one * 0.5f;

        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);

        if (panelRectTransform != null)
        {
            panelRectTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    public void Close(Action onComplete = null)
    {
        PlayerPrefs.Save();

        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);

        if (panelRectTransform != null)
        {
            panelRectTransform.DOScale(Vector3.one * 0.7f, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true);
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }).SetUpdate(true);
    }
}

