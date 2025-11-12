// Файл: LevelData.cs
// Наследуется от ScriptableObject, чтобы создавать ассеты уровней.

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "WordWeaver/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Level Settings")]
    public string puzzleID; // Например, "Level_01"
    public int timeLimitInSeconds; // Время на уровень в секундах

    [Header("Puzzle Content")]
    [Tooltip("Ровно 5 слов для этой головоломки")]
    public List<WordEntry> words = new List<WordEntry>(5);

    [Header("Distractors")]
    [Tooltip("Дополнительные плитки-обманки (слоги или буквы)")]
    public List<string> distractorTiles;
}
