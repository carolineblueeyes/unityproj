// Файл: PlayerData.cs

[System.Serializable]
public class PlayerData
{
    public int lastCompletedLevel; // Номер последнего пройденного уровня
    public float musicVolume;
    public float sfxVolume;

    // Конструктор с значениями по умолчанию
    public PlayerData()
    {
        lastCompletedLevel = 0; // 0 означает, что ни один уровень не пройден
        musicVolume = 0.8f;
        sfxVolume = 1.0f;
    }
}
