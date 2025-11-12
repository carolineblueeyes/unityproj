// Файл: MainMenuController.cs

using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Transform levelButtonsContainer; // Content от ScrollView
    [SerializeField] private GameObject levelButtonPrefab;

    void Start()
    {
        PopulateLevelButtons();
        SetupContinueButton();
    }

    void PopulateLevelButtons()
    {
        // Очищаем старые кнопки
        foreach (Transform child in levelButtonsContainer)
        {
            Destroy(child.gameObject);
        }

        int totalLevels = GameManager.Instance.totalLevels;
        int lastCompleted = GameManager.Instance.PlayerData.lastCompletedLevel;

        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, levelButtonsContainer);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();

            bool isUnlocked = (i <= lastCompleted + 1);
            levelButton.Setup(i, isUnlocked);
        }
    }

    void SetupContinueButton()
    {
        int nextLevel = GameManager.Instance.PlayerData.lastCompletedLevel + 1;
        
        // Если следующий уровень существует, кнопка "Продолжить" ведет на него
        if (nextLevel <= GameManager.Instance.totalLevels)
        {
            continueButton.onClick.AddListener(() => GameManager.Instance.LoadLevel(nextLevel));
        }
        else // Если все пройдено, можно сделать кнопку неактивной или вести на последний уровень
        {
            continueButton.interactable = false;
        }
    }

    public void OnSettingsButtonPressed()
    {
        // Здесь будет логика открытия панели настроек
        Debug.Log("Кнопка настроек нажата!");
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }
}
