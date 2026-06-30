using System;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

public class Racing_Gameplay_Model
{
    public float CalculatedForwardSpeed { get; private set; }
    public float CalculatedTurnInput { get; private set; }
    public float BaseSpeed { get; private set; }
    public float MaxTurnSpeed { get; private set; }

    public float CurrentSpeedModifier = 1f;
    private float _currentJoystickY = 0f;

    public bool IsGameFinished { get; private set; } = false;

    public Transform PlayerTransform {  get; private set; }
    public CarController CarController { get; private set; }

    public event Action<float, float> OnMovementCalculated;
    public event Action OnRaceFinished;
    public event Action<Transform, CarController> OnPlayerSpawned;
    public event Action<float> OnSpeedChanged;
    public Racing_Gameplay_Model(CarConfigData selctedCarConfig)
    {
        if (selctedCarConfig != null)
        {
            BaseSpeed = selctedCarConfig.MaxSpeed <= 0 ? 5f : selctedCarConfig.MaxSpeed;
            MaxTurnSpeed = selctedCarConfig.MaxTurnSpeed;
        }
        else
        {
            BaseSpeed = 5f;
            MaxTurnSpeed = 150f;
        }
        
        
        CurrentSpeedModifier = 1f;
        _currentJoystickY = 0f;
    }

    public void SetPlayer(GameObject playerObject)
    {
        if (playerObject == null) return;

        PlayerTransform = playerObject.transform;
        CarController = playerObject.GetComponent<CarController>();

        OnPlayerSpawned?.Invoke(PlayerTransform, CarController);
    }

    public void UpdateSpeed(float currentSpeed)
    {
        OnSpeedChanged?.Invoke(currentSpeed);
    }

    private void RecalculateSpeed()
    {
        if (IsGameFinished)
        {
            CalculatedForwardSpeed = 0f;
            CalculatedTurnInput = 0f;
        }
        else
        {
            CalculatedForwardSpeed = _currentJoystickY * BaseSpeed * CurrentSpeedModifier;
        }

        OnMovementCalculated?.Invoke(CalculatedForwardSpeed, CalculatedTurnInput);
    }
    public void SetJoystickInput(Vector2 input)
    {
        if (IsGameFinished)
        {
            Debug.LogWarning("Cannot change Go button state when the game is finished.");
            return;
        }

        _currentJoystickY = input.y;
        CalculatedTurnInput = input.x;

        RecalculateSpeed();
    }

    public void ApplySurfaceModifier(float modifier)
    {
        if (IsGameFinished)
        {
            Debug.LogWarning("Cannot apply surface modifier when the game is finished.");
            return;
        }

        CurrentSpeedModifier = modifier;
        RecalculateSpeed();
    }
    public void CompleteRace()
    {
        if (IsGameFinished) return;

        IsGameFinished = true;
        CurrentSpeedModifier = 0f;
        CalculatedTurnInput = 0f;
        CalculatedForwardSpeed = 0f;
        _currentJoystickY = 0f;

        OnMovementCalculated?.Invoke(0f, 0f);

        OnRaceFinished?.Invoke();
    }
}
