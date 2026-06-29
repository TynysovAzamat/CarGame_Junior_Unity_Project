using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SettingsWindowView : BaseMenuView
{
    public event Action OnClosePressed;

    [Header("Components")]
    [SerializeField] private Button closeButton;

    [Header("Tabs & Panels")]
    [SerializeField] private GameObject mainTabsContainer;
    [SerializeField] private string defaultPanelId = "Settings";
    [SerializeField] private List<SettingsTabButton> tabButtons;
    [SerializeField] private List<SettingsPanelBase> settingsPanels;

    private Dictionary<string, SettingsPanelBase> panelsDictionary = new Dictionary<string, SettingsPanelBase>();
    private SettingsPanelBase currentActivePanel = null;
    private Pause_GameState_Model _pauseModel;

    protected override void Awake()
    {
        base.Awake();

        DOTween.Init();

        DOTween.defaultUpdateType = UpdateType.Normal;
        DOTween.defaultTimeScaleIndependent = true;

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => OnClosePressed?.Invoke());
        }

        panelsDictionary.Clear();
        foreach (var panel in settingsPanels)
        {
            if (panel != null && !panelsDictionary.ContainsKey(panel.PanelId))
            {
                panelsDictionary.Add(panel.PanelId, panel);
            }
        }

        
    }

    public void Init(Pause_GameState_Model model)
    {
        _pauseModel = model;
    }

    public void Open(Action onComplete = null)
    {
        gameObject.SetActive(true);

        if (MainCanvasGroup != null) MainCanvasGroup.SetInputActive(true);

        foreach (var panel in settingsPanels)
        {
            if (panel != null) 
            {
                panel.InitializeData();
                panel.HideInstant();

                panel.OnCloseRequested -= HandlePanelBackRequest;
                panel.OnCloseRequested += HandlePanelBackRequest;
            } 
        }

        foreach (var tabButton in tabButtons)
        {
            if (tabButton != null)
            {
                tabButton.OnTabClicked -= SwitchCategory;
                tabButton.OnTabClicked += SwitchCategory;
            }
        }

        currentActivePanel = null;

        SwitchCategory(defaultPanelId);

        if (MainCanvasGroup != null) MainCanvasGroup.alpha = 0f;
        if (MainRectTransform != null) MainRectTransform.localScale = Vector3.one * 0.5f;
        if (MainCanvasGroup != null) MainCanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);

        if (MainRectTransform != null)
        {
            MainRectTransform.DOScale(Vector3.one, 0.25f)
             .SetEase(Ease.OutBack)
             .SetUpdate(true)
             .OnComplete(() =>
             {
                 onComplete?.Invoke();
             });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void Close(Action onComplete = null)
    {
        PlayerPrefs.Save();

        if (MainCanvasGroup != null) MainCanvasGroup.DOFade(0f, 0.2f).SetUpdate(true);

        if (MainRectTransform != null)
        {
            MainRectTransform.DOScale(Vector3.one * 0.7f, 0.2f)
                .SetEase(Ease.InBack)
                .SetUpdate(true);
        }

        DOVirtual.DelayedCall(0.2f, () =>
        {
            gameObject.SetActive(false);
            onComplete?.Invoke();
        }).SetUpdate(true);
    }
    private void HandlePanelBackRequest()
    {
        if (currentActivePanel != null)
        {
            currentActivePanel.Hide();
            currentActivePanel = null;
        }

        if (mainTabsContainer != null)
        {
            mainTabsContainer.SetActive(true);
        }
    }
    private void SwitchCategory(string panelId)
    {
        if (panelId == "Settings")
        {
            if (currentActivePanel != null) currentActivePanel.Hide();
            currentActivePanel = null;

            if (mainTabsContainer != null) mainTabsContainer.SetActive(true);
            return;
        }

        if (!panelsDictionary.ContainsKey(panelId)) return;

        SettingsPanelBase targetPanel = panelsDictionary[panelId];
        if (currentActivePanel == targetPanel) return;

        if (currentActivePanel != null) currentActivePanel.Hide();

        if (mainTabsContainer != null) mainTabsContainer.SetActive(false);

        targetPanel.Show();
        currentActivePanel = targetPanel;
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
        foreach (var panel in settingsPanels)
        {
            if (panel != null) panel.OnCloseRequested -= HandlePanelBackRequest;
        }

        foreach (var button in tabButtons)
        {
            if (button != null) button.OnTabClicked -= SwitchCategory;
        }
    }
}

