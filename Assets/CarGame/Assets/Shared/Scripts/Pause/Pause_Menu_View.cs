using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Pause_Menu_View : MonoBehaviour
{
    [Header("Animation Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform mainPanel;

    [Header("Main Pause Panel")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Shared Settings Sub-Prefab")]
    [SerializeField] private SettingsWindowView settingsWindow;

    private Pause_GameState_Model _model;

    public void Init(Pause_GameState_Model model)
    {
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);

        resumeButton.onClick.AddListener(() => _model.RequestResume());
        mainMenuButton.onClick.AddListener(() => _model.RequestMainMenu());

        openSettingsButton.onClick.AddListener(() => settingsWindow.Open());
        settingsWindow.OnClosePressed += () => settingsWindow.Close();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        mainPanel.localScale = Vector3.one * 0.8f;

        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        mainPanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Hide(Action onComplete)
    {
        mainPanel.DOScale(Vector3.one * 0.8f, 0.2f).SetEase(Ease.InBack).SetUpdate(true);
        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }
    private void OnDestroy()
    {
        resumeButton.onClick.RemoveAllListeners();
        mainMenuButton.onClick.RemoveAllListeners();
        if (settingsWindow != null) settingsWindow.OnClosePressed -= () => settingsWindow.Close();
    }
}
