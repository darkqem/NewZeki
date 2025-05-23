using UnityEngine;
using cherrydev;


public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph[] dialogGraphs;
    [SerializeField] private string characterId;
    [SerializeField] private Scenes sceneTransition;
    [SerializeField] private Scenes1 sceneTransition1;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool showDialogOnStart = true; // Флаг для автоматического показа диалога при старте
    public int scneneNumber = 6;
    public int scneneNumber1 = 7;
    private int currentGameDay; // Текущий игровой день
    private int currentDialogIndex; // Текущий индекс диалога
    private bool wasDialogViewedToday; // Флаг для отслеживания просмотра диалога в текущий день
    private bool waitingForTransitionClick = false;
    private bool waitingForTransition1Click = false;
    private System.Action pendingTransition = null;
    
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
    
    private void Update()
    {
        if ((waitingForTransitionClick || waitingForTransition1Click) && Input.GetMouseButtonDown(0))
        {
            if (pendingTransition != null)
            {
                pendingTransition.Invoke();
                pendingTransition = null;
            }
            waitingForTransitionClick = false;
            waitingForTransition1Click = false;
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

        dialogBehaviour.BindExternalFunction("Transition", () => {
            waitingForTransitionClick = true;
            pendingTransition = () => sceneTransition.Transition(scneneNumber);
            Debug.Log("Click to proceed with Transition");
        });

        dialogBehaviour.BindExternalFunction("Transition1", () => {
            waitingForTransition1Click = true;
            pendingTransition = () => sceneTransition1.Transition1(scneneNumber1);
            Debug.Log("Click to proceed with Transition1");
        });

        dialogBehaviour.BindExternalFunction("StartNewGame", () => {
            waitingForTransition1Click = true;
            gameManager.StartNewGame();
            Debug.Log("Click to proceed with StartNewGame");
        });

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