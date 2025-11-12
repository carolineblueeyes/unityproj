// Файл: Slot.cs
// ПОЛНАЯ ВЕРСИЯ С СОЗДАНИЕМ ТЕКСТА ВНУТРИ

using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro; // <-- ДОБАВЬТЕ ЭТО

public class Slot : MonoBehaviour
{
    public static event Action<Slot> OnSlotClicked;

    public Tile PlacedTile { get; private set; }
    public bool IsOccupied => PlacedTile != null;

    private Button button;
    private Image image;
    private Color originalColor;
    
    // --- НОВЫЕ ПЕРЕМЕННЫЕ ---
    private TextMeshProUGUI textComponent; // Ссылка на наш созданный текст

    void Awake()
    {
        image = GetComponent<Image>();
        originalColor = image.color;
        button = gameObject.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        OnSlotClicked?.Invoke(this);
    }

    public void SetHighlight(bool isHighlighted)
    {
        image.color = isHighlighted ? Color.yellow : originalColor;
    }

    // --- ПОЛНОСТЬЮ ЗАМЕНИТЕ ЭТИ ДВА МЕТОДА ---

    /// <summary>
    /// Помещает плитку, прячет ее и создает текст внутри слота.
    /// </summary>
    public void PlaceTile(Tile tile)
    {
        if (IsOccupied) return;

        PlacedTile = tile;
        
        // 1. Делаем плитку дочерней, но невидимой.
        // Она нужна только для хранения данных.
        tile.transform.SetParent(this.transform);
        tile.gameObject.SetActive(false); // Прячем плитку

        // 2. Создаем новый игровой объект для текста
        GameObject textObject = new GameObject("Slot_Text");
        textObject.transform.SetParent(this.transform, false); // Делаем его дочерним для слота

        // 3. Добавляем и настраиваем компонент TextMeshProUGUI
        textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = tile.Letter; // Берем текст из данных плитки
        textComponent.fontSize = 48; // Подберите нужный размер шрифта
        textComponent.color = Color.black; // Подберите нужный цвет
        
        // 4. ГЛАВНАЯ ЧАСТЬ: Настройка выравнивания и якорей
        textComponent.alignment = TextAlignmentOptions.Center; // Выравнивание по центру
        textComponent.enableWordWrapping = false; // Отключаем перенос строк

        RectTransform textRect = textComponent.rectTransform;
        textRect.anchorMin = Vector2.zero; // Якорь в нижний левый угол
        textRect.anchorMax = Vector2.one;  // Якорь в правый верхний угол
        textRect.sizeDelta = Vector2.zero; // Растягиваем по всему родителю (слоту)
        textRect.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Убирает плитку из слота и уничтожает созданный текст.
    /// </summary>
    public Tile RemoveTile()
    {
        if (!IsOccupied) return null;

        // Уничтожаем текстовый объект, который мы создали
        if (textComponent != null)
        {
            Destroy(textComponent.gameObject);
        }

        Tile tileToRemove = PlacedTile;
        PlacedTile = null; 
        
        // Активируем плитку обратно, чтобы ее можно было уничтожить в InputController
        tileToRemove.gameObject.SetActive(true); 
        
        return tileToRemove;
    }
}
