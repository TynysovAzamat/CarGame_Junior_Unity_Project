using UnityEngine;

public static class UiButtonInteraction
{
    public static void SetInputActive(this CanvasGroup canvasGroup, bool isActive)
    {
        if (canvasGroup == null) return;

        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }
}
