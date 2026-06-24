using System;
using UnityEngine;

public class Pause_GameState_Model
{
    public event Action OnRestartRequested;
    public void RequestRestart() { Debug.Log("[Pause Model] Запрос рестарта получен от View! Вызываю событие..."); OnRestartRequested?.Invoke(); }

    public event Action OnResumeRequested;
    public event Action OnMainMenuRequested;
    public void RequestResume() => OnResumeRequested?.Invoke();
    public void RequestMainMenu() => OnMainMenuRequested?.Invoke();
    
    public void SetMasterVolume(float value) => PlayerPrefs.SetFloat("Volume_Master", value);
    public void SetMusicVolume(float value) => PlayerPrefs.SetFloat("Volume_Music", value);
    public void SetSoundsVolume(float value) => PlayerPrefs.SetFloat("Volume_Sounds", value);
    public void SetLanguageNull(int index) => Debug.Log($"Language changed to {index}");
    public void SetLanguage(int index) => LocalizationManager.SetLanguage(index);
}
