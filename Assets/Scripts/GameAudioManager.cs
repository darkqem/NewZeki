using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Audio Mixer Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string volumeParameter = "MasterVolume";
    
    [Header("Background Music Settings")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playMusicOnStart = true;
    private AudioSource musicSource;
    
    [Header("Save Settings")]
    [SerializeField] private bool loadOnStart = true;
    private const string VolumeKey = "GameVolume";

    private void Awake()
    {
        // Реализация паттерна Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Инициализация компонента AudioSource
            InitializeAudioSource();
            
            // Подписываемся на событие смены сцены
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (playMusicOnStart)
            {
                PlayBackgroundMusic();
            }

            // Загружаем сохраненную громкость
            LoadSavedVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioSource()
    {
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        
        if (audioMixer != null)
        {
            AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("Master");
            if (groups.Length > 0)
            {
                musicSource.outputAudioMixerGroup = groups[0];
                Debug.Log("[GameAudioManager] AudioSource успешно настроен на группу Master");
            }
            else
            {
                Debug.LogError("[GameAudioManager] Группа Master не найдена в Audio Mixer!");
            }
        }
    }

    private void LoadSavedVolume()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey);
            SetVolume(savedVolume, false);
            Debug.Log($"[GameAudioManager] Загружена сохраненная громкость: {savedVolume}");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameAudioManager] Загружена сцена: {scene.name}");
        
        // Переприменяем текущие настройки звука
        if (audioMixer != null && audioMixer.GetFloat(volumeParameter, out float currentDb))
        {
            float currentVolume = currentDb <= -80f ? 0f : Mathf.Pow(10f, currentDb / 20f);
            SetVolume(currentVolume, false);
            Debug.Log($"[GameAudioManager] Перепримененa громкость: {currentVolume}");
        }

        // Перезапускаем музыку если нужно
        if (playMusicOnStart && !musicSource.isPlaying)
        {
            PlayBackgroundMusic();
        }
    }

    public void SetVolume(float volume, bool saveToPrefs = false)
    {
        if (audioMixer == null)
        {
            Debug.LogError("[GameAudioManager] Audio Mixer не назначен!");
            return;
        }

        // Убеждаемся, что значение в допустимом диапазоне
        volume = Mathf.Clamp01(volume);

        // Конвертируем значение слайдера (0-1) в децибелы (-80 до 0)
        float dbValue = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        
        // Применяем значение к Audio Mixer и проверяем результат
        bool success = audioMixer.SetFloat(volumeParameter, dbValue);
        if (success)
        {
            Debug.Log($"[GameAudioManager] Установлена громкость: {volume} ({dbValue} dB)");
        }
        else
        {
            Debug.LogError($"[GameAudioManager] Не удалось установить громкость: {volume} ({dbValue} dB)");
        }
        
        // Сохраняем значение если указано
        if (saveToPrefs)
        {
            PlayerPrefs.SetFloat(VolumeKey, volume);
            PlayerPrefs.Save();
            Debug.Log($"[GameAudioManager] Громкость сохранена в PlayerPrefs: {volume}");
        }
    }

    public float GetVolume()
    {
        // Сначала пытаемся получить сохраненное значение
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(VolumeKey);
            Debug.Log($"Загружено сохраненное значение громкости: {savedVolume}");
            return savedVolume;
        }

        // Если сохраненного значения нет, получаем текущее из микшера
        if (audioMixer != null && audioMixer.GetFloat(volumeParameter, out float dbValue))
        {
            float volume = dbValue <= -80f ? 0f : Mathf.Pow(10f, dbValue / 20f);
            Debug.Log($"Получено значение громкости из микшера: {volume} ({dbValue} dB)");
            return volume;
        }

        Debug.Log("Используется значение громкости по умолчанию: 0.0");
        return 0f; // Возвращаем тишину по умолчанию
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        Debug.Log($"Загрузка сохраненной громкости: {savedVolume}");
        SetVolume(savedVolume, false);
        
        // Проверяем, что значение действительно применилось
        if (audioMixer.GetFloat(volumeParameter, out float currentDb))
        {
            Debug.Log($"Текущее значение в микшере: {currentDb} dB");
        }
    }

    private void LoadVolumeImmediate()
    {
        if (audioMixer == null)
        {
            Debug.LogError("Audio Mixer не назначен! Невозможно загрузить громкость.");
            return;
        }

        // Проверяем, есть ли сохраненное значение
        if (!PlayerPrefs.HasKey(VolumeKey))
        {
            Debug.Log("Сохраненное значение громкости не найдено, используется значение по умолчанию: 1");
            SetVolume(1f, true); // Сохраняем значение по умолчанию
            return;
        }

        // Получаем сохраненное значение
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0f);
        Debug.Log($"Загружаем сохраненную громкость при старте: {savedVolume}");

        // Конвертируем значение в децибелы
        float dbValue = savedVolume > 0 ? 20f * Mathf.Log10(savedVolume) : -80f;
        
        // Применяем значение к микшеру
        if (audioMixer.SetFloat(volumeParameter, dbValue))
        {
            Debug.Log($"Громкость успешно установлена: {savedVolume} ({dbValue} dB)");
        }
        else
        {
            Debug.LogError($"Не удалось установить громкость: {savedVolume} ({dbValue} dB)");
        }

        // Проверяем, что значение действительно установилось
        if (audioMixer.GetFloat(volumeParameter, out float currentDb))
        {
            float currentVolume = currentDb <= -80f ? 0f : Mathf.Pow(10f, currentDb / 20f);
            Debug.Log($"Проверка текущей громкости: {currentVolume} ({currentDb} dB)");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void ResetVolume()
    {
        SetVolume(1f, true);
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
} 