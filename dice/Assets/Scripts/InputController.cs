// Файл: InputController.cs (ПОЛНАЯ ФИНАЛЬНАЯ ВЕРСИЯ)

using UnityEngine;
using System.Linq;

public class InputController : MonoBehaviour
{
    [SerializeField] private TimerController timerController;
    private Slot selectedSlot;

   private void OnEnable()
    {
        Tile.OnTileClicked += HandleTileClicked;
        PuzzleRow.OnRowClicked += HandleRowClicked;
        Slot.OnSlotClicked += HandleSlotClicked;
    }

    private void OnDisable()
    {
        Tile.OnTileClicked -= HandleTileClicked;
        PuzzleRow.OnRowClicked -= HandleRowClicked;
        Slot.OnSlotClicked -= HandleSlotClicked;
    }

    private void HandleRowClicked(PuzzleRow row)
    {
        if (row.IsCompleted) return;
        SelectNextEmptySlotInRow(row);
    }

    private void HandleSlotClicked(Slot clickedSlot)
    {
        PuzzleRow parentRow = clickedSlot.GetComponentInParent<PuzzleRow>();
        if (parentRow.IsCompleted) return;

        if (clickedSlot.IsOccupied)
        {
            ReturnTileFromSlot(clickedSlot);
        }
        else
        {
            SelectSlot(clickedSlot);
        }
    }

// Файл: InputController.cs

// ... (другие методы) ...

private void HandleTileClicked(Tile clickedTile, bool isCopy)
{
    // 1. Игнорируем клики по копиям плиток в слотах
    if (isCopy) return;

    // 2. Игнорируем клик, если ни один слот не выбран или он уже занят
    if (selectedSlot == null || selectedSlot.IsOccupied) return;

    // --- ВОТ КЛЮЧЕВАЯ ЛОГИКА ---
    // 3. Создаем ПОЛНОЦЕННУЮ КОПИЮ плитки
    Tile tileCopy = clickedTile.CreateCopy();

    // 4. Помещаем эту КОПИЮ в выбранный слот.
    //    Метод PlaceTile внутри Slot делает ее дочерней и прячет текст-заполнитель.
    selectedSlot.PlaceTile(tileCopy);
    // ---------------------------

    // 5. Делаем оригинальную плитку в пуле серой (неактивной)
    clickedTile.SetState(false);
    
    // 6. Получаем текущую строку для последующей проверки
    PuzzleRow currentRow = selectedSlot.GetComponentInParent<PuzzleRow>();
    
    // 7. Снимаем выделение со слота
    selectedSlot.SetHighlight(false);
    selectedSlot = null;

    // 8. Проверяем, не собрано ли слово
    CheckWordCompletion(currentRow);

    // 9. Если слово еще не собрано, выбираем следующий пустой слот в этой строке
    if (!currentRow.IsCompleted)
    {
        SelectNextEmptySlotInRow(currentRow);
    }
}

// ... (остальные методы) ...


    // --- ИЗМЕНЕНИЕ ТОЛЬКО В ЭТОМ МЕТОДЕ ---
    private void ReturnTileFromSlot(Slot slot)
    {
        if (!slot.IsOccupied) return;
        Tile tileToRemove = slot.RemoveTile();

        // --- ГЛАВНОЕ ИСПРАВЛЕНИЕ ---
        // Мы больше не обращаемся к мертвому оригиналу.
        // Мы просим менеджера найти живую плитку с таким же текстом и активировать ее.
        if (TilePoolManager.Instance != null)
        {
            TilePoolManager.Instance.ActivateTileByLetter(tileToRemove.Letter);
        }
        // -------------------------

        Destroy(tileToRemove.gameObject);
        SelectSlot(slot);
    }

    // --- ФИНАЛЬНАЯ ВЕРСИЯ ЭТОГО МЕТОДА ---
    private void CheckWordCompletion(PuzzleRow row)
    {
        var slots = row.GetComponentsInChildren<Slot>();
        if (!slots.All(s => s.IsOccupied))
        {
            // Это нормальное поведение, просто выходим
            return;
        }

        // Если все слоты заняты, проверяем слово
        if (row.CheckWord())
        {
            // Слово верное! Вызываем подсветку.
            row.SetCompleted();
            // TODO: Проверить, не завершен ли весь уровень
        }
        else
        {
            // Слово неверное
            Debug.Log($"Слово '{row.CorrectWord}' собрано неверно!");
        }
        if (row.CheckWord())
{
    row.SetCompleted();
    
    // Проверяем, пройден ли весь уровень
    CheckLevelCompletion();
}
    }

    private void SelectSlot(Slot slot)
    {
        DeselectCurrentSlot();
        selectedSlot = slot;
        if (selectedSlot != null)
        {
            selectedSlot.SetHighlight(true);
        }
    }

    private void DeselectCurrentSlot()
    {
        if (selectedSlot != null)
        {
            selectedSlot.SetHighlight(false);
            selectedSlot = null;
        }
    }

    private void SelectNextEmptySlotInRow(PuzzleRow row)
    {
        if (row.IsCompleted)
        {
            DeselectCurrentSlot();
            return;
        }

        var slotsInRow = row.GetComponentsInChildren<Slot>();
        Slot nextEmptySlot = slotsInRow.FirstOrDefault(s => !s.IsOccupied);

        if (nextEmptySlot != null)
        {
            SelectSlot(nextEmptySlot);
        }
        else
        {
            DeselectCurrentSlot();
        }
    }


    // Новый метод в InputController.cs
private void CheckLevelCompletion()
{
    // Проходим по всем строкам, которые знает LevelGenerator
    // (Нам нужна ссылка на LevelGenerator)
    // Давайте пока сделаем проще, без сложных ссылок.
    // Мы просто проверим, все ли строки в сцене завершены.
    
    PuzzleRow[] allRows = FindObjectsOfType<PuzzleRow>();
    bool allWordsCompleted = true;
    foreach (var row in allRows)
    {
        if (!row.IsCompleted)
        {
            allWordsCompleted = false;
            break; // Если хотя бы одно слово не завершено, выходим из цикла
        }
    }

    if (allWordsCompleted)
    {
        Debug.LogWarning("УРОВЕНЬ ПРОЙДЕН!");
        if (timerController != null)
        {
            timerController.StopTimer();
        }
        // Здесь в будущем будет логика показа экрана победы, начисления очков и т.д.
    }
}
 // --- НОВЫЙ МЕТОД СПЕЦИАЛЬНО ДЛЯ ПОДСКАЗКИ ---
    /// <summary>
    /// Устанавливает плитку в указанный слот "тихо", без выбора следующего слота.
    /// </summary>
    public void PlaceTileForHint(Tile tileFromPool, Slot targetSlot)
    {
        // Проверяем, что все на месте
        if (tileFromPool == null || targetSlot == null || targetSlot.IsOccupied)
        {
            return;
        }

        // Создаем копию плитки и размещаем ее
        Tile tileCopy = tileFromPool.CreateCopy();
        targetSlot.PlaceTile(tileCopy);

        // Делаем оригинальную плитку в пуле неактивной
        tileFromPool.SetState(false);

        // Проверяем, не завершилось ли слово (или весь уровень)
        PuzzleRow currentRow = targetSlot.GetComponentInParent<PuzzleRow>();
        CheckWordCompletion(currentRow);

        // ВАЖНО: Мы НЕ вызываем здесь SelectNextEmptySlotInRow()
        // и НЕ меняем selectedSlot.
        // Мы также должны убедиться, что текущий выделенный слот (если он есть) сбрасывается.
        DeselectCurrentSlot();
    }
}
