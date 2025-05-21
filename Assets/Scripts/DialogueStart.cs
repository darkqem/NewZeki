using UnityEngine;
using cherrydev;

public class DialogStart : MonoBehaviour
{
    [SerializeField] private DialogBehaviour dialogBehaviour;
    [SerializeField] private DialogNodeGraph[] dialogGraphs; // Массив графов диалогов
    
    private void Start()
    {
        // Подписываемся на событие нового дня
        GameManager.Instance.onNewDay.AddListener(OnNewDay);
        
        // Проверяем, можно ли показать диалог
        if (GameManager.Instance.CanShowDialog())
        {
            ShowNextDialog();
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении объекта
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onNewDay.RemoveListener(OnNewDay);
        }
    }

    private void OnNewDay()
    {
        // Автоматически показываем диалог в начале нового дня
        if (GameManager.Instance.CanShowDialog())
        {
            ShowNextDialog();
        }
    }

    // Метод для показа следующего диалога
    public void ShowNextDialog()
    {
        if (!GameManager.Instance.CanShowDialog())
        {
            Debug.Log("Диалог на сегодня уже был показан");
            return;
        }

        if (dialogGraphs == null || dialogGraphs.Length == 0)
        {
            Debug.LogError("No dialog graphs assigned!");
            return;
        }

        // Получаем последний показанный индекс
        int currentDialogIndex = GameManager.Instance.GetLastShownDialogIndex();
        currentDialogIndex++;

        // Если дошли до конца массива, начинаем сначала
        if (currentDialogIndex >= dialogGraphs.Length)
        {
            currentDialogIndex = 0;
        }

        // Проверяем, что текущий граф существует
        if (dialogGraphs[currentDialogIndex] != null)
        {
            try
            {
                dialogBehaviour.StartDialog(dialogGraphs[currentDialogIndex]);
                GameManager.Instance.MarkDialogAsShown(currentDialogIndex);
                Debug.Log($"Показан диалог {currentDialogIndex} в день {GameManager.Instance.GetCurrentDay()}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error starting dialog: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning($"Dialog graph at index {currentDialogIndex} is not assigned!");
        }
    }

    // Метод для показа конкретного диалога по индексу
    public void ShowDialogByIndex(int index)
    {
        Debug.Log($"Attempting to show dialog at specific index: {index}");

        if (dialogGraphs == null || dialogGraphs.Length == 0)
        {
            Debug.LogError("No dialog graphs assigned!");
            return;
        }

        if (index < 0 || index >= dialogGraphs.Length)
        {
            Debug.LogError($"Dialog index {index} is out of range!");
            return;
        }

        if (GameManager.Instance.CanShowDialog())
        {
            try
            {
                dialogBehaviour.StartDialog(dialogGraphs[index]);
                Debug.Log($"Successfully started dialog at index {index}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error starting dialog: {e.Message}");
            }
        }
        else
        {
            Debug.Log("Диалог на сегодня уже был показан");
        }
    }
}