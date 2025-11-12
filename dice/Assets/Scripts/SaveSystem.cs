// Файл: SaveSystem.cs

using UnityEngine;
using System.IO;

public static class SaveSystem
{
    // Путь к файлу сохранения. Application.persistentDataPath - специальная папка для данных на любой платформе.
    private static readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "player.json");

    public static void SavePlayer(PlayerData data)
    {
        string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
        File.WriteAllText(SAVE_PATH, json);
        Debug.Log("Данные сохранены в: " + SAVE_PATH);
    }

    public static PlayerData LoadPlayer()
    {
        if (File.Exists(SAVE_PATH))
        {
            string json = File.ReadAllText(SAVE_PATH);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);
            Debug.Log("Данные загружены.");
            return data;
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден. Создаем новые данные.");
            return new PlayerData(); // Возвращаем данные по умолчанию, если файла нет
        }
    }
}
