using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

public class Pause_GameState : BaseMenuView, IGameState 
{

    private readonly IGameStateService _stateService;
    private readonly ISceneLoader _sceneLoader;
    private readonly RacingLevelData _currentLevelData;
    private readonly IGameState _previousGameplayState;

    private Pause_GameState_Model _model;
    private Pause_Menu_View _view;
    private SettingsWindowView _settingsWindowView;
    public Pause_GameState(IGameStateService stateService, ISceneLoader sceneLoader,
     RacingLevelData currentLevelData, IGameState previousGameplayState, Pause_Menu_View view, SettingsWindowView settingsWindowView)
    {
        _stateService = stateService;
        _sceneLoader = sceneLoader;
        _currentLevelData = currentLevelData;
        _previousGameplayState = previousGameplayState;
        _view = view;
        _settingsWindowView = settingsWindowView;
    }

    public void Enter()
    {
        Time.timeScale = 0f;

        _model = new Pause_GameState_Model();

        var prefab = Resources.Load<GameObject>("Shared/Prefabs/UI_PauseMenu_Canvas");
        if (prefab == null)
        {
            Debug.LogError("[Pause State] КРИТИЧЕСКАЯ ОШИБКА: Префаб паузы не найден в Shared/Prefabs!");
            return;
        }

        var instance = UnityEngine.Object.Instantiate(prefab);
        _view = instance.GetComponent<Pause_Menu_View>();

        GameObject settingsPrefabObject = Resources.Load<GameObject>("Shared/Prefabs/Ui_Settings_Canvas");
        if (settingsPrefabObject != null)
        {
            GameObject spawnedSettingsInstance = UnityEngine.Object.Instantiate(settingsPrefabObject);
            _settingsWindowView = spawnedSettingsInstance.GetComponent<SettingsWindowView>();
        }

        if (_model != null)
        {
            _model.OnRestartRequested += HandleRestart;
            _model.OnResumeRequested += HandleResume;
            _model.OnMainMenuRequested += HandleMainMenu;
            _model.OnSettingsRequested += HandleSettingsSelected;
            
        }

        if (_view != null)
        {
            _view.Init(_model);
            _view.Show();
            Debug.Log("[Pause State] Меню паузы успешно инициализировано и показано.");
        }
        else
        {
            Debug.LogError("[Pause State] Ошибка: Компонент Pause_Menu_View не найден на префабе!");
        }

        if (_settingsWindowView != null)
        {
            _settingsWindowView.Init(null);
            _settingsWindowView.gameObject.SetActive(false);
            _settingsWindowView.OnClosePressed += HandleSettingsClosed;
        }
    }

    public void Exit()
    {
        if (_model != null)
        {
            _model.OnResumeRequested -= HandleResume;
            _model.OnMainMenuRequested -= HandleMainMenu;
            _model.OnRestartRequested -= HandleRestart;
            _model.OnSettingsRequested -= HandleSettingsSelected;
        }

        if (_settingsWindowView != null)
        {
            _settingsWindowView.OnClosePressed -= HandleSettingsClosed;
            _settingsWindowView.gameObject.SetActive(false);
        }

        if (_view != null)
        {
            _view.gameObject.SetActive(false);
        }
    }

    private void HandleResume()
    {
        if (_view != null)
        {
            _view.Hide(() =>
            {
                Time.timeScale = 1f;
                UnityEngine.Object.Destroy(_view.gameObject);
                _stateService.ChangeState(_previousGameplayState);
            });
        }
    }

    private void HandleRestart()
    {
        // ЭТОТ ЛОГ ОБЯЗАН ПОЯВИТЬСЯ СЛЕДУЮЩИМ ЗА ЖЕЛТЫМ
        Debug.Log("<color=cyan>[Pause State] МЕТОД HandleRestart УСПЕШНО СРАБОТАЛ!</color>");

        if (_view == null || _view.MainCanvasGroup == null)
        {
            Debug.LogError("[Pause State] Ошибка: _view или CanvasGroup равны null!");
            return;
        }

        if (_currentLevelData == null)
        {
            Debug.LogError("[Pause State] Ошибка: _currentLevelData равен null! См. конструктор стейта.");
            _view.MainCanvasGroup.SetInputActive(true);
            return;
        }

        int currentLevelId = _currentLevelData.LevelID;
        string dynamicPath = $"Racing/Levels/Map_{currentLevelId}";

        Debug.Log($"[Pause State] Загружаю карту из ресурсов по пути: {dynamicPath}");
        RacingLevelData freshLevelData = Resources.Load<RacingLevelData>(dynamicPath);

        if (freshLevelData == null)
        {
            Debug.LogError($"<color=red>[Pause State] ОШИБКА ПУТИ: Не удалось найти ассет по пути '{dynamicPath}'. Проверьте имя в паблике Resources!</color>");
            _view.MainCanvasGroup.SetInputActive(true);
            return;
        }

        Debug.Log("<color=green>[Pause State] Карта найдена успешно. Запускаю скрытие UI и сброс паузы...</color>");

        Time.timeScale = 1f;

        _view.MainCanvasGroup.SetInputActive(false);

        _view.Hide(() =>
        {
            // Возвращаем время в норму ДО вызова лоадера сцены

            UnityEngine.Object.Destroy(_view.gameObject);

            IGameStateService cachedStateService = _stateService;
            ISceneLoader cachedSceneLoader = _sceneLoader;

            Debug.Log("[Pause State] Время идет, запускаем чистую загрузку сцены...");
            cachedSceneLoader.LoadScene("Gameplay", () =>
            {
                if (cachedStateService == null || cachedSceneLoader == null) return;

                Debug.Log("<color=orange>[Pause State Callback] Новая сцена успешно загружена!</color>");
                var freshGameplayState = new Racing_Gameplay_GameState(cachedStateService, cachedSceneLoader, freshLevelData, null, null);
                cachedStateService.ChangeState(freshGameplayState);
            });
        });
    }

    private void HandleMainMenu()
    {
        if (_view == null) return;

        _view.MainCanvasGroup.SetInputActive(false);
        _view.Hide(() =>
        {
            if (_stateService == null || _sceneLoader == null) { _view.MainCanvasGroup.SetInputActive(true); return; }

            IGameStateService cachedStateService = _stateService;
            ISceneLoader cachedSceneLoader = _sceneLoader;

            Time.timeScale = 1f;
            UnityEngine.Object.Destroy(_view.gameObject);

            cachedSceneLoader.LoadScene("Menu", () =>
            {
                var freshMenuState = new Racing_Main_Menu_GameState(cachedStateService, cachedSceneLoader);
                cachedStateService.ChangeState(freshMenuState);
            });
        });
    }
    private void HandleSettingsSelected()
    {
        if (_settingsWindowView == null || _view == null) return;
        _settingsWindowView.Close(() =>
        {
            _settingsWindowView.gameObject.SetActive(false);
            _view.AnimateIn(() => { _view.MainCanvasGroup.SetInputActive(true); });
        });
    }

    private void HandleSettingsClosed()
    {
        if (_settingsWindowView == null || _view == null) return;
        _settingsWindowView.Close(() =>
        {
            _settingsWindowView.gameObject.SetActive(false);
            _view.AnimateIn(() => { _view.MainCanvasGroup.SetInputActive(true); });
        });
    }
} 

