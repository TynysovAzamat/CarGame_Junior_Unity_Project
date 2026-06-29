using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationManager
{
    public static event Action OnLanguageChanged;

    public static int CurrentLanguageId { get; private set; } = 0;

    private static readonly Dictionary<string, string[]> LocalizationTable = new Dictionary<string, string[]>()
    {
        { "TitleGameText", new[] { "Car Racing", "Гонки на машинках"} },
        { "CarsText", new[] { "Vehicles", "Гараж"} },
        { "NewGameText", new[] { "New Game", "Новая Игра"} },
        { "ExitGameText", new[] { "Exit Game", "Выйти из игры"} },
        { "SettingsText", new[] { "Settings", "Настройки"} },
        { "BackText", new[] { "BACK", "Назад"} },
        { "WinText", new[] { "YOU WON!", "ТЫ ПОБЕДИЛ!"} },
        { "NextLevelText", new[] { "Next level", "Слейдующий уровень"} },
        { "MainMenuText", new[] { "Main menu", "Главное меню"} },
        { "PlayAgainText", new[] { "Play again", "Играть снова"} },
        { "PauseText", new[] { "Pause", "Пауза"} },
        { "AudioText", new[] { "Audio", "Звуки"} },
        { "LanguageText", new[] { "Language", "Язык"} },
        { "ResumeText", new[] { "Resume", "Продолжить"} },
        { "CarStatsText", new[] { "SPEED: {0}\nTURN SPEED: {1}", "Скорость: {0}\nПодвижность: {1}" } },
        { "Blue Bond", new[] { "Blue Bond",    "Синий Бонд" } },
        { "Red Heavy", new[] { "Red Heavy", "Красный и тяжёлый" } },
        { "Purple Guy", new[] { "Purple Guy",   "Фиолетовый чел" } },
        { "Map1", new[] { "Map1", "Карта1" } },
        { "Map2", new[] { "Map2",   "Карта2" } },
    };
    static LocalizationManager()
    {
        CurrentLanguageId = PlayerPrefs.GetInt("SelectedLanguageIndex", 0);

        if (CurrentLanguageId < 0 || CurrentLanguageId > 1)
        {
            CurrentLanguageId = 0;
            PlayerPrefs.SetInt("SelectedLanguageIndex", 0);
            PlayerPrefs.Save();
        }
    }

    public static void SetLanguage(int index)
    {
        CurrentLanguageId = index;
        PlayerPrefs.SetInt("SelectedLanguageIndex", index);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public static string GetTranslation(string key)
    {

        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (LocalizationTable.TryGetValue(key, out var translation))
        {
            if (CurrentLanguageId >= 0 && CurrentLanguageId < translation.Length)
            {
                return translation[CurrentLanguageId];
            }
            Debug.LogWarning($"[Localization] Language ID {CurrentLanguageId} is out of bounds for key '{key}'. Falling back to default (0).");

            if (translation.Length > 0)
                return translation[0];
        }
        return key;
    }

    
}
