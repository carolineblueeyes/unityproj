// Файл: SymmetricalPlaceholderGrid.cs
// Финальная версия, реализующая вашу идею с симметричными пустышками.

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(GridLayoutGroup))]
public class SymmetricalPlaceholderGrid : MonoBehaviour
{
 [Header("Настройки")]
    [SerializeField] private GameObject placeholderPrefab;

    private GridLayoutGroup grid;
    private RectTransform rectTransform;
    private List<GameObject> createdPlaceholders = new List<GameObject>();
    private bool isUpdating = false;

    void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartCoroutine(UpdatePlaceholdersRoutine());
    }

    void OnTransformChildrenChanged()
    {
        if (Application.isPlaying && !isUpdating)
        {
            StartCoroutine(UpdatePlaceholdersRoutine());
        }
    }

    private IEnumerator UpdatePlaceholdersRoutine()
    {
        if (isUpdating) yield break;
        isUpdating = true;
        yield return new WaitForEndOfFrame();

        foreach (var placeholder in createdPlaceholders)
        {
            if (placeholder != null) Destroy(placeholder);
        }
        createdPlaceholders.Clear();

        if (grid == null || rectTransform == null || placeholderPrefab == null)
        {
            isUpdating = false;
            yield break;
        }

        float containerWidth = rectTransform.rect.width;
        if (containerWidth <= 0) { isUpdating = false; yield break; }
        int columns = Mathf.FloorToInt((containerWidth - grid.padding.left - grid.padding.right + grid.spacing.x) / (grid.cellSize.x + grid.spacing.x));
        if (columns <= 0) columns = 1;

        List<Transform> activeVisibleTiles = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf && child.GetComponent<PlaceholderIdentifier>() == null && child.GetComponent<Tile>() != null)
            {
                activeVisibleTiles.Add(child);
            }
        }

        if (activeVisibleTiles.Count == 0 || activeVisibleTiles.Count % columns == 0)
        {
            isUpdating = false;
            yield break;
        }

        int itemsInLastRow = activeVisibleTiles.Count % columns;
        int placeholdersTotal = columns - itemsInLastRow;
        int placeholdersBefore = placeholdersTotal / 2;
        int placeholdersAfter = placeholdersTotal - placeholdersBefore;
        int insertionIndex = activeVisibleTiles.Count - itemsInLastRow;

        for (int i = 0; i < placeholdersBefore; i++)
        {
            GameObject p = Instantiate(placeholderPrefab, transform);
            p.AddComponent<PlaceholderIdentifier>();
            p.transform.SetSiblingIndex(insertionIndex);
            createdPlaceholders.Add(p);
        }
        for (int i = 0; i < placeholdersAfter; i++)
        {
            GameObject p = Instantiate(placeholderPrefab, transform);
            p.AddComponent<PlaceholderIdentifier>();
            p.transform.SetAsLastSibling();
            createdPlaceholders.Add(p);
        }
        isUpdating = false;
    }
}

// Пустой класс-маркер, чтобы отличать пустышки от реальных плиток
public class PlaceholderIdentifier : MonoBehaviour { }
