// Файл: WordEntry.cs (полностью замените)

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WordEntry
{
    [Tooltip("Слово, разбитое на слоги или части")]
    public List<string> wordParts = new List<string>();

    [Tooltip("Подсказка для этого слова")]
    public string clue;

    // Улучшенное свойство FullWord
    public string FullWord
    {
        get
        {
            string result = "";
            foreach (string part in wordParts)
            {
                // Убираем пробелы у каждой части перед соединением
                result += part.Trim();
            }
            return result;
        }
    }
}
