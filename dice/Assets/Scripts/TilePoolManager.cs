// Файл: TilePoolManager.cs
// Создайте этот скрипт и поместите его на объект TilePool_Container

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TilePoolManager : MonoBehaviour
{
    // Статическая ссылка, чтобы любой скрипт мог легко получить доступ к менеджеру
    public static TilePoolManager Instance { get; private set; }

    private readonly Dictionary<string, List<Tile>> tilesInPool = new Dictionary<string, List<Tile>>();

    void Awake()
    {
        // Реализация синглтона
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Вызывается при создании новой плитки, чтобы зарегистрировать ее.
    /// </summary>
    public void RegisterTile(Tile tile)
    {
        if (!tilesInPool.ContainsKey(tile.Letter))
        {
            tilesInPool[tile.Letter] = new List<Tile>();
        }
        tilesInPool[tile.Letter].Add(tile);
    }

    /// <summary>
    /// Вызывается при уничтожении плитки, чтобы удалить ее из реестра.
    /// </summary>
    public void UnregisterTile(Tile tile)
    {
        if (tilesInPool.ContainsKey(tile.Letter))
        {
            tilesInPool[tile.Letter].Remove(tile);
        }
    }

    /// <summary>
    /// Находит первую доступную (неактивную) плитку с нужным текстом и активирует ее.
    /// </summary>
    public void ActivateTileByLetter(string letter)
    {
        if (tilesInPool.ContainsKey(letter))
        {
            // Ищем первую плитку, которая сейчас неактивна (серая)
            Tile tileToActivate = tilesInPool[letter].FirstOrDefault(t => !t.IsInteractable());
            if (tileToActivate != null)
            {
                tileToActivate.SetState(true);
            }
        }
    }
}
