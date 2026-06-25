using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class Racing_Win_Menu_View : MonoBehaviour
{
    // Testing for cmd git commands
    
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button nextLevelButton;

    private IGameStateService _stateService;
    private ISceneLoader _sceneLoader;
    private RacingLevelData _currentLevel;

    public void Init(IGameStateService stateService, ISceneLoader sceneLoader, RacingLevelData currentLevel)
    {
        if (stateService == null || currentLevel == null || sceneLoader == null)
        {
            Debug.LogError("Initialization parameters cannot be null.");
            return;
        }

        if (canvasGroup == null || playAgainButton == null || mainMenuButton == null)
        {
            Debug.LogError("UI components are not assigned in the inspector.");
            return;
        }

        _stateService = stateService;
        _currentLevel = currentLevel;
        _sceneLoader = sceneLoader;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f).SetUpdate(true);

        if (playAgainButton != null) playAgainButton.onClick.AddListener(HandlePlayAgain);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(HandleMainMenu);
        if (nextLevelButton != null) nextLevelButton.onClick.AddListener(HandleNextLevel);
    }

    private void HandlePlayAgain()
    {
        if (canvasGroup == null) return; 

        canvasGroup.SetInputActive(false);

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            if (_currentLevel == null) { canvasGroup.SetInputActive(true); return; }

            int currentLevelId = _currentLevel.LevelID;
            string dynamicPath = $"Racing/Levels/Map_{currentLevelId}";
            RacingLevelData freshLevelData = Resources.Load<RacingLevelData>(dynamicPath);

            if (freshLevelData == null)
            {
                Debug.LogError($"[WinMenu] Ошибка: Не удалось найти ассет по пути '{dynamicPath}'. Проверьте имя файла в папке Resources!");
                canvasGroup.SetInputActive(true);
                return;
            }

            if (_stateService == null || _sceneLoader == null) { canvasGroup.SetInputActive(true); return; }

            IGameStateService cachedStateService = _stateService;
            ISceneLoader cachedSceneLoader = _sceneLoader;

            Destroy(gameObject);

            cachedSceneLoader.LoadScene("Gameplay", () =>
            {
                if (cachedStateService == null || cachedSceneLoader == null) return;

                Debug.Log("[WinMenu Callback] Чистая сцена загружена. Создаем новый стейт гонки.");

                var freshGameplayState = new Racing_Gameplay_GameState(cachedStateService, cachedSceneLoader, freshLevelData);

                cachedStateService.ChangeState(freshGameplayState);
            });
        });
    }

    private void HandleNextLevel()
    {
        canvasGroup.SetInputActive(false);

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            int nextLevelId = _currentLevel.LevelID + 1;
            RacingLevelData nextLevelData = Resources.Load<RacingLevelData>($"Racing/Levels/Map_{nextLevelId}");

            if (nextLevelData == null)
            {
                IGameStateService menuStateService = _stateService;
                ISceneLoader menuSceneLoader = _sceneLoader;

                Destroy(gameObject);

                menuSceneLoader.LoadScene("Menu", () => {
                    menuStateService.ChangeState(new Racing_Main_Menu_GameState(menuStateService, menuSceneLoader));
                });
                return;
            }

            IGameStateService cachedStateService = _stateService;
            ISceneLoader cachedSceneLoader = _sceneLoader;

            Destroy(gameObject);

            cachedSceneLoader.LoadScene("Gameplay", () =>
            {
                Debug.Log("[WinMenu Callback] Переход на следующий уровень. Активируем GameplayState.");

                var freshGameplayState = new Racing_Gameplay_GameState(cachedStateService, cachedSceneLoader, nextLevelData);
                cachedStateService.ChangeState(freshGameplayState);
            });
        });
    }

    private void HandleMainMenu()
    {
        if (canvasGroup == null) return;

        canvasGroup.SetInputActive(false);

        canvasGroup.DOFade(0f, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            if (_stateService == null || _sceneLoader == null)
            {
                canvasGroup.SetInputActive(true);
                return;
            }

            IGameStateService cachedStateService = _stateService;
            ISceneLoader cachedSceneLoader = _sceneLoader;

            Destroy(gameObject);

            LoadMainMenu(cachedStateService, cachedSceneLoader);
        });
    }

    private void LoadMainMenu(IGameStateService stateService, ISceneLoader sceneLoader)
    {
        if (stateService == null || sceneLoader == null)
        {
            Debug.LogError("[WinMenu -> LoadMainMenu] Переданы пустые аргументы сервисов (null). Загрузка отменена.");
            return;
        }

        _sceneLoader.LoadScene("Menu", () =>
        {
            var freshMenuState = new Racing_Main_Menu_GameState(_stateService, _sceneLoader);
            _stateService.ChangeState(freshMenuState);
        });
    }
    private void OnDestroy()
    {
        if (playAgainButton != null) playAgainButton.onClick.RemoveListener(HandlePlayAgain);
        if (mainMenuButton != null) mainMenuButton.onClick.RemoveListener(HandleMainMenu);
        if (nextLevelButton != null) nextLevelButton.onClick.RemoveListener(HandleNextLevel);
    }
}
