using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

public class UIHoldButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
{
    [InputControl (layout = "Button")]
    [SerializeField] private string m_ControlPath;
    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SendValueToControl (1.0f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SendValueToControl (0.0f);
    }
}
