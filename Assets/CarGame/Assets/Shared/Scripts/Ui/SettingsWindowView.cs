using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SettingsWindowView : MonoBehaviour
{
    [Header("Animation Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelRectTransform;

    [Header("Polymorphic Navigation & Panels")]
    [SerializeField] private List<SettingsTabButton> tabButtons;
    [SerializeField] private List<SettingsPanelBase> settingsPanels;
    [SerializeField] private string defaultPanelId = "Audio";

    [Header("Navigation")]
    [SerializeField] private Button closeButton;

    private Dictionary<string, SettingsPanelBase> panelsDictionary = new Dictionary<string, SettingsPanelBase>();
    private SettingsPanelBase currentActivePanel;
    public event Action OnClosePressed;
    private Pause_GameState_Model _pauseModel;

    private void Awake()
    {
        DOTween.Init();

        DOTween.defaultUpdateType = UpdateType.Normal;
        DOTween.defaultTimeScaleIndependent = true;
    }

    public void Init(Pause_GameState_Model model)
    {
        _pauseModel = model;
    }

    public void Open()
    {
        gameObject.SetActive(true);

        foreach (var panel in settingsPanels)
        {
            if (panel != null) panel.InitializeData();
        }

        SwitchCategory(defaultPanelId);

        canvasGroup.alpha = 0f;
        if (panelRectTransform != null) panelRectTransform.localScale = Vector3.one * 0.5f;

        canvasGroup.DOFade(1f, 0.3f).SetUpdate(true);

        if (panelRectTransform != null)
        {
            panelRectTransform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    public void Close(Action onComplete = null)
    {
        PlayerPrefs.Save();

        canvasGroup.DOFade(0f, 0.2f).SetUpdate(true);

        if (panelRectTransform != null)
        {
            panelRectTransform.DOScale(Vector3.one * 0.7f, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true);
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }).SetUpdate(true);
    }

    private void SwitchCategory(string panelId)
    {
        if (!panelsDictionary.ContainsKey(panelId)) return;
        SettingsPanelBase targetPanel = panelsDictionary[panelId];
        if (currentActivePanel == targetPanel) return;

        if (currentActivePanel != null) currentActivePanel.Hide();
        targetPanel.Show();
        currentActivePanel = targetPanel;
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        foreach (var button in tabButtons)
        {
            if (button != null) button.OnTabClicked -= SwitchCategory;
        }
    }
}

