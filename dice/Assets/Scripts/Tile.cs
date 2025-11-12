// Файл: Tile.cs (ИСПРАВЛЕННАЯ ВЕРСИЯ)

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Tile : MonoBehaviour
{
    public static event Action<Tile, bool> OnTileClicked;

    public string Letter { get; private set; }
    // public Tile OriginalTileInPool { get; set; } // --- УДАЛИТЕ ИЛИ ЗАКОММЕНТИРУЙТЕ ЭТУ СТРОКУ ---
    public bool IsCopy { get; private set; } = false;

    private Button button;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // --- НОВЫЙ МЕТОД ---
    // Регистрируем плитку в менеджере, когда она появляется на сцене
    void Start()
    {
        if (!IsCopy && TilePoolManager.Instance != null)
        {
            TilePoolManager.Instance.RegisterTile(this);
        }
    }

    // --- НОВЫЙ МЕТОД ---
    // Удаляем плитку из менеджера перед ее уничтожением
    void OnDestroy()
    {
        if (!IsCopy && TilePoolManager.Instance != null)
        {
            TilePoolManager.Instance.UnregisterTile(this);
        }
    }

    public void Initialize(string letter)
    {
        Letter = letter;
        GetComponentInChildren<TextMeshProUGUI>().text = letter;
    }

    private void HandleClick()
    {
        OnTileClicked?.Invoke(this, this.IsCopy);
    }

    public void SetState(bool isInteractable)
    {
        canvasGroup.alpha = isInteractable ? 1.0f : 0.5f;
        canvasGroup.interactable = isInteractable;
    }

    // --- НОВЫЙ МЕТОД ---
    public bool IsInteractable()
    {
        return canvasGroup.interactable;
    }

    public Tile CreateCopy()
    {
        GameObject instance = Instantiate(this.gameObject);
        Tile copyTile = instance.GetComponent<Tile>();
        
        copyTile.Initialize(this.Letter);
        copyTile.IsCopy = true;
        // copyTile.OriginalTileInPool = this; // --- УДАЛИТЕ ИЛИ ЗАКОММЕНТИРУЙТЕ ЭТУ СТРОКУ ---
        return copyTile;
    }
}
