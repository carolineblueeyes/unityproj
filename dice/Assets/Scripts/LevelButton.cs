// Файл: LevelButton.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image lockIcon;
    private Button button;
    private int levelIndex;
    private bool isUnlocked;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
    }

    public void Setup(int level, bool unlocked)
    {
        levelIndex = level;
        isUnlocked = unlocked;

        levelText.text = level.ToString();
        lockIcon.gameObject.SetActive(!unlocked);
        button.interactable = unlocked;
    }

    private void OnButtonClick()
    {
        if (isUnlocked)
        {
            GameManager.Instance.LoadLevel(levelIndex);
        }
    }
}
