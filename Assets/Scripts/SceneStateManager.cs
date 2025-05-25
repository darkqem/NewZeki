using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    private static SceneStateManager instance;
    public static SceneStateManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Подписываемся на событие загрузки сцены
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Сбрасываем состояние всех панелей
        PanelManager.ResetAllPanels();
    }
} 