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
    [SerializeField] private Button restartButton;

    [Header("Shared Settings Sub-Prefab")]
    [SerializeField] private SettingsWindowView settingsWindow;

    private Pause_GameState_Model _model;
    public CanvasGroup CanvasGroup => canvasGroup;
    public void Init(Pause_GameState_Model model)
    {
        _model = model;

        if (settingsWindow != null)
        {
          settingsWindow.Init(_model);
        }



        resumeButton?.onClick.AddListener(() => _model.RequestResume());
        mainMenuButton?.onClick.AddListener(() => _model.RequestMainMenu());
        openSettingsButton?.onClick.AddListener(() => settingsWindow.Open());

        restartButton.onClick.AddListener(() =>
        {
            Debug.Log("<color=yellow>[Pause View] Кнопка RESTART физически нажата в Unity!</color>");
            _model.RequestRestart();
        });
    }

    public void Show(Action onComplete = null)
    {
        gameObject.SetActive(true);

        canvasGroup.SetInputActive(false);
        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);


        mainPanel.localScale = Vector3.zero;

        mainPanel.DOScale(Vector3.one, 0.3f)
        .SetEase(Ease.OutBack)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            onComplete?.Invoke();
        });
    }

    public void Hide(Action onComplete)
    {
        canvasGroup.SetInputActive(false);
        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);

        mainPanel.DOScale(Vector3.zero, 0.2f)
        .SetEase(Ease.InBack)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        });

    }
    private void OnDestroy()
    {
        resumeButton?.onClick.RemoveAllListeners();
        mainMenuButton?.onClick.RemoveAllListeners();
        openSettingsButton?.onClick.RemoveAllListeners();
        restartButton?.onClick.RemoveAllListeners();
        if (settingsWindow != null) settingsWindow.OnClosePressed -= () => settingsWindow.Close();
    }

    public void SubscribeToRestart(UnityEngine.Events.UnityAction action)
    {
        // Очищаем только старые системные листенеры Unity, если они были в префабе
        restartButton?.onClick.RemoveAllListeners();
        // Добавляем лог + само действие
        restartButton.onClick.AddListener(() => {
            Debug.Log("<color=yellow>[Pause View] Кнопка RESTART физически нажата! Вызываю HandleRestart напрямую...</color>");
        });
        restartButton.onClick.AddListener(action);
    }

    public void SubscribeToResume(UnityEngine.Events.UnityAction action)
    {
        resumeButton?.onClick.RemoveAllListeners();
        resumeButton?.onClick.AddListener(action);
    }

    public void SubscribeToMainMenu(UnityEngine.Events.UnityAction action)
    {
        mainMenuButton?.onClick.RemoveAllListeners();
        mainMenuButton?.onClick.AddListener(action);
    }
}
