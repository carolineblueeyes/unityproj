// Файл: TimerController.cs
// Управляет простым таймером-секундомером.

using UnityEngine;
using TMPro; // Убедитесь, что используете TextMeshPro

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerController : MonoBehaviour
{
    private TextMeshProUGUI timerText;
    private float timeElapsed;
    private bool isTimerRunning = false;

    void Awake()
    {
        // Получаем компонент текста при старте
        timerText = GetComponent<TextMeshProUGUI>();
        // Устанавливаем начальное значение
        timerText.text = "00:00";
    }

    void Update()
    {
        // Если таймер не запущен, ничего не делаем
        if (!isTimerRunning)
        {
            return;
        }

        // Увеличиваем счетчик времени
        timeElapsed += Time.deltaTime;

        // Форматируем время в минуты и секунды
        int minutes = (int)(timeElapsed / 60);
        int seconds = (int)(timeElapsed % 60);

        // Обновляем текстовое поле, используя форматирование D2 для добавления ведущего нуля (01, 02, ...)
        timerText.text = $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// Запускает таймер.
    /// </summary>
    public void StartTimer()
    {
        if (isTimerRunning) return; // Не запускать, если уже запущен
        
        timeElapsed = 0f;
        isTimerRunning = true;
        Debug.Log("Таймер запущен!");
    }

    /// <summary>
    /// Останавливает таймер.
    /// </summary>
    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log($"Таймер остановлен! Финальное время: {timerText.text}");
    }

    /// <summary>
    /// Возвращает общее количество прошедших секунд.
    /// </summary>
    /// <returns>Время в секундах</returns>
    public float GetTimeElapsed()
    {
        return timeElapsed;
    }
}
