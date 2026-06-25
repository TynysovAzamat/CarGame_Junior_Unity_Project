using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameBootstrapper : MonoBehaviour, IGameStateService, ISceneLoader
{
    private IGameState _currentState;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        InitializeDefaultAudioSettings();
    }
    private void Start()
    {
        //создаём начальное состояние игры
        ChangeState(new Racing_Main_Menu_GameState(this, this));
    }
    public void ChangeState(IGameState newState)
    {
        // меняем сцену в зависимости от состояния
        // и заходим в неё
        if (newState == null) return;

        
        _currentState?.Exit();
        

        _currentState = newState;
        _currentState.Enter();
    }

    public void LoadScene(string sceneName, Action onSceneLoaded = null)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Check if the scene is added to the build settings.");
            return;
        }

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            // выходим если текущая сцена совподает с сценой префаба
            Debug.Log($"Scene '{sceneName}' is already loaded.");
            onSceneLoaded?.Invoke();
            return;
        }

        var asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        if (asyncOp == null) return;

        // создаём загружаем сцену и завершаем операцию
        asyncOp.completed += (op) =>
        {
            Debug.Log($"Scene '{sceneName}' loaded successfully.");
            
            Resources.UnloadUnusedAssets();
            
            onSceneLoaded?.Invoke();
        };
    }

    private void InitializeDefaultAudioSettings()
    {
        if (!PlayerPrefs.HasKey("Volume_Master")) PlayerPrefs.SetFloat("Volume_Master", 1f);
        if (!PlayerPrefs.HasKey("Volume_Music")) PlayerPrefs.SetFloat("Volume_Music", 0.5f);
        if (!PlayerPrefs.HasKey("Volume_Sounds")) PlayerPrefs.SetFloat("Volume_Sounds", 0.5f);

        PlayerPrefs.Save();
    }
}
