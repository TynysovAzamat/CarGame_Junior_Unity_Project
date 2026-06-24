using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class SettingsPanelBase : MonoBehaviour
{
    [SerializeField] private string panelId;
    public string PanelId => panelId;

    protected CanvasGroup canvasGroup;
    private const float AnimDuration = 0.2f;

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public abstract void InitializeData();

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, AnimDuration).SetUpdate(true);
    }

    public virtual void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0f, AnimDuration).SetUpdate(true).OnComplete(() => gameObject.SetActive(false));
    }

    public void HideInstant()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.DOKill();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false);
    }
}
