using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuPanel; // Панель меню паузы
    public GameObject hudPanel; // Панель HUD
    public KeyCode pauseKey = KeyCode.Escape; // Клавиша для паузы
    public string mainMenuSceneName = "MAIN MENU"; // Имя сцены главного меню
    
    // Массив всех кнопок HUD, которые нужно деактивировать при паузе
    public Button[] hudButtons;
    
    [Header("Panels to Hide")]
    public GameObject[] panelsToHide; // Массив панелей, которые нужно скрывать при паузе
    private bool[] panelStates; // Массив для хранения состояний панелей
    
    [Header("Transition Settings")]
    public Animator transitionAnimator; // Аниматор для перехода между сценами
    public float transitionTime = 1f; // Время анимации перехода
    
    private bool isPaused = false;
    private float previousTimeScale;
    private static readonly string TRANSITION_TRIGGER = "Start"; // Имя триггера анимации

    private void Start()
    {
        // Убедимся, что меню паузы скрыто при старте
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Если кнопки HUD не заданы вручную, найдем их автоматически
        if (hudButtons == null || hudButtons.Length == 0)
        {
            if (hudPanel != null)
            {
                hudButtons = hudPanel.GetComponentsInChildren<Button>(true);
            }
        }

        // Инициализируем массив состояний панелей
        if (panelsToHide != null)
        {
            panelStates = new bool[panelsToHide.Length];
        }
        
        previousTimeScale = Time.timeScale;
    }

    private void Update()
    {
        // Проверяем нажатие клавиши паузы
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // Переключение состояния паузы
    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Поставить игру на паузу
    public void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        isPaused = true;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Деактивируем все кнопки HUD
        SetHUDButtonsInteractable(false);

        // Сохраняем состояния и скрываем все панели
        if (panelsToHide != null)
        {
            for (int i = 0; i < panelsToHide.Length; i++)
            {
                if (panelsToHide[i] != null)
                {
                    panelStates[i] = panelsToHide[i].activeSelf;
                    panelsToHide[i].SetActive(false);
                }
            }
        }
    }

    // Возобновить игру
    public void ResumeGame()
    {
        Time.timeScale = previousTimeScale;
        isPaused = false;
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Активируем все кнопки HUD
        SetHUDButtonsInteractable(true);

        // Восстанавливаем состояния всех панелей
        if (panelsToHide != null)
        {
            for (int i = 0; i < panelsToHide.Length; i++)
            {
                if (panelsToHide[i] != null)
                {
                    panelsToHide[i].SetActive(panelStates[i]);
                }
            }
        }
    }

    // Метод для управления интерактивностью кнопок HUD
    private void SetHUDButtonsInteractable(bool interactable)
    {
        if (hudButtons != null)
        {
            foreach (Button button in hudButtons)
            {
                if (button != null)
                {
                    button.interactable = interactable;
                }
            }
        }

        // Если есть панель HUD, можно также управлять её прозрачностью
        if (hudPanel != null)
        {
            CanvasGroup canvasGroup = hudPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = interactable ? 1f : 0.5f; // Затемняем HUD при паузе
                canvasGroup.interactable = interactable;
                canvasGroup.blocksRaycasts = interactable;
            }
        }
    }

    // Открыть настройки
    public void OpenSettings()
    {
        Debug.Log("Opening Settings...");
    }

    // Выход в главное меню с анимацией перехода
    public void ReturnToMainMenu()
    {
        if (SceneTransitionManager.Instance != null)
        {
            // Восстанавливаем нормальную скорость времени перед переходом
            Time.timeScale = 1f;
            // Используем SceneTransitionManager для перехода
            SceneTransitionManager.Instance.LoadScene(0); // Предполагаем, что главное меню имеет индекс 0
        }
        else
        {
            // Запускаем старый вариант перехода если менеджер недоступен
            StartCoroutine(LoadMainMenuWithTransition());
        }
    }

    private System.Collections.IEnumerator LoadMainMenuWithTransition()
    {
        // Запускаем анимацию перехода
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger(TRANSITION_TRIGGER);
            
            // Ждем завершения анимации
            yield return new WaitForSecondsRealtime(transitionTime);
        }

        // Восстанавливаем нормальную скорость времени
        Time.timeScale = 1f;
        
        // Загружаем сцену главного меню
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Выход из игры (для standalone builds)
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 