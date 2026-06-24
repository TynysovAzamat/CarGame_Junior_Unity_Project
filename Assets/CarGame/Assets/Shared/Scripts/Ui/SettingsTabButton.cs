using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsTabButton : MonoBehaviour
{
    [SerializeField] private string targetPanelId;

    private Button button;
    public event Action<string> OnTabClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => OnTabClicked?.Invoke(targetPanelId));
    }
}
