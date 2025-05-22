using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeSettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button saveButton;
    [SerializeField] private TextMeshProUGUI volumeText;

    private float currentVolume;
    private GameAudioManager audioManager;
    private bool isInitializing = true;

    private void OnEnable()
    {
        // При активации объекта переинициализируем UI
        InitializeUI();
    }

    private void Start()
    {
        // Инициализация при старте
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Получение ссылки на GameAudioManager
        audioManager = GameAudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("[VolumeSettingsUI] GameAudioManager не найден!");
            return;
        }

        // Проверка и настройка компонентов UI
        if (!ValidateComponents()) return;

        // Настраиваем слайдер
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;

        // Получаем текущую громкость из AudioManager
        currentVolume = audioManager.GetVolume();
        Debug.Log($"[VolumeSettingsUI] Начальная громкость UI: {currentVolume}");

        // Обновляем UI без вызова событий
        volumeSlider.SetValueWithoutNotify(currentVolume);
        UpdateUI();

        // Удаляем старые слушатели перед добавлением новых
        volumeSlider.onValueChanged.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();

        // Добавляем слушатели событий
        volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
        saveButton.onClick.AddListener(SaveVolumeSettings);

        isInitializing = false;

        // Применяем начальную громкость
        audioManager.SetVolume(currentVolume, false);
    }

    private bool ValidateComponents()
    {
        if (volumeSlider == null)
        {
            Debug.LogError("[VolumeSettingsUI] Volume Slider не назначен!");
            return false;
        }

        if (saveButton == null)
        {
            Debug.LogError("[VolumeSettingsUI] Save Button не назначен!");
            return false;
        }

        if (volumeText == null)
        {
            Debug.LogError("[VolumeSettingsUI] Volume Text не назначен!");
            return false;
        }

        return true;
    }

    private void UpdateUI()
    {
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(currentVolume);
            Debug.Log($"[VolumeSettingsUI] Слайдер установлен на значение: {currentVolume}");
        }

        if (volumeText != null)
        {
            volumeText.text = $"{(currentVolume * 100):F0}%";
            Debug.Log($"[VolumeSettingsUI] Текст обновлен: {volumeText.text}");
        }
    }

    public void OnVolumeSliderChanged(float value)
    {
        if (!isInitializing && audioManager != null)
        {
            currentVolume = value;
            audioManager.SetVolume(currentVolume, false);
            UpdateUI();
            Debug.Log($"[VolumeSettingsUI] Слайдер изменен на: {value}");
        }
    }

    public void SaveVolumeSettings()
    {
        if (audioManager != null)
        {
            audioManager.SetVolume(currentVolume, true);
            Debug.Log($"[VolumeSettingsUI] Настройки сохранены. Текущая громкость: {currentVolume}");
        }
    }

    private void OnDisable()
    {
        // Сохраняем настройки при деактивации UI
        SaveVolumeSettings();
    }

    private void OnDestroy()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
        }

        if (saveButton != null)
        {
            saveButton.onClick.RemoveListener(SaveVolumeSettings);
        }
    }
} 