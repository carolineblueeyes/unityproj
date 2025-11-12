// Файл: UIButtonsController.cs (ФИНАЛЬНАЯ ВЕРСИЯ С УЛУЧШЕННОЙ АНИМАЦИЕЙ)

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIButtonsController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Transform tilePoolContainer;
    [SerializeField] private InputController inputController;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private VerticalLayoutGroup parentLayoutGroup;

    private SymmetricalPlaceholderGrid placeholderGrid;
    private GridLayoutGroup gridLayout;
    private CanvasGroup tilePoolCanvasGroup;
    private bool isAnimating = false;

    void Start()
    {
        if (tilePoolContainer != null)
        {
            placeholderGrid = tilePoolContainer.GetComponent<SymmetricalPlaceholderGrid>();
            gridLayout = tilePoolContainer.GetComponent<GridLayoutGroup>();
            tilePoolCanvasGroup = tilePoolContainer.GetComponent<CanvasGroup>();
        }
        if (inputController == null) inputController = FindObjectOfType<InputController>();

        if (placeholderGrid == null) Debug.LogError("На 'tilePoolContainer' не найден SymmetricalPlaceholderGrid!", this);
        if (gridLayout == null) Debug.LogError("На 'tilePoolContainer' не найден GridLayoutGroup!", this);
        if (tilePoolCanvasGroup == null) Debug.LogError("На 'tilePoolContainer' не найден CanvasGroup! Пожалуйста, добавьте его.", this);
        if (inputController == null) Debug.LogError("На сцене не найден InputController!", this);
        if (tilePrefab == null) Debug.LogError("В UIButtonsController не назначен префаб плитки!", this);
        if (parentLayoutGroup == null) Debug.LogError("В UIButtonsController не назначен родительский Layout Group!", this);
    }

    public void ShuffleTiles()
    {
        if (isAnimating) return;
        List<TileState> tileStates = GetCurrentTileStates();
        var shuffledStates = tileStates.OrderBy(a => Random.value).ToList();
        StartCoroutine(AnimateAndRecreateRoutine(shuffledStates));
    }

    public void SortTilesAlphabetically()
    {
        if (isAnimating) return;
        List<TileState> tileStates = GetCurrentTileStates();
        var sortedStates = tileStates.OrderBy(s => s.Letter).ToList();
        StartCoroutine(AnimateAndRecreateRoutine(sortedStates));
    }

    private List<TileState> GetCurrentTileStates()
    {
        List<TileState> states = new List<TileState>();
        foreach (Tile tile in tilePoolContainer.GetComponentsInChildren<Tile>())
        {
            bool isInteractable = tile.IsInteractable();
            states.Add(new TileState(tile.Letter, isInteractable));
        }
        return states;
    }

    private IEnumerator AnimateAndRecreateRoutine(List<TileState> states)
    {
        isAnimating = true;

        // 1. ПОДГОТОВКА
        // Отключаем родительский layout, чтобы он не влиял на размер во время анимации
        if (parentLayoutGroup != null) parentLayoutGroup.enabled = false;
        if (placeholderGrid != null) placeholderGrid.enabled = false;

        List<Transform> tilesToAnimate = new List<Transform>();
        foreach (Transform child in tilePoolContainer)
        {
            if (child.GetComponent<Tile>() != null)
            {
                tilesToAnimate.Add(child);
            }
        }

        // 2. АНИМАЦИЯ ПЕРЕВОРОТА
        float flipDuration = 0.4f; // Сделаем чуть быстрее для эффекта "вжух"
        float halfFlipTime = flipDuration / 2f;

        // --- Фаза 1: Переворот "от нас" с ускорением ---
        for (float t = 0; t < halfFlipTime; t += Time.deltaTime)
        {
            float progress = t / halfFlipTime;
            // --- ИЗМЕНЕНИЕ: Добавляем ускорение (Ease-In) ---
            float easedProgress = progress * progress; // Квадратичное ускорение

            foreach (Transform tile in tilesToAnimate)
            {
                tile.localScale = new Vector3(1 - easedProgress, 1, 1);
            }
            yield return null;
        }

        // --- Фаза 2: "Магия подмены" в невидимом контейнере ---
        if (tilePoolCanvasGroup != null) tilePoolCanvasGroup.alpha = 0;

        foreach (Transform child in tilePoolContainer)
        {
            Destroy(child.gameObject);
        }
        yield return null;

        foreach (TileState state in states)
        {
            GameObject tileInstance = Instantiate(tilePrefab, tilePoolContainer);
            Tile tileComponent = tileInstance.GetComponent<Tile>();
            tileComponent.Initialize(state.Letter);
            tileComponent.SetState(state.IsInteractable);
        }

        ForceRecalculateContainerSize(states.Count);
        
        // --- ИЗМЕНЕНИЕ: Включаем родительский layout ДО того, как контейнер станет видимым ---
        // Это уберет финальный "прыжок"
        if (parentLayoutGroup != null) parentLayoutGroup.enabled = true;
        
        yield return new WaitForEndOfFrame();

        // --- Фаза 3: Появление и обратный переворот ---
        if (tilePoolCanvasGroup != null) tilePoolCanvasGroup.alpha = 1;

        List<Transform> newTiles = new List<Transform>();
        foreach (Transform child in tilePoolContainer)
        {
            if (child.GetComponent<Tile>() != null)
            {
                child.localScale = new Vector3(0, 1, 1);
                newTiles.Add(child);
            }
        }

        // Анимируем переворот "к нам" с замедлением в конце
        for (float t = 0; t < halfFlipTime; t += Time.deltaTime)
        {
            float progress = t / halfFlipTime;
            // --- ИЗМЕНЕНИЕ: Добавляем замедление (Ease-Out) ---
            float easedProgress = 1 - (1 - progress) * (1 - progress);

            foreach (Transform tile in newTiles)
            {
                if (tile != null) tile.localScale = new Vector3(easedProgress, 1, 1);
            }
            yield return null;
        }

        // Устанавливаем финальный масштаб
        foreach (Transform tile in newTiles)
        {
            if (tile != null) tile.localScale = Vector3.one;
        }

        // 3. ФИНАЛИЗАЦИЯ
        // Включаем только наш скрипт центрирования, родительский уже включен
        if (placeholderGrid != null) placeholderGrid.enabled = true;

        isAnimating = false;
    }

    // Остальные методы без изменений
    private void ForceRecalculateContainerSize(int itemCount)
    {
        if (gridLayout == null || itemCount == 0) return;
        RectTransform containerRect = tilePoolContainer.GetComponent<RectTransform>();
        float containerWidth = containerRect.rect.width;
        int columns = Mathf.FloorToInt((containerWidth - gridLayout.padding.left - gridLayout.padding.right + gridLayout.spacing.x) / (gridLayout.cellSize.x + gridLayout.spacing.x));
        if (columns <= 0) columns = 1;
        int rows = Mathf.CeilToInt((float)itemCount / columns);
        float newHeight = gridLayout.padding.top + gridLayout.padding.bottom + (rows * gridLayout.cellSize.y) + ((rows - 1) * gridLayout.spacing.y);
        containerRect.sizeDelta = new Vector2(containerRect.sizeDelta.x, newHeight);
    }

    // public void OnHintButtonPressed()
    // {
    //     if (isAnimating || inputController == null) return;
    //     var allEmptySlots = FindObjectsOfType<PuzzleRow>().Where(row => !row.IsCompleted).SelectMany(row => row.GetComponentsInChildren<Slot>()).Where(slot => !slot.IsOccupied).ToList();
    //     if (!allEmptySlots.Any()) { Debug.Log("Нет доступных слотов для подсказки."); return; }
    //     allEmptySlots = allEmptySlots.OrderBy(s => Random.value).ToList();
    //     foreach (var randomSlot in allEmptySlots)
    //     {
    //         PuzzleRow parentRow = randomSlot.GetComponentInParent<PuzzleRow>();
    //         var allSlotsInRow = parentRow.GetComponentsInChildren<Slot>();
    //         int slotIndex = System.Array.IndexOf(allSlotsInRow, randomSlot);
    //         if (slotIndex == -1) continue;
    //         string correctSyllable = parentRow.GetCorrectSyllableForIndex(slotIndex);
    //         if (string.IsNullOrEmpty(correctSyllable)) continue;
    //         Tile tileInPool = FindTileInPool(correctSyllable);
    //         if (tileInPool != null)
    //         {
    //             randomSlot.SendMessage("HandleClick", SendMessageOptions.DontRequireReceiver);
    //             tileInPool.SendMessage("HandleClick", SendMessageOptions.DontRequireReceiver);
    //             Debug.Log($"Подсказка: слог '{correctSyllable}' установлен.");
    //             return;
    //         }
    //     }
    //     Debug.Log("Не удалось найти подходящую плитку в пуле для доступных слотов.");
    // }
// В файле UIButtonsController.cs

    /// <summary>
    /// Логика для кнопки "Подсказка".
    /// </summary>
    public void OnHintButtonPressed()
    {
        if (isAnimating || inputController == null) return;

        // 1. Находим все возможные пустые слоты
        var allEmptySlots = FindObjectsOfType<PuzzleRow>()
            .Where(row => !row.IsCompleted)
            .SelectMany(row => row.GetComponentsInChildren<Slot>())
            .Where(slot => !slot.IsOccupied)
            .ToList();

        if (!allEmptySlots.Any())
        {
            Debug.Log("Нет доступных слотов для подсказки.");
            return;
        }

        // 2. Перемешиваем их, чтобы подсказка была случайной
        allEmptySlots = allEmptySlots.OrderBy(s => Random.value).ToList();

        // 3. Ищем первую валидную комбинацию "слот + плитка"
        foreach (var randomSlot in allEmptySlots)
        {
            PuzzleRow parentRow = randomSlot.GetComponentInParent<PuzzleRow>();
            var allSlotsInRow = parentRow.GetComponentsInChildren<Slot>();
            int slotIndex = System.Array.IndexOf(allSlotsInRow, randomSlot);

            if (slotIndex == -1) continue;

            string correctSyllable = parentRow.GetCorrectSyllableForIndex(slotIndex);
            if (string.IsNullOrEmpty(correctSyllable)) continue;

            // Ищем подходящую плитку в пуле
            Tile tileInPool = FindTileInPool(correctSyllable);

            // 4. Если все найдено, вызываем новый "тихий" метод
            if (tileInPool != null)
            {
                // --- ГЛАВНОЕ ИЗМЕНЕНИЕ ---
                // Вместо имитации кликов напрямую вызываем наш новый метод
                inputController.PlaceTileForHint(tileInPool, randomSlot);
                // -------------------------

                Debug.Log($"Подсказка: слог '{correctSyllable}' установлен.");
                return; // Выходим после успешной подсказки
            }
        }

        Debug.Log("Не удалось найти подходящую плитку в пуле для доступных слотов.");
    }

    private Tile FindTileInPool(string text)
    {
        foreach (var tile in tilePoolContainer.GetComponentsInChildren<Tile>())
        {
            if (tile.Letter == text && tile.IsInteractable())
            {
                return tile;
            }
        }
        return null;
    }
}
