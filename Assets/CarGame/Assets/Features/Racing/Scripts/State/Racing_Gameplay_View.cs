using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class Racing_Gameplay_View : BaseMenuView
{

<<<<<<< HEAD:Assets/CarGame/Assets/Features/Racing/Scripts/State/Racing_Gameplay_View.cs

    [Header ("Base Menu Ui")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private CameraFollow3D cameraFollow;

    public CameraFollow3D CameraFollow => cameraFollow;
    public event Action<Vector2> OnInputVectorChanged;
=======
    [SerializeField] private CameraFollow3D cameraFollow;

    public event Action<Vector2> OnJoystickInputChanged;
>>>>>>> 6a59bfe (making the smooth camera follow, camera now softlymove towards the player, when contacting(colliding) an obstacle):Assets/CarGame/Assets/Features/Racing/Gameplay/Racing_Gameplay_View.cs
    public event Action OnPauseClicked;

    public CameraFollow3D CameraFollow => cameraFollow;
    protected override void Awake()
    {

        if (MainCanvasGroup == null)
        {
            Debug.LogError($"Racing_Gameplay_View on Prefab {gameObject.name} does not have CanvasGroup/GoButton components configured");
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(() => OnPauseClicked?.Invoke());
        }

        if (cameraFollow == null)
        {
            cameraFollow = FindObjectOfType<CameraFollow3D>();

            if (cameraFollow == null)
            {
                Debug.LogWarning ("CameraFollow3D не найдена на сцене Gameplay! Проверьте, добавлен ли этот скрипт на Main Camera.");
            }
        }
    }

    public void AnimateIn()
    {
        // прячем кнопнку
        if (MainCanvasGroup == null) return;
        MainCanvasGroup.alpha = 0f;
        MainCanvasGroup.DOFade(1f, 0.3f);
    }

    public void AnimateOut(Action onComplete = null)
    {
        if (MainCanvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        MainCanvasGroup.DOFade(0f, 0.3f).OnComplete(() => onComplete?.Invoke());
    }

    public void HandleNewInput (Vector2 incomingInput)
    {
        OnInputVectorChanged?.Invoke (incomingInput);
    }
    public void SetInputActive (bool isActive)
    {
        if (TryGetComponent<CanvasGroup> (out var canvasGroup))
        {
            canvasGroup.SetInputActive (isActive);
        }
        if (!isActive)
        {
            ClearAllInputs ();
        }
    }

    public void ClearAllInputs ()
    {
        OnInputVectorChanged?.Invoke (Vector2.zero);
    }

    private void OnDestroy ()
    {
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners ();
        }
    }
}
