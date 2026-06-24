using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using TMPro;
using UnityEngine;

public class Racing_Gameplay_GameState : IGameState
{
    private const string CAR_PREFS_KEY = "Racing_SelectedCarId";

    private readonly IGameStateService _stateService;
    private readonly ISceneLoader _sceneLoader;
    private readonly RacingLevelData _levelData;

    private Racing_Gameplay_Model _model;
    private Racing_Gameplay_View _view;

    private GameObject _spawnedTrack;
    private CarController _spawnedCar;

    public Racing_Gameplay_GameState(IGameStateService stateService, ISceneLoader sceneLoader, RacingLevelData levelData)
    {
        _stateService = stateService;
        _sceneLoader = sceneLoader;
        _levelData = levelData;
    }

    public void Enter()
    {
       

        var hudPrefab = Resources.Load<Racing_Gameplay_View>("Racing/Prefabs/UI_Racing_HUD");
        if (hudPrefab == null)
        {
            Debug.LogError("[GameplayState] Ошибка ресурсов: Не найден префаб HUD по пути 'Resources/Racing/Prefabs/UI_Racing_HUD'!");
            return;
        }

        _view = Object.Instantiate(hudPrefab);
        _view.AnimateIn();

        if (_levelData == null)
        {
            Debug.LogError("[GameplayState] Критическая ошибка: В 3D стейт геймплея передан пустой уровень (null)!");
            return;
        }

        if (_levelData.TrackPrefab == null)
        {
            Debug.LogError($"[GameplayState] Ошибка данных уровня: В ScriptableObject '{_levelData.LevelName}' отсутствует префаб трассы TrackPrefab!");
            return;
        }
        _spawnedTrack = Object.Instantiate(_levelData.TrackPrefab);

        // задаём начальное положение и спавн машинки 
        Vector3 startPosition = Vector3.zero; 
        Transform startPoint = _spawnedTrack.transform.Find("StartPoint");

        if (startPoint == null)
        {
            foreach (Transform child in _spawnedTrack.GetComponentInChildren<Transform>())
            {
                if (child.CompareTag("StartPoint"))
                {
                    startPoint = child;
                    break;
                }
            }
        }

        if (startPoint != null)
        {
            startPosition = startPoint.transform.position;
            Debug.Log($"[GameplayState] Точка старта успешно найдена в 3D координатах: {startPosition}");
        }
        else
        {
            Debug.LogWarning("[GameplayState] Предупреждение: Объект с тегом 'StartPoint' не найден на заспавненной трассе. Спавним в (0,0,0).");
        }
        
        string savedCarId = PlayerPrefs.GetString(CAR_PREFS_KEY, "CarPlayer");
        Debug.Log($"[GameplayState] Чтение памяти: Игрок выбрал машину с ID '{savedCarId}'. Загружаем ассет...");
        string carPath = $"Racing/Cars/{savedCarId}";
        var carConfig = Resources.Load<CarConfigData>(carPath);

        if (carConfig == null)
        {
            Debug.LogError($"[GameplayState] Ошибка ресурсов: Не найден ScriptableObject машины по пути 'Resources/Racing/Cars/{savedCarId}'!");
            return;
        }

        _model = new Racing_Gameplay_Model(carConfig);

        if (carConfig.CarPrefab == null)
        {
            Debug.LogError($"[GameplayState] Ошибка данных машины: В ScriptableObject '{carConfig.CarName}' пустует поле CarPrefab!");
            return;
        }

        GameObject spawnedCarObject = Object.Instantiate(carConfig.CarPrefab, startPosition, Quaternion.identity);
        _spawnedCar = spawnedCarObject.GetComponent<CarController>();

        if (_spawnedCar == null)
        {
            Debug.LogError("[GameplayState] Критическая ошибка: На заспавненном префабе машины отсутствует компонент CarController!");
            Object.Destroy(spawnedCarObject); 
            return;
        }

        _spawnedCar.InjectModel(_model);
        _view.OnJoystickInputChanged += HandleJoystickInputChanged;
        _model.OnMovementCalculated += HandleMovementCalculated;
        _model.OnRaceFinished += HandleRaceFinished;
        _view.OnPauseClicked += HandlePauseClicked;

        _spawnedCar.SetPhysicsSpeed(_model.BaseSpeed, 0f);
    }

    public void Exit()
    {
        // проверяем класс, чтобы он не был пустым
        // и удаляем с сцены
        if (_view != null) _view.OnJoystickInputChanged -= HandleJoystickInputChanged;
        if (_model != null) _model.OnMovementCalculated -= HandleMovementCalculated;
        if (_model != null) _model.OnRaceFinished -= HandleRaceFinished;
        if (_view != null) _view.OnPauseClicked -= HandlePauseClicked;

        if (_view != null) Object.Destroy(_view.gameObject);
        if (_spawnedCar != null) Object.Destroy(_spawnedCar.gameObject);
        if (_spawnedTrack != null) Object.Destroy(_spawnedTrack.gameObject);
    }

    private void HandlePauseClicked()
    {
        _stateService.ChangeState(new Pause_GameState(_stateService, _sceneLoader, _levelData, this));
    }
    private void HandleJoystickInputChanged(Vector2 joystickVector)
    {
        _model?.SetJoystickInput(joystickVector);
    }

    private void HandleMovementCalculated(float newSpeed, float newTurnInput)
    {
        _spawnedCar?.SetPhysicsSpeed(newSpeed, newTurnInput);
    }

    private void HandleRaceFinished()
    {
         
        if (_view != null)
        {
            _view.AnimateOut(() => SpawnWinMenu3D());
        }
        else 
        { 
            SpawnWinMenu3D(); }
    }

    private void SpawnWinMenu3D()
    {
        GameObject winPrefab = Resources.Load<GameObject>("Racing/Prefabs/UI_Win_Menu");
        if (winPrefab == null)
        {
            return;
        }

        GameObject winInstantiate = Object.Instantiate(winPrefab);

        Racing_Win_Menu_View winMenuScript = winInstantiate.GetComponent<Racing_Win_Menu_View>();

        if (winMenuScript == null)
        {
            Object.Destroy(winInstantiate);
            return;
        }

        winMenuScript.Init(_stateService, _sceneLoader, _levelData);
    }
}
