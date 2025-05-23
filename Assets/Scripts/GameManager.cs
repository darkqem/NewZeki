using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public UnityEvent onDayEnd = new UnityEvent();
    public UnityEvent onNewDay = new UnityEvent();

    [SerializeField] private CanvasGroup dayNotificationPanel; // Панель с уведомлением о новом дне
    [SerializeField] private float notificationDuration = 2f; // Длительность показа уведомления
    [SerializeField] private float fadeDuration = 0.5f; // Длительность появления/исчезновения

    private GameSaveData saveData;
    private string savePath;

    [SerializeField] private DialogueManager dialogueManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        savePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
        LoadGame();
        
        // Скрываем панель уведомления при старте
        if (dayNotificationPanel != null)
        {
            dayNotificationPanel.alpha = 0f;
            dayNotificationPanel.gameObject.SetActive(false);
        }
    }

    public void EndDay()
    {
        onDayEnd.Invoke();
        saveData.currentDay++;
        saveData.dialogShownToday = false;
        SaveGame();
        
        // Показываем уведомление о новом дне
        if (dayNotificationPanel != null)
        {
            StartCoroutine(ShowDayNotification());
        }
        
        onNewDay.Invoke();
        Debug.Log($"День {saveData.currentDay - 1} завершен. Начинается день {saveData.currentDay}");
    }

    private IEnumerator ShowDayNotification()
    {
        dayNotificationPanel.gameObject.SetActive(true);

        // Плавное появление
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            dayNotificationPanel.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            yield return null;
        }

        // Ждем указанное время
        yield return new WaitForSeconds(notificationDuration);

        // Плавное исчезновение
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            dayNotificationPanel.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        dayNotificationPanel.gameObject.SetActive(false);
    }

    public bool CanShowDialog()
    {
        return !saveData.dialogShownToday;
    }

    public void MarkDialogAsShown(int dialogIndex)
    {
        saveData.dialogShownToday = true;
        saveData.lastShownDialogIndex = dialogIndex;
        SaveGame();
    }

    public int GetCurrentDay()
    {
        return saveData.currentDay;
    }

    public int GetLastShownDialogIndex()
    {
        return saveData.lastShownDialogIndex;
    }

    private void SaveGame()
    {
        string jsonData = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, jsonData);
        Debug.Log("Игра сохранена");
    }

    private void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
            Debug.Log($"Игра загружена. Текущий день: {saveData.currentDay}");
        }
        else
        {
            saveData = new GameSaveData();
            Debug.Log("Создано новое сохранение");
        }
    }

    // Метод для начала новой игры
    public void StartNewGame()
    {
        // Сбрасываем все данные сохранения
        saveData = new GameSaveData();
        
        // Сбрасываем прогресс диалогов
        if (dialogueManager != null)
        {
            dialogueManager.ResetProgress();
        }
        
        // Удаляем файл сохранения, если он существует
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        
        // Сохраняем новое пустое состояние игры
        SaveGame();
        
        // Загружаем начальную сцену
        SceneManager.LoadScene(1);
    }
    
    // Метод для продолжения игры (загрузка сохранённого прогресса)
    public void ContinueGame()
    {
        // Здесь можно добавить загрузку сохранённого прогресса
        SceneManager.LoadScene("SampleScene day");
    }
    
    // Метод для выхода из игры
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 