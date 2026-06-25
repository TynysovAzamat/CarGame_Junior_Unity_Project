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
        { "CloseLevelsText", new[] { "BACK", "Назад"} },
        { "WinText", new[] { "YOU WON!", "ТЫ ПОБЕДИЛ!"} },
        { "NextLevelText", new[] { "Next level", "Слейдущий уровень"} },
        { "MainMenuText", new[] { "Main menu", "Главное меню"} },
        { "OpenPlayAgainButton", new[] { "Play again", "Играть снова"} },
        { "CarSettingsText",       new[] { "SPEED: {0}\nTURN SPEED: {1}", "Скорость: {0}\nПодвижность: {1}" } },
        { "Blue Bond",                  new[] { "Blue Bond",    "Синий Бонд" } },
        { "Red Heavy",              new[] { "Red Heavy", "Красный и тяжёлый" } },
        { "Purple Guy",                new[] { "Purple Guy",   "Фиолетовый чел" } },
        { "Map1",               new[] { "Map1", "Карта1" } },
        { "Map2",                 new[] { "Map2",   "Карта2" } },
    };

    static LocalizationManager()
    {
        CurrentLanguageId = PlayerPrefs.GetInt("SelectedLanguageIndex", 0);
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
            return translation[CurrentLanguageId];
        }
        return key;
    }
}
