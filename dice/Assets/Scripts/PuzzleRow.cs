// Файл: PuzzleRow.cs
// Финальная, рабочая версия для иерархии с дочерним объектом 'Content'

using UnityEngine;
using UnityEngine.UI; // Для доступа к Image
using UnityEngine.EventSystems; // Для IPointerClickHandler
using System.Linq; // Для .Any() и .All()
using System; // Для Action
using System.Collections.Generic; // Для List
public class PuzzleRow : MonoBehaviour, IPointerClickHandler
{
    // Событие, на которое подпишется InputController, чтобы знать, когда кликнули по строке
    public static event Action<PuzzleRow> OnRowClicked;

    // --- Ссылки, которые нужно перетащить в инспекторе ---
    [Header("Ссылки на UI элементы")]
    [Tooltip("Объект с зеленым фоном, который появляется при завершении")]
    [SerializeField] private GameObject completedBackground;

    [Tooltip("Текстовый компонент для отображения подсказки")]
    [SerializeField] private TMPro.TextMeshProUGUI clueText;
    // ----------------------------------------------------
private List<string> correctWordParts;
    // --- Публичные свойства для доступа из других скриптов ---
    public string CorrectWord { get; private set; }
    public bool IsCompleted { get; private set; } = false;
    // ----------------------------------------------------

    /// <summary>
    /// Вызывается один раз при создании объекта.
    /// </summary>
    void Awake()
    {
        // Гарантированно выключаем фон завершения при старте
        if (completedBackground != null)
        {
            completedBackground.SetActive(false);
        }
        else
        {
            Debug.LogError($"На префабе '{this.gameObject.name}' не назначена ссылка на 'Completed Background' в инспекторе!", this.gameObject);
        }
    }

    /// <summary>
    /// Инициализирует строку данными из LevelGenerator.
    /// </summary>
    /// <param name="correctWord">Правильное слово для этой строки</param>
    /// <param name="clue">Подсказка для этой строки</param>
// Внутри класса PuzzleRow.cs

// --- ЗАМЕНИТЕ ВЕСЬ МЕТОД INITIALIZE ---
public void Initialize(string correctWord, string clue, List<string> wordParts)
{
    this.CorrectWord = correctWord;
    this.correctWordParts = wordParts; // Сохраняем части слова
    
    if (clueText != null)
    {
        clueText.text = clue;
    }
    else
    {
        Debug.LogError($"На префабе '{this.gameObject.name}' не назначена ссылка на 'Clue Text'!", this.gameObject);
    }
}


    /// <summary>
    /// Обрабатывает клик по этой строке.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Если слово уже собрано, игнорируем клики
        if (IsCompleted) return;
        
        // Сообщаем всем, кто слушает (а именно InputController), что по нам кликнули
        OnRowClicked?.Invoke(this);
    }

    /// <summary>
    /// Проверяет, правильно ли собрано слово в слотах.
    /// </summary>
    /// <returns>True, если слово собрано верно.</returns>
    public bool CheckWord()
    {
        var slots = GetComponentsInChildren<Slot>();

        // Если хоть один слот пуст, слово точно не собрано
        if (slots.Any(s => !s.IsOccupied))
        {
            return false;
        }

        // Собираем слово из плиток, которые лежат в слотах
        string currentAssembledWord = string.Concat(slots.Select(s => s.PlacedTile.Letter));

        // Сравниваем "очищенные" версии строк (без пробелов и с одинаковым регистром)
        return currentAssembledWord.Trim().ToLower() == this.CorrectWord.Trim().ToLower();
    }

    /// <summary>
    /// Отмечает строку как завершенную и визуально подсвечивает ее.
    /// </summary>
    public void SetCompleted()
    {
        IsCompleted = true;

        // Включаем зеленый фон
        if (completedBackground != null)
        {
            completedBackground.SetActive(true);
        }

        // Блокируем возможность убрать плитки из завершенной строки
        var slots = GetComponentsInChildren<Slot>();
        foreach (var slot in slots)
        {
            if (slot.IsOccupied)
            {
                // Убираем возможность нажимать на кнопки плиток
                var button = slot.PlacedTile.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = false;
                }
            }
        }
    }


    public string GetCorrectSyllableForIndex(int index)
{
    if (correctWordParts != null && index >= 0 && index < correctWordParts.Count)
    {
        return correctWordParts[index];
    }
    return null;
}
}
