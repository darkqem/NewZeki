using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioInitializer : MonoBehaviour
{
    private static AudioInitializer instance;
    private GameAudioManager audioManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAudio();
    }

    private void InitializeAudio()
    {
        // Получаем экземпляр GameAudioManager
        audioManager = GameAudioManager.Instance;
        
        if (audioManager != null)
        {
            // Проверяем, есть ли сохраненное значение
            if (PlayerPrefs.HasKey("GameVolume"))
            {
                float savedVolume = PlayerPrefs.GetFloat("GameVolume");
                Debug.Log($"[AudioInitializer] Загружаем сохраненную громкость: {savedVolume}");
                audioManager.SetVolume(savedVolume, false);
            }
            else
            {
                Debug.Log("[AudioInitializer] Сохраненная громкость не найдена, используем значение по умолчанию");
                audioManager.SetVolume(1f, true);
            }
        }
        else
        {
            Debug.LogError("[AudioInitializer] GameAudioManager не найден!");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[AudioInitializer] Загрузка сцены: {scene.name}");
        InitializeAudio();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
} 