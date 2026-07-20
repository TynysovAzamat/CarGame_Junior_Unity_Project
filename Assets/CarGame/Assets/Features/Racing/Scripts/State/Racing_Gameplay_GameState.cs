using System;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Racing_Gameplay_GameState : IGameState
{
    private const string CAR_PREFS_KEY = "Racing_SelectedCarId";

    private readonly IGameStateService _stateService;
    private readonly ISceneLoader _sceneLoader;
    private readonly RacingLevelData _levelData;

    private Pause_GameState_Model _pauseModel;
    private Pause_Menu_View _pauseView;

    private Racing_Gameplay_Model _model;
    private Racing_Gameplay_View _view;

    private GameObject _spawnedTrack;
    private CarController _controller;
    private SettingsWindowView _settingsView;

    public Racing_Gameplay_GameState (IGameStateService stateService, ISceneLoader sceneLoader, RacingLevelData levelData)
    {
        _stateService = stateService;
        _sceneLoader = sceneLoader;
        _levelData = levelData;
    }

    public void Enter ()
    {
        AudioEvents.RequestMapMusic (_levelData.LevelMusic);

        if (_view != null)
        {
            _view.SetInputActive (true);
        }

        var hudPrefab = Resources.Load<Racing_Gameplay_View> ("Racing/Prefabs/UI_Racing_HUD");
        if (hudPrefab == null)
        {
            Debug.LogError ("[GameplayState] Ошибка ресурсов: Не найден префаб HUD по пути 'Resources/Racing/Prefabs/UI_Racing_HUD'!");
            return;
        }

        _view = UnityEngine.Object.Instantiate (hudPrefab);
        _view.AnimateIn ();
        _view.SetInputActive (true);

        if (_levelData == null)
        {
            Debug.LogError ("[GameplayState] Критическая ошибка: В 3D стейт геймплея передан пустой уровень (null)!");
            return;
        }

        if (_levelData.TrackPrefab == null)
        {
            Debug.LogError ($"[GameplayState] Ошибка данных уровня: В ScriptableObject '{_levelData.LevelName}' отсутствует префаб трассы TrackPrefab!");
            return;
        }

        _spawnedTrack = UnityEngine.Object.Instantiate (_levelData.TrackPrefab);

        Vector3 startPosition = Vector3.zero;
        Transform startPoint = _spawnedTrack.transform.Find ("StartPoint");

        if (startPoint == null)
        {
            foreach (Transform child in _spawnedTrack.GetComponentInChildren<Transform> ())
            {
                if (child.CompareTag ("StartPoint"))
                {
                    startPoint = child;
                    break;
                }
            }
        }

        if (startPoint != null)
        {
            startPosition = startPoint.transform.position;
            Debug.Log ($"[GameplayState] Точка старта успешно найдена в 3D координатах: {startPosition}");
        }
        else
        {
            Debug.LogWarning ("[GameplayState] Предупреждение: Объект с тегом 'StartPoint' не найден на заспавненной трассе. Спавним в (0,0,0).");
        }

        string savedCarId = PlayerPrefs.GetString (CAR_PREFS_KEY, "CarPlayer");
        Debug.Log ($"[GameplayState] Чтение памяти: Игрок выбрал машину с ID '{savedCarId}'. Загружаем ассет...");
        string carPath = $"Racing/Cars/{savedCarId}";
        var carConfig = Resources.Load<CarConfigData> (carPath);

        if (carConfig == null)
        {
            Debug.LogError ($"[GameplayState] Ошибка ресурсов: Не найден ScriptableObject машины по пути 'Resources/Racing/Cars/{savedCarId}'!");
            return;
        }

        _model = new Racing_Gameplay_Model (carConfig);

        if (carConfig.CarPrefab == null)
        {
            Debug.LogError ($"[GameplayState] Ошибка данных машины: В ScriptableObject '{carConfig.CarName}' пустует поле CarPrefab!");
            return;
        }

        _controller = UnityEngine.Object.Instantiate<CarController>(carConfig.CarPrefab, startPosition, Quaternion.identity);
        if (_controller == null)
        {
            Debug.LogError ("[GameplayState] Критическая ошибка: На заспавненном префабе машины отсутствует компонент CarController!");
            return;
        }

        var settingsPrefab = Resources.Load<SettingsWindowView> ("Shared/Prefabs/UI_Settings_Canvas");
        if (settingsPrefab != null)
        {
            _settingsView = UnityEngine.Object.Instantiate (settingsPrefab);
            _settingsView.gameObject.SetActive (false);
        }
        else
        {
            Debug.LogError ("[GameplayState] Ошибка: Не найден префаб UI_Settings_Canvas в Resources!");
        }

        if (InputSystem.actions != null)
        {
            InputSystem.actions.FindActionMap ("UI")?.Disable ();
            InputSystem.actions.FindActionMap ("Player")?.Enable ();
        }

        _controller.InitializeStats (carConfig);
        _controller.InjectModel (_model);

        if (_view != null)
        {
            _view.OnInputVectorChanged -= HandleInputVectorChanged;
            _view.OnInputVectorChanged += HandleInputVectorChanged;
            _view.OnPauseClicked += HandlePauseClicked;
        }

        if (_model != null)
        {
            _model.OnPlayerSpawned += HandlePlayerSpawned;
            _model.OnMovementCalculated += HandleMovementCalculated;
            _model.OnRaceFinished += HandleRaceFinished;
        } 

        _controller.SetPhysicsSpeed (_model.BaseSpeed, 0f);
    }

    public void Exit ()
    {
        if (_view != null)
        {
            _view.OnInputVectorChanged -= HandleInputVectorChanged;
            _view.OnPauseClicked -= HandlePauseClicked;
            UnityEngine.Object.Destroy (_view.gameObject);
        }

        if (_model != null)
        {
            _model.OnPlayerSpawned -= HandlePlayerSpawned;
            _model.OnMovementCalculated -= HandleMovementCalculated;
            _model.OnRaceFinished -= HandleRaceFinished;
        }

        if (_controller != null) UnityEngine.Object.Destroy (_controller.gameObject);
        if (_spawnedTrack != null) UnityEngine.Object.Destroy (_spawnedTrack.gameObject);
        if (_settingsView != null) UnityEngine.Object.Destroy (_settingsView.gameObject);
    }

    private void HandlePauseClicked ()
    {
        Time.timeScale = 0f;
        _view.ClearAllInputs ();
        _view.SetInputActive (false);

        if (InputSystem.actions != null)
        {
            InputSystem.actions.FindActionMap ("Player")?.Disable ();
            InputSystem.actions.FindActionMap ("UI")?.Enable ();
        }

        _pauseModel = new Pause_GameState_Model ();
        var prefab = Resources.Load<Pause_Menu_View> ("Shared/Prefabs/UI_PauseMenu_Canvas");
        _pauseView = UnityEngine.Object.Instantiate (prefab);

        _pauseView.Init (_pauseModel);
        _pauseView.Show ();

        if (_settingsView != null)
        {
            _settingsView.Init (null);
            _settingsView.OnClosePressed += HandleSettingsClosed;
        }

        _pauseModel.OnResumeRequested += ResumeGame;
        _pauseModel.OnMainMenuRequested += GoToMainMenu;
        _pauseModel.OnRestartRequested += RestartGame;
        _pauseModel.OnSettingsRequested += HandleSettingsSelected;
    }

    private void HandleSettingsSelected ()
    {
        if (_settingsView == null || _pauseView == null) return;
        _pauseView.Hide (() => { _settingsView.Open (); });
    }

    private void HandleSettingsClosed ()
    {
        if (_settingsView == null || _pauseView == null) return;

        _settingsView.Close (() =>
        {
            _settingsView.gameObject.SetActive (false);
            _pauseView.gameObject.SetActive (true);
            _pauseView.MainCanvasGroup.SetInputActive (false);
            _pauseView.AnimateIn (() =>
            {
                _pauseView.MainCanvasGroup.SetInputActive (true);
            });
        });
    }

    private void ResumeGame ()
    {
        UnsubscribePauseEvents ();
        if (_pauseView != null)
        {
            _pauseView.Hide (() =>
            {
                UnityEngine.Object.Destroy (_pauseView.gameObject);
                Time.timeScale = 1f;
                _view.SetInputActive (true);
            });
        }
    }

    private void GoToMainMenu ()
    {
        UnsubscribePauseEvents ();
        Time.timeScale = 1f;
        if (_pauseView != null) UnityEngine.Object.Destroy (_pauseView.gameObject);
        _stateService.ChangeState (new Main_Menu_GameState (_stateService, _sceneLoader));
    }

    public void OnMoveInputReceived (InputAction.CallbackContext context)
    {
        var moveCmd = new MoveCommand (_controller.SetVericalInput, context);
        moveCmd.Execute ();
    }

    public void OnTurnInputReceived (InputAction.CallbackContext context)
    {
        var turnCmd = new TurnCommand (_controller.SetHorizontalInput, context);
        turnCmd.Execute ();
    }

    private void RestartGame ()
    {
        UnsubscribePauseEvents ();
        Time.timeScale = 1f;
        if (_pauseView != null) UnityEngine.Object.Destroy (_pauseView.gameObject);
        _stateService.ChangeState (new Racing_Gameplay_GameState (_stateService, _sceneLoader, _levelData));
    }

    private void UnsubscribePauseEvents ()
    {
        if (_pauseModel == null) return;
        _pauseModel.OnResumeRequested -= ResumeGame;
        _pauseModel.OnMainMenuRequested -= GoToMainMenu;
        _pauseModel.OnRestartRequested -= RestartGame;
        _pauseModel.OnSettingsRequested -= HandleSettingsSelected;
        if (_settingsView != null) _settingsView.OnClosePressed -= HandleSettingsClosed;
    }

    private void HandleInputVectorChanged (Vector2 inputVector)
    {
        Debug.Log ($"[State] Получен вектор от UI: {inputVector}. Ссылка на контроллер: {(_controller != null ? "ОК" : "NULL")}");
        if (_controller == null || _model == null) return;
        _model.SetButtonsInput(inputVector);
        float targetSpeed = 0f;

        if (inputVector.y > 0.05f)
        {
            targetSpeed = _controller.CurrentSpeed;
        }
        else if (inputVector.y < -0.05f)
        {
            targetSpeed = -_controller.CurrentSpeed;
        }

        float targetTurn = 0f;

        if (inputVector.x > 0.05f)
        {
            targetTurn = inputVector.x;
        }
        else if (inputVector.x < -0.05f)
        {
            targetTurn = inputVector.x;
        }

        HandleMovementCalculated(targetSpeed, targetTurn);
    }

    private void HandleMovementCalculated (float newSpeed, float newTurnInput)
    {
        if (_controller != null) _controller.SetPhysicsSpeed (newSpeed, newTurnInput);
    }

    private void HandleRaceFinished ()
    {
        if (_view != null)
        {
            _view.ClearAllInputs ();
            _view.SetInputActive (false);
        }

        if (InputSystem.actions != null)
        {
            InputSystem.actions.FindActionMap ("Player")?.Disable ();
            InputSystem.actions.FindActionMap ("UI")?.Enable ();
        }

        var winMenuPrefab = Resources.Load<Racing_Win_Menu_View> ("Racing/Prefabs/UI_Win_Menu");
        if (winMenuPrefab == null)
        {
            Debug.LogError ("(GameplayState) Не найден префаб UI_Win_Menu в Resources!");
            return;
        }
        
        var winMenuView = UnityEngine.Object.Instantiate (winMenuPrefab);
        winMenuView.Init ();

        
        winMenuView.OnPlayAgainRequested += () =>
        {
            winMenuView.CloseAnimate (() =>
            {
                _sceneLoader.LoadScene ("Gameplay", () =>
                {
                    _stateService.ChangeState (new Racing_Gameplay_GameState (_stateService, _sceneLoader, _levelData));
                });
            });
        };
        winMenuView.OnNextLevelRequested += () =>
        {
            winMenuView.CloseAnimate (() =>
            {
                int nextLevelId = _levelData.LevelID + 1;
                var nextLevelData = Resources.Load<RacingLevelData> ($"Racing/Levels/Map_{nextLevelId}");
                if (nextLevelData == null)
                {
                    _sceneLoader.LoadScene ("Menu", () =>
                    {
                        _stateService.ChangeState (new Main_Menu_GameState (_stateService, _sceneLoader));
                    });
                    return;
                }
                _sceneLoader.LoadScene ("Gameplay", () =>
                {
                    _stateService.ChangeState (new Racing_Gameplay_GameState (_stateService, _sceneLoader, nextLevelData));
                });
            });
        };
        winMenuView.OnMainMenuRequested += () =>
        {
            winMenuView.CloseAnimate (() =>
            {
                _sceneLoader.LoadScene ("Menu", () =>
                {
                    _stateService.ChangeState (new Main_Menu_GameState (_stateService, _sceneLoader));
                });
            });
        };
    }
    private void HandlePlayerSpawned(Transform playerTransform, CarController carController)
    {
        _view.CameraFollow.Init(playerTransform, carController);
    }
}
