using UnityEngine;
using System;
public class Pause_GameState : IGameState
{
    private readonly IGameStateService _stateService;
    private readonly ISceneLoader _sceneLoader;
    private readonly Assets.CarGame.Assets.Features.Racing.Scripts.Data.RacingLevelData _currentLevelData;
    private readonly IGameState _previousGameplayState;
    
    private Pause_GameState_Model _model;
    private Pause_Menu_View _view;

    public Pause_GameState(IGameStateService stateService, ISceneLoader sceneLoader, Assets.CarGame.Assets.Features.Racing.Scripts.Data.RacingLevelData currentLevelData, IGameState previousGameplayState  )
    {
        _stateService = stateService;
        _sceneLoader = sceneLoader;
        _currentLevelData = currentLevelData;
        _previousGameplayState = previousGameplayState;
    }

    public void Enter()
    {
        Time.timeScale = 0f;
        _model = new Pause_GameState_Model();

        var prefab = Resources.Load<GameObject>("Racing/Prefabs/UI_PauseMenu_Canvas");
        if (prefab == null)
        {
            Debug.LogError("[PauseState] Не найден префаб по пути 'Resources/Racing/Prefabs/UI_PauseMenu_Canvas'!");
            return;
        }

        var instance = UnityEngine.Object.Instantiate(prefab);
        _view = instance.GetComponent<Pause_Menu_View>();

        if (_view != null)
        {
            _view.Init(_model);
            _view.Show();
        }

        _model.OnResumeRequested += HandleResume;
        _model.OnMainMenuRequested += HandleMainMenu;
    }

    public void Exit()
    {
        if (_model != null)
        {
            _model.OnResumeRequested -= HandleResume;
            _model.OnMainMenuRequested -= HandleMainMenu;
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

    private void HandleMainMenu()
    {
        Time.timeScale = 1f;
        if (_view != null)
        {
            _view.Hide(() =>
            {
                UnityEngine.Object.Destroy(_view.gameObject);

                var gameplayView = UnityEngine.Object.FindAnyObjectByType<Racing_Gameplay_View>();
                if (gameplayView != null) UnityEngine.Object.Destroy(gameplayView.gameObject);

                _sceneLoader.LoadScene("Menu", () =>
                {
                    _stateService.ChangeState(new Racing_Main_Menu_GameState(_stateService, _sceneLoader));
                });
            });
        }
    }
}

