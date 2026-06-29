using UnityEngine;

public abstract class BaseMenuView : MonoBehaviour
{
    [Header("Base Ui Components")]
    [SerializeField] private CanvasGroup mainCanvasGroup;
    [SerializeField] private RectTransform mainRectTransform;
    public CanvasGroup MainCanvasGroup => mainCanvasGroup;
    public RectTransform MainRectTransform => mainRectTransform;
    protected virtual void Awake()
    {
        if (mainCanvasGroup == null)
        {
            mainCanvasGroup = GetComponent<CanvasGroup>();
            if (mainCanvasGroup == null)
            {
                Debug.LogError($"[UI] CanvasGroup отсутствует на объекте {gameObject.name}!");
            }
        }

        if (mainRectTransform == null)
        {
            mainRectTransform = GetComponent<RectTransform>();
            if (MainCanvasGroup == null)
            {
                Debug.LogError($"[UI] RectTransform отсутствует на объекте {gameObject.name}!");
            }
        }
    }
}
