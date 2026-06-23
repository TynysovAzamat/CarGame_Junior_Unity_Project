using System;
using System.Collections.Generic;
using Assets.CarGame.Assets.Features.Racing.Scripts.Data;
using UnityEngine;

public class Racing_Main_Menu_Model
{
    private readonly List<RacingLevelData> availableLevels;
    private readonly List<CarConfigData> availableCars;
    private string selectedCarId;

    private int _currentCarId;
    public List<RacingLevelData> AvailableLevels => availableLevels;
    public List<CarConfigData> AvailableCars => availableCars;
    public string SelectedCarId => selectedCarId;

    public CarConfigData CurrentCar => availableCars[_currentCarId];

    public event Action<string> OnCarSelected;

    public event Action<CarConfigData, SelectionDirection> OnCarCarouselChanged;

    public enum SelectionDirection
    {
        Next, Previous
    }
    public Racing_Main_Menu_Model(List<RacingLevelData> levels, List<CarConfigData> carsDatas, string initialCarId)
    {
        availableLevels = levels ?? new List<RacingLevelData>();
        availableCars = carsDatas ?? new List<CarConfigData>();
        selectedCarId = initialCarId;

        _currentCarId = availableCars.FindIndex(c => c.CarId == selectedCarId);
        if(_currentCarId == -1) _currentCarId = 0;
    }

    public void NextCar()
    {
        if (availableCars.Count <= 1) return;
        _currentCarId = (_currentCarId + 1) % availableCars.Count;
        SelectCar(CurrentCar.CarId);
        OnCarCarouselChanged?.Invoke(CurrentCar, SelectionDirection.Next);
    }
    public void PreviousCar()
    {
        if (availableCars.Count <= 1) return;
        _currentCarId = (_currentCarId - 1 + availableCars.Count) % availableCars.Count;
        SelectCar(CurrentCar.CarId);
        OnCarCarouselChanged?.Invoke(CurrentCar, SelectionDirection.Previous);
    }

    public void SelectCar(string carId)
    {
        // выбираем машину и запускаем
        if (string.IsNullOrEmpty(carId))
        {
            Debug.LogError("Car ID cannot be null or empty.");
            return;
        }

        selectedCarId = carId;
        PlayerPrefs.SetString("Racing_SelectedCarId", carId);
        PlayerPrefs.Save();
        Debug.Log($"[Model] Выбор успешно зафиксирован в реестре ОС: {carId}");
        OnCarSelected?.Invoke(carId);
    }
}
