using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public UnityEvent onDayEnd = new UnityEvent();
    public UnityEvent onNewDay = new UnityEvent();
    public UnityEvent onNewGameStarted = new UnityEvent();

    [SerializeField] private CanvasGroup dayNotificationPanel; // Панель с уведомлением о новом дне
    [SerializeField] private float notificationDuration = 2f; // Длительность показа уведомления
    [SerializeField] private float fadeDuration = 0.5f; // Длительность появления/исчезновения

    private GameSaveData saveData;
    private string savePath;

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

    public void StartNewGame()
    {
        Debug.Log("Starting new game - resetting all game data");
        // Создаем новые данные сохранения
        saveData = new GameSaveData();
        saveData.currentDay = 1;
        saveData.dialogShownToday = false;
        saveData.lastShownDialogIndex = -1; // Сбрасываем индекс последнего показанного диалога
        
        // Сохраняем новое состояние игры
        SaveGame();
        
        Debug.Log($"New game started. Day: {saveData.currentDay}, dialogShownToday: {saveData.dialogShownToday}, lastDialogIndex: {saveData.lastShownDialogIndex}");
        
        // Вызываем событие начала новой игры
        onNewGameStarted.Invoke();
    }

    public void EndDay()
    {
        onDayEnd.Invoke();
        saveData.currentDay++;
        saveData.dialogShownToday = false; // Сбрасываем флаг показа диалога для нового дня
        SaveGame();
        
        // Показываем уведомление о новом дне
        if (dayNotificationPanel != null)
        {
            StartCoroutine(ShowDayNotification());
        }
        
        onNewDay.Invoke();
        Debug.Log($"День {saveData.currentDay - 1} завершен. Начинается день {saveData.currentDay}. Диалоги доступны: {!saveData.dialogShownToday}");
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
        Debug.Log($"Проверка возможности показа диалога - День {saveData.currentDay}, Диалог уже показан сегодня: {saveData.dialogShownToday}");
        return !saveData.dialogShownToday;
    }

    public void MarkDialogAsShown(int dialogIndex)
    {
        Debug.Log($"Отмечаем диалог {dialogIndex} как показанный в день {saveData.currentDay}");
        saveData.dialogShownToday = true;
        saveData.lastShownDialogIndex = dialogIndex;
        SaveGame();
        Debug.Log($"Диалог {dialogIndex} отмечен как показанный. Следующий диалог будет доступен в следующий день.");
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
        Debug.Log($"Игра сохранена. День: {saveData.currentDay}, Диалог показан: {saveData.dialogShownToday}, Последний диалог: {saveData.lastShownDialogIndex}");
    }

    private void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            saveData = JsonUtility.FromJson<GameSaveData>(jsonData);
            Debug.Log($"Игра загружена. День: {saveData.currentDay}, Диалог показан: {saveData.dialogShownToday}, Последний диалог: {saveData.lastShownDialogIndex}");
        }
        else
        {
            saveData = new GameSaveData();
            Debug.Log("Создано новое сохранение с начальными значениями");
        }
    }
} 