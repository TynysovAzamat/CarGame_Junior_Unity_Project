using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public abstract class SettingsPanelBase : MonoBehaviour
{
    public event Action OnCloseRequested;

    [SerializeField] private string panelId;
    public string PanelId => panelId;

    [Header("Base Navigation")]
    [SerializeField] protected Button closeButton;

    protected CanvasGroup canvasGroup;
    private const float AnimDuration = 0.2f;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void InitializeData()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => OnCloseRequested?.Invoke());
        }
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.SetInputActive(true);

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, AnimDuration).SetUpdate(true);
    }

    public virtual void Hide()
    {
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, AnimDuration).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void HideInstant()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.SetInputActive(false);

        gameObject.SetActive(false);
    }
}
