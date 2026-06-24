using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
public class Racing_Gameplay_View : MonoBehaviour
{
    [SerializeField] private CanvasGroup hudCanvasGroup;
    [SerializeField] private Joystick goJoystickImage;
    [SerializeField] private Button pauseButton;

    public event Action<Vector2> OnJoystickInputChanged;
    public event Action OnPauseClicked;
    private void Awake()
    {
        if (hudCanvasGroup == null || goJoystickImage == null)
        {
            Debug.LogError($"Racing_Gameplay_View on Prefab {gameObject.name} does not have CanvasGroup/GoButton components configured");
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => OnPauseClicked?.Invoke());
        }
    }

    public void AnimateIn()
    {
        // пр€чем кнопнку
        if (hudCanvasGroup == null) return;
        hudCanvasGroup.alpha = 0f;
        hudCanvasGroup.DOFade(1f, 0.3f);
    }

    public void AnimateOut(Action onComplete = null)
    {
        if (hudCanvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        hudCanvasGroup.DOFade(0f, 0.3f).OnComplete(() => onComplete?.Invoke());
    }

    public void SetInputActive(bool isActive)
    {
        if (TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.blocksRaycasts = isActive;
            canvasGroup.interactable = isActive;
        }
    }

    public void ResetJoystick()
    {
        if (goJoystickImage != null)
        {
            goJoystickImage.OnPointerUp(null);
        }
    }

    private void Update()
    {
        if (goJoystickImage == null) return;

        Vector2 joystickVector = new Vector2(goJoystickImage.Horizontal, goJoystickImage.Vertical);
        OnJoystickInputChanged?.Invoke(joystickVector);
    }

    private void OnDestroy()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
        }
    }
}
