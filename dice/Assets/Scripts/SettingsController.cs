// Файл: SettingsController.cs

using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private GameObject settingsPanel; // Сама панель

    void Start()
    {
        // Загружаем сохраненные значения и устанавливаем их на слайдеры
        musicSlider.value = GameManager.Instance.PlayerData.musicVolume;
        sfxSlider.value = GameManager.Instance.PlayerData.sfxVolume;

        // Добавляем слушателей на изменение значений
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        
        // Применяем значения при старте
        ApplyVolumeSettings();
    }

    private void OnMusicVolumeChanged(float value)
    {
        GameManager.Instance.PlayerData.musicVolume = value;
        ApplyVolumeSettings();
    }

    private void OnSfxVolumeChanged(float value)
    {
        GameManager.Instance.PlayerData.sfxVolume = value;
        ApplyVolumeSettings();
    }

    private void ApplyVolumeSettings()
    {
        AudioManager.Instance.SetMusicVolume(musicSlider.value);
        AudioManager.Instance.SetSfxVolume(sfxSlider.value);
    }

    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        // При закрытии панели сохраняем данные
        GameManager.Instance.SaveData();
        settingsPanel.SetActive(false);
    }
}
