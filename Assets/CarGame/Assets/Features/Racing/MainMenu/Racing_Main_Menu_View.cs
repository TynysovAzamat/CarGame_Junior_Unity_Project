using System;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Racing_Main_Menu_View : BaseMenuView
{
    public event Action<RacingLevelData> OnLevelButtonClicked;
    public event Action OnExitButtonClicked;
    public event Action OnSettingsButtonClicked;
    // ���� ��� ������ � ������� ��������
    [Header("Main Panels")]
    [SerializeField] private Transform levelsPanel;
    [SerializeField] private Transform carsPanel;

    [Header("Navigation Buttons")]
    [SerializeField] private Button openLevelsButton;
    [SerializeField] private Button openCarsButton;
    [SerializeField] private Button closeLevelsButton;
    [SerializeField] private Button closeCarsButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button settingsButton;
 
    [Header("Dynamic Content Containers")]
    [SerializeField] private Transform levelsContentContainer;

    [Header("Prefabs")]
    [SerializeField] private Button levelButtonPrefab;

    [Header("Carousel Text Elements")]
    [SerializeField] private TextMeshProUGUI _carNameText;
    [SerializeField] private TextMeshProUGUI _statsText;

    [Header("Carousel Componet")]
    [SerializeField] private CarCarousel_Ui _carCarousel_Ui;

    private readonly CarConfigData _cachedCenterCar;
    private readonly CarConfigData _cachedLeftCar;
    private readonly CarConfigData _cachedRightCar;

    private Racing_Main_Menu_Model _model;
    public void Init(Racing_Main_Menu_Model model)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));

        if (MainCanvasGroup == null || levelsPanel == null || carsPanel == null)
        {
            Debug.LogError("Main menu view is not properly set up. Please assign all required references.");
            return;
        }

        // ����� ��������� ��������
        MainCanvasGroup.alpha = 0f;
        levelsPanel.localScale = Vector3.zero;
        carsPanel.localScale = Vector3.zero;

        // ��������� �� �������
        if (openLevelsButton != null) openLevelsButton.onClick.AddListener(() => TogglePanel3D(levelsPanel, true));
        if (closeLevelsButton != null) closeLevelsButton.onClick.AddListener(() => TogglePanel3D(levelsPanel, false));
        if (openCarsButton != null) openCarsButton.onClick.AddListener(() => TogglePanel3D(carsPanel, true));
        if (closeCarsButton != null) closeCarsButton.onClick.AddListener(() => TogglePanel3D(carsPanel, false));
        if (exitGameButton != null) exitGameButton.onClick.AddListener(HandleExitClick);
        if (settingsButton != null) settingsButton.onClick.AddListener(HandleSettingsButton);

        // ������ ������ ����� � �������
        BuildLevelsList();

        _carCarousel_Ui.Setup(_model);

        UpdateTextAndStats(_model.CurrentCar);
        model.OnCarCarouselChanged += HandleCarCarouselChanged;
    }
    
    private void HandleCarCarouselChanged(CarConfigData carConfigData, Racing_Main_Menu_Model.SelectionDirection direction)
    {
        UpdateTextAndStats(carConfigData);
    }

    private void UpdateTextAndStats(CarConfigData car)
    {
        if (car == null) return;
        if (_carNameText != null) _carNameText.text = car.CarName;
        if (_statsText != null)
        {
            _statsText.text = $" SPEED: {car.MaxSpeed}\n TURN SPEED: {car.MaxTurnSpeed}";
        }
    }

    public void AnimateIn(Action onComplete = null)
    {
        if (MainCanvasGroup == null) return;
        MainCanvasGroup.DOFade(1f, 0.5f).OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateOut(Action onComplete = null)
    {
        if (MainCanvasGroup == null)
        {
            onComplete?.Invoke();
            return;
        }

        MainCanvasGroup.DOFade(0f, 0.4f)
        .OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    private void TogglePanel3D(Transform panel, bool show)
    {
        if (panel == null) return;

        Vector3 targetScale = show ? Vector3.one : Vector3.zero;
        panel.DOScale(targetScale, 0.5f).SetEase(show ? Ease.OutBack : Ease.InQuad);
    }

    private void BuildLevelsList()
    {
        if (levelsContentContainer == null || levelButtonPrefab == null)
        {
           Debug.LogError("Levels content container or level button prefab is not assigned. Cannot build levels list.");
           return;
        }

        foreach (Transform child in levelsContentContainer) Destroy(child.gameObject);

        // ���� ������� �������
        foreach (var level in _model.AvailableLevels)
        {
            if (level == null)
            {
                Debug.LogWarning("Encountered null level data. Skipping.");
                continue;
            }

            var buttonInstance = Instantiate(levelButtonPrefab, levelsContentContainer);

            var textComponent = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null) textComponent.text = LocalizationManager.GetTranslation(level.LevelName);

            RacingLevelData currentLevelCopy = level;

            buttonInstance.onClick.AddListener(() =>
            {
                Debug.Log($"[MainMenu_View] Click on button level: {level.LevelName}");
                OnLevelButtonClicked?.Invoke(currentLevelCopy);
            });
        }
    }

    private void HandleExitClick()
    {
        if (exitGameButton != null) MainCanvasGroup.SetInputActive(false);

        OnExitButtonClicked?.Invoke();
    }

    private void HandleSettingsButton()
    {
        if (settingsButton != null) MainCanvasGroup.SetInputActive(false);

        OnSettingsButtonClicked?.Invoke();
    }
    private void OnDestroy()
    {
        if (openLevelsButton != null) openLevelsButton.onClick.RemoveAllListeners();
        if (closeLevelsButton != null) closeLevelsButton.onClick.RemoveAllListeners();
        if (openCarsButton != null) openCarsButton.onClick.RemoveAllListeners();
        if (closeCarsButton != null) closeCarsButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (exitGameButton != null) exitGameButton.onClick.RemoveListener(HandleExitClick);
        

        if (levelsContentContainer != null)
        {
            foreach (Transform child in levelsContentContainer)
            {
                if (child == null) continue;
                var button = child.GetComponent<Button>();
                if (button != null) button.onClick.RemoveAllListeners();
            }
        }

        if (_model != null)
        {
            _model.OnCarCarouselChanged -= HandleCarCarouselChanged;
        }

        if (_carCarousel_Ui != null)
        {
            _carCarousel_Ui.CleanUp();
        }
    }
}
