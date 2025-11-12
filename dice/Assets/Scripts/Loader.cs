// Файл: Loader.cs

using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    // Префабы менеджеров, которые нужно создать
    [SerializeField] private GameObject gameManagerPrefab;
    [SerializeField] private GameObject audioManagerPrefab;

    void Start()
    {
        // Создаем GameManager, если его еще нет в сцене
        if (GameManager.Instance == null)
        {
            Instantiate(gameManagerPrefab);
        }
        
        // Создаем AudioManager, если его еще нет
        if (AudioManager.Instance == null)
        {
            Instantiate(audioManagerPrefab);
        }

        // После инициализации всех систем переходим в главное меню
        // Добавляем небольшую задержку, чтобы все успело инициализироваться
        Invoke(nameof(LoadMainMenu), 0.5f); 
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
