using UnityEngine;
using cherrydev;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph[] dialogGraphs;
    [SerializeField] private string characterId;
    [SerializeField] private bool showDialogOnStart = true; // Флаг для автоматического показа диалога при старте
    
    private int currentGameDay; // Текущий игровой день
    private int currentDialogIndex; // Текущий индекс диалога
    private bool wasDialogViewedToday; // Флаг для отслеживания просмотра диалога в текущий день
    
    private void Start()
    {
        // Получаем текущий игровой день из GameManager
        currentGameDay = GameManager.Instance.GetCurrentDay();
        // Устанавливаем индекс диалога в зависимости от дня
        currentDialogIndex = (currentGameDay - 1) % dialogGraphs.Length;
        wasDialogViewedToday = false;

        // Если включен автопоказ диалога при старте
        if (showDialogOnStart)
        {
            ShowDialogue();
        }
    }
    
    // Показать диалог
    public void ShowDialogue()
    {
        if (dialogGraphs == null || dialogGraphs.Length == 0)
        {
            Debug.LogWarning($"Нет диалогов для персонажа {characterId}!");
            return;
        }

        // Проверяем, не изменился ли день
        int gameDay = GameManager.Instance.GetCurrentDay();
        if (gameDay != currentGameDay)
        {
            // День изменился, обновляем индекс диалога и сбрасываем флаг просмотра
            currentGameDay = gameDay;
            currentDialogIndex = (currentGameDay - 1) % dialogGraphs.Length;
            wasDialogViewedToday = false;
        }

        // Показываем текущий диалог
        dialogBehaviour.StartDialog(dialogGraphs[currentDialogIndex]);
        wasDialogViewedToday = true;
        Debug.Log($"Персонаж {characterId}: показан диалог {currentDialogIndex + 1} (День {currentGameDay})");
    }
    
    // Сброс прогресса при новой игре
    public void ResetProgress()
    {
        currentGameDay = 1;
        currentDialogIndex = 0;
        wasDialogViewedToday = false;
        Debug.Log($"Прогресс диалогов для персонажа {characterId} сброшен");
    }
    
    // Получить текущий индекс диалога
    public int GetCurrentDialogIndex()
    {
        return currentDialogIndex;
    }
    
    // Получить текущий игровой день
    public int GetCurrentGameDay()
    {
        return currentGameDay;
    }

    // Проверить, был ли просмотрен диалог сегодня
    public bool WasDialogViewedToday()
    {
        return wasDialogViewedToday;
    }
} 