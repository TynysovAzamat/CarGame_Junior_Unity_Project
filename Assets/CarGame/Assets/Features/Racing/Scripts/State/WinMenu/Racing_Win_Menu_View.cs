using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class Racing_Win_Menu_View : BaseMenuView
{
    // Testing for cmd git commands

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;

    public event Action OnPlayAgainRequested;
    public event Action OnMainMenuRequested;
    public event Action OnNextLevelRequested;

    public void Init()
    {
        if (MainCanvasGroup == null || playAgainButton == null || mainMenuButton == null || nextLevelButton == null)
        {
            Debug.LogError("[WinMenu] Ошибка: Не все ссылки на UI элементы установлены в инспекторе!");
            return;
        }

        MainCanvasGroup.alpha = 0f;
        MainCanvasGroup.DOFade (1f, 0.5f).SetUpdate (true);

       playAgainButton.onClick.AddListener(() => OnPlayAgainRequested?.Invoke());
       nextLevelButton.onClick.AddListener(() => OnNextLevelRequested?.Invoke());
       mainMenuButton.onClick.AddListener(() => OnMainMenuRequested?.Invoke());
    }

    public void CloseAnimate(Action OnComplete)
    {
        if (MainCanvasGroup == null) return;

        
        MainCanvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            OnComplete?.Invoke();
            Destroy (gameObject);
        });
    }
    private void OnDestroy()
    {
        if (playAgainButton != null) playAgainButton.onClick.RemoveAllListeners();
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveAllListeners();
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveAllListeners();
    }
}
