using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Pause_Menu_View : BaseMenuView
{

    [Header("Main Pause Panel")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button restartButton;

    [Header("Settings Menu Panel")]
    [SerializeField] private Button settingsButton;
    [SerializeField] private GameObject pauseMenuPanel; 

    private Pause_GameState_Model _model;
    
    public void Init(Pause_GameState_Model model)
    {
        _model = model;

        if (resumeButton != null) resumeButton.onClick.AddListener(() => _model.RequestResume());
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(() => _model.RequestMainMenu());

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                Debug.Log("<color=yellow>[Pause View] Кнопка RESTART физически нажата в Unity!</color>");
                _model.RequestRestart();
            });
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => _model.RequestSettings());
        }
    }

    public void Show(Action onComplete = null)
    {
        gameObject.SetActive(true);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        MainCanvasGroup.SetInputActive(false);

        MainCanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
        MainRectTransform.localScale = Vector3.zero;

        MainRectTransform.DOScale(Vector3.one, 0.3f)
        .SetEase(Ease.OutBack)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            MainCanvasGroup.SetInputActive(true);
            onComplete?.Invoke();
        });
    }

    public void Hide(Action onComplete)
    {
        MainCanvasGroup.SetInputActive(false);
        MainCanvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
        MainRectTransform.DOScale(Vector3.zero, 0.2f)

        .SetEase(Ease.InBack)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });

    }
    public void SubscribeToRestart(UnityEngine.Events.UnityAction action)
    {
        // Очищаем только старые системные листенеры Unity, если они были в префабе
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
        // Добавляем лог + само действие
        restartButton.onClick.AddListener(() => {
            Debug.Log("<color=yellow>[Pause View] Кнопка RESTART физически нажата! Вызываю HandleRestart напрямую...</color>");
        });
        restartButton.onClick.AddListener(action);
    }

    public void SubscribeToResume(UnityEngine.Events.UnityAction action)
    {
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (resumeButton != null) resumeButton.onClick.AddListener(action);
    }

    public void SubscribeToMainMenu(UnityEngine.Events.UnityAction action)
    {
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(action);
    }
    public void AnimateIn(Action onComplete = null)
    {
        if (MainCanvasGroup == null) return;
        MainCanvasGroup.DOFade(1f, 0.5f).OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateOut(Action onComplete = null)
    {
        if (MainCanvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        MainCanvasGroup.DOFade(0f, 0.4f)
        .OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void OnDestroy()
    {
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (restartButton != null) restartButton.onClick.RemoveAllListeners();
    }
}
