// Файл: LevelGenerator.cs (ПОЛНАЯ ИСПРАВЛЕННАЯ ВЕРСИЯ)

using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Data")]
    [SerializeField] private LevelData currentLevelData;

    [Header("UI Prefabs")]
    [SerializeField] private GameObject puzzleRowPrefab;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject tilePrefab;

    [Header("UI Containers")]
    [SerializeField] private Transform puzzleAreaContainer;
    [SerializeField] private Transform tilePoolContainer;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;


// Внутри класса LevelGenerator, в секции с переменными
[Header("Ссылки на контроллеры")]
[SerializeField] private TimerController timerController;

    void Start()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("Не назначены данные уровня (Level Data) в инспекторе для LevelGenerator!");
            return;
        }
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // Очистка поля перед генерацией нового уровня
        foreach (Transform child in puzzleAreaContainer) Destroy(child.gameObject);
        foreach (Transform child in tilePoolContainer) Destroy(child.gameObject);

        // Настройка UI
        levelText.text = $"# {currentLevelData.puzzleID.Replace("_", " ")}";
        timerText.text = "00:00";

        // Создание строк головоломки
        foreach (var wordEntry in currentLevelData.words)
        {
            GameObject rowInstance = Instantiate(puzzleRowPrefab, puzzleAreaContainer);
            rowInstance.name = $"Row_{wordEntry.FullWord}";

            // --- ИСПРАВЛЕНИЕ №1: Инициализация строки ---
            PuzzleRow puzzleRowComponent = rowInstance.GetComponent<PuzzleRow>();
            if (puzzleRowComponent != null)
            {
                // Передаем и слово, и подсказку
                //puzzleRowComponent.Initialize(wordEntry.FullWord, wordEntry.clue);
                    puzzleRowComponent.Initialize(wordEntry.FullWord, wordEntry.clue, wordEntry.wordParts);

            }

            // --- ИСПРАВЛЕНИЕ №2: Поиск контейнера слотов ---
            // Указываем полный путь к контейнеру: "Content/Slots_Container"
            Transform slotsContainer = rowInstance.transform.Find("Content/Slots_Container");
            
            if (slotsContainer == null)
            {
                Debug.LogError($"Не удалось найти 'Content/Slots_Container' в префабе {rowInstance.name}!", rowInstance);
                continue; // Пропускаем создание слотов для этой строки, чтобы избежать ошибок
            }

            // Создаем нужное количество слотов внутри найденного контейнера
            for (int i = 0; i < wordEntry.wordParts.Count; i++)
            {
                Instantiate(slotPrefab, slotsContainer);
            }
        }

        // Создание пула плиток (этот код не меняется)
        List<string> tilesToCreate = new List<string>();
        foreach (var wordEntry in currentLevelData.words)
        {
            tilesToCreate.AddRange(wordEntry.wordParts);
        }
        tilesToCreate.AddRange(currentLevelData.distractorTiles);

        System.Random rng = new System.Random();
        tilesToCreate = tilesToCreate.OrderBy(a => rng.Next()).ToList();

        foreach (string tileText in tilesToCreate)
        {
            GameObject tileInstance = Instantiate(tilePrefab, tilePoolContainer);
            tileInstance.name = $"Tile_{tileText}";

            Tile tileComponent = tileInstance.GetComponent<Tile>();
            if (tileComponent != null)
            {
                tileComponent.Initialize(tileText);
            }
        }

        // Запускаем таймер
if (timerController != null)
{
    timerController.StartTimer();
}
else
{
    Debug.LogError("Ссылка на TimerController не установлена в инспекторе!");
}
    }
}
