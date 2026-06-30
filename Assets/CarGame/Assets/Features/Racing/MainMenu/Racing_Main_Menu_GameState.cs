using System.Collections.Generic;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

public class Racing_Main_Menu_GameState : BaseMenuView, IGameState
{
    private const string CAR_PREFS_KEY = "Racing_SelectedCarId";

    private readonly IGameStateService _stateService;
    private readonly ISceneLoader _sceneLoader;

    private Racing_Main_Menu_Model _model;
    private Racing_Main_Menu_View _view;
    private SettingsWindowView _settingsWindowView;
    public Racing_Main_Menu_GameState(IGameStateService stateService, ISceneLoader sceneLoader)
    {
        _stateService = stateService;
        _sceneLoader = sceneLoader;
    }

    public void Enter()
    {
        _sceneLoader.LoadScene("Menu", () =>
        {
            // собираем данные при открытий сцены
            var levels = new List<RacingLevelData>(Resources.LoadAll<RacingLevelData>("Racing/Levels"));
            var cars = new List<CarConfigData>(Resources.LoadAll<CarConfigData>("Racing/Cars"));

            if (levels.Count == 0 || cars.Count == 0)
            {
                Debug.LogError("Racing_MainMenu_GameState: No levels or cars found in Resources.");
                return;
            }

            string savedCarId = PlayerPrefs.GetString(CAR_PREFS_KEY, cars[0].CarId);
            _model = new Racing_Main_Menu_Model(levels, cars, savedCarId);

            // создаём меню(canvas) из префабов по заданым путям 
            GameObject menuPrefabObject = Resources.Load<GameObject>("Racing/Prefabs/UI_MainMenu_Canvas");
            if (menuPrefabObject == null)
            {
                Debug.LogError("Racing_MainMenu_GameState: Failed to load UI_MainMenu_Canvas prefab object.");
                return;
            }

            GameObject spawnedMenuInstance = Object.Instantiate(menuPrefabObject);
            _view = spawnedMenuInstance.GetComponent<Racing_Main_Menu_View>();

            if (_view == null)
            {
                Debug.LogError("Racing_MainMenu_GameState: Component 'Racing_Main_Menu_View' not found on the instantiated prefab!");
                return;
            }

            GameObject settingsPrefabObject = Resources.Load<GameObject>("Shared/Prefabs/Ui_Settings_Canvas");
            if (settingsPrefabObject == null) return;

            GameObject spawnedSettingsInstance = Object.Instantiate(settingsPrefabObject);
            _settingsWindowView = spawnedSettingsInstance.GetComponent<SettingsWindowView>();

            if (_settingsWindowView != null)
            {
                _settingsWindowView.Init(null);

                _settingsWindowView.gameObject.SetActive(false);
            }

            // связываем интерфейс с игрой
            _view.Init(_model);

            if (_view != null)
            {
                _view.Init(_model);
                _view.OnExitButtonClicked += HandleExitGame;
                _view.OnSettingsButtonClicked += HandleSettingsSelected;
                _view.OnLevelButtonClicked += HandleLevelSelected;
            }

            if (_settingsWindowView != null) _settingsWindowView.OnClosePressed += HandleSettingsClosed;

            if (_model != null) _model.OnCarSelected += HandleCarSelected;

            _view.AnimateIn();

            Debug.Log("[MainMenuState] Init. state has beed completed. Menu is ready to input");
        });
        
    }

    public void Exit()
    {
        // удаляем все объекты с сцены 
        if (_view != null )
        {
            _view.OnLevelButtonClicked -= HandleLevelSelected;
            _view.OnExitButtonClicked -= HandleExitGame;
            _view.OnSettingsButtonClicked -= HandleSettingsSelected;
        }
        
        if (_model != null) _model.OnCarSelected -= HandleCarSelected;
        
        if (_settingsWindowView != null) _settingsWindowView.OnClosePressed -= HandleSettingsClosed;
        

        GameObject menuCube = GameObject.FindWithTag("MenuDecor");
        if (menuCube != null) Object.Destroy(menuCube);

        if (_view != null) Object.Destroy(_view.gameObject);
        if (_settingsWindowView != null) Object.Destroy(_settingsWindowView.gameObject);
    }

    private void HandleCarSelected(string carId)
    {
        if (string.IsNullOrEmpty(carId)) return;

        PlayerPrefs.SetString(CAR_PREFS_KEY, carId);
        PlayerPrefs.Save();
    }

    private void HandleLevelSelected(RacingLevelData selectedLevelData)
    {
        if (selectedLevelData == null)
        {
            Debug.LogError("Racing_MainMenu_GameState: Selected level data is null.");
            return;
        }

        if (_view != null) 
        {
            _view.AnimateOut(() =>
            {
                // переключаем на Gameplay и передаём управлению ему
                Debug.Log($"[GameState] Menu Animation completed. Safety run gameplay for level: {selectedLevelData.LevelName}");

                _sceneLoader.LoadScene("Gameplay", () =>
                {
                    var gameplayState = new Racing_Gameplay_GameState(_stateService, _sceneLoader, selectedLevelData, null, null);
                    _stateService.ChangeState(gameplayState);
                });
            });
        }
        else
        {
            _sceneLoader.LoadScene("Gameplay", () =>
            {
                var gameplayState = new Racing_Gameplay_GameState(_stateService, _sceneLoader, selectedLevelData, null, null);
                _stateService.ChangeState(gameplayState);
            });
        }
    }
    private void HandleExitGame()
    {
        PlayerPrefs.Save();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif  
            Application.Quit();
    }

    private void HandleSettingsSelected()
    {
        if (_settingsWindowView == null || _view == null) return;

        _view.AnimateOut(() => { _settingsWindowView.Open(); });
    }

    private void HandleSettingsClosed()
    {
        if (_settingsWindowView == null || _view == null) return;
        
        

        _settingsWindowView.Close(() => 
        {
            _settingsWindowView.gameObject.SetActive(false);

            _view.AnimateIn(()  =>
            {
                var settingButton = _view.GetComponentInChildren<UnityEngine.UI.Button>();
                _view.MainCanvasGroup.SetInputActive(true);
            });
        });
    }
}
