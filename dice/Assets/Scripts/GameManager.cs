// Файл: GameManager.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData PlayerData { get; private set; }
    
    // Предположим, у вас есть 10 уровней.
    // Имена сцен должны соответствовать: "Level_1", "Level_2" и т.д.
    public int totalLevels = 10; 

    void Awake()
    {
        // Классическая реализация синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData(); // Загружаем данные при первом запуске
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadData()
    {
        PlayerData = SaveSystem.LoadPlayer();
    }

    public void SaveData()
    {
        SaveSystem.SavePlayer(PlayerData);
    }

    // Метод для завершения уровня
    public void CompleteLevel(int levelNumber)
    {
        if (levelNumber > PlayerData.lastCompletedLevel)
        {
            PlayerData.lastCompletedLevel = levelNumber;
            SaveData();
        }
    }

    // Метод для загрузки сцен
    public void LoadLevel(int levelNumber)
    {
        // Проверяем, доступен ли уровень
        if (levelNumber <= PlayerData.lastCompletedLevel + 1 && levelNumber <= totalLevels)
        {
            // Имена сцен должны быть "Level_1", "Level_2" и т.д.
            SceneManager.LoadScene("Level_" + levelNumber);
        }
        else
        {
            Debug.LogWarning($"Попытка загрузить недоступный уровень: {levelNumber}");
        }
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Убедитесь, что у вас есть сцена с таким именем
    }

    // Вызывается при закрытии приложения
    void OnApplicationQuit()
    {
        SaveData();
    }
}
