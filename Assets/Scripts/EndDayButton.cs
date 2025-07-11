using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EndDayButton : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Массив индексов сцен, которые будут показаны в разные дни")]
    [SerializeField] private int[] dailyScenes;
    
    [Header("Game Settings")]
    [Tooltip("Последний день игры")]
    [SerializeField] private int finalDay = 3;
    
    [Header("Transition Settings")]
    [Tooltip("Возвращаться ли к основной сцене после показа специальной сцены")]
    [SerializeField] private bool returnToMainScene = true;
    [SerializeField] private int mainSceneIndex = 1; // Индекс основной сцены игры
    [SerializeField] private float delayBeforeReturn = 2f; // Задержка перед возвратом к основной сцене
    
    private Button endDayButton;
    private bool isTransitioning = false;

    private void Awake()
    {
        endDayButton = GetComponent<Button>();
        if (endDayButton != null)
        {
            endDayButton.onClick.AddListener(OnEndDayClick);
        }
        else
        {
            Debug.LogError("EndDayButton: Компонент Button не найден!");
        }
    }

    private void OnEnable()
    {
        // Проверяем текущий день при активации
        if (GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
            // Если текущий день - последний, деактивируем кнопку
            if (currentDay >= finalDay)
            {
                if (endDayButton != null)
                {
                    endDayButton.interactable = false;
                }
                return;
            }
        }

        // В остальных случаях активируем кнопку
        if (endDayButton != null)
        {
            endDayButton.interactable = true;
        }
    }

    private void OnEndDayClick()
    {
        if (!isTransitioning && GameManager.Instance != null)
        {
            int currentDay = GameManager.Instance.GetCurrentDay();
            
            // Проверяем, не последний ли это день
            if (currentDay >= finalDay)
            {
                endDayButton.interactable = false;
                return;
            }

            // Деактивируем кнопку сразу после нажатия
            endDayButton.interactable = false;
            
            // Проверяем, есть ли сцена для текущего дня
            if (dailyScenes != null && currentDay <= dailyScenes.Length)
            {
                isTransitioning = true;
                StartCoroutine(ShowDailyScene(currentDay));
            }
            
            // Уведомляем GameManager о конце дня
            GameManager.Instance.EndDay();
        }
    }

    private IEnumerator ShowDailyScene(int currentDay)
    {
        // Получаем индекс сцены для текущего дня (массив начинается с 0, а дни с 1)
        int sceneIndex = dailyScenes[currentDay - 1];
        
        // Загружаем сцену для текущего дня
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneIndex);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        if (returnToMainScene)
        {
            // Ждем указанное время
            yield return new WaitForSeconds(delayBeforeReturn);

            // Возвращаемся к основной сцене
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.LoadScene(mainSceneIndex);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneIndex);
            }
        }

        isTransitioning = false;
    }

    // Метод для установки сцен для разных дней из других скриптов
    public void SetDailyScenes(int[] newDailyScenes)
    {
        dailyScenes = newDailyScenes;
    }

    // Метод для добавления сцены для следующего дня
    public void AddDailyScene(int sceneIndex)
    {
        if (dailyScenes == null)
        {
            dailyScenes = new int[] { sceneIndex };
        }
        else
        {
            int[] newSequence = new int[dailyScenes.Length + 1];
            dailyScenes.CopyTo(newSequence, 0);
            newSequence[newSequence.Length - 1] = sceneIndex;
            dailyScenes = newSequence;
        }
    }

    // Публичный метод для ручной активации кнопки
    public void EnableButton()
    {
        if (endDayButton != null && GameManager.Instance != null)
        {
            // Проверяем, не последний ли день перед активацией
            if (GameManager.Instance.GetCurrentDay() >= finalDay)
            {
                endDayButton.interactable = false;
                return;
            }
            endDayButton.interactable = true;
        }
    }
} 