using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PrisonerFilesManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject filesPanel; // Панель с досье
    [SerializeField] private Image fileImage; // Компонент Image для отображения досье
    [SerializeField] private Button nextButton; // Кнопка следующего досье
    [SerializeField] private Button previousButton; // Кнопка предыдущего досье
    [SerializeField] private TextMeshProUGUI pageNumberText; // Текст с номером текущей страницы
    [SerializeField] private Button hudToggleButton; // Кнопка в HUD для открытия/закрытия досье
    
    [Header("Files Settings")]
    [SerializeField] private Sprite[] prisonerFiles; // Массив спрайтов досье
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab; // Клавиша для открытия/закрытия досье
    [SerializeField] private bool useKeyboardToggle = true; // Использовать ли клавишу для переключения
    
    private int currentFileIndex = 0; // Индекс текущего досье
    private bool isFilesOpen = false; // Открыто ли досье

    private void Start()
    {
        // Проверяем наличие всех необходимых компонентов
        if (filesPanel == null || fileImage == null)
        {
            Debug.LogError("PrisonerFilesManager: Не все компоненты назначены!");
            return;
        }

        // Скрываем панель при старте
        filesPanel.SetActive(false);

        // Настраиваем кнопки навигации
        if (nextButton != null)
            nextButton.onClick.AddListener(ShowNextFile);
        
        if (previousButton != null)
            previousButton.onClick.AddListener(ShowPreviousFile);

        // Настраиваем кнопку в HUD
        if (hudToggleButton != null)
            hudToggleButton.onClick.AddListener(ToggleFiles);

        // Обновляем UI
        UpdateUI();
    }

    private void Update()
    {
        // Проверяем нажатие клавиши для открытия/закрытия досье только если включено управление с клавиатуры
        if (useKeyboardToggle && Input.GetKeyDown(toggleKey))
        {
            ToggleFiles();
        }
    }

    // Открыть/закрыть досье
    public void ToggleFiles()
    {
        if (prisonerFiles == null || prisonerFiles.Length == 0)
        {
            Debug.LogWarning("PrisonerFilesManager: Нет доступных досье!");
            return;
        }

        isFilesOpen = !isFilesOpen;
        filesPanel.SetActive(isFilesOpen);

        if (isFilesOpen)
        {
            UpdateUI();
        }
    }

    // Показать следующее досье
    public void ShowNextFile()
    {
        if (prisonerFiles == null || prisonerFiles.Length == 0) return;

        currentFileIndex++;
        if (currentFileIndex >= prisonerFiles.Length)
        {
            currentFileIndex = 0; // Переходим к первому досье если достигли конца
        }

        UpdateUI();
    }

    // Показать предыдущее досье
    public void ShowPreviousFile()
    {
        if (prisonerFiles == null || prisonerFiles.Length == 0) return;

        currentFileIndex--;
        if (currentFileIndex < 0)
        {
            currentFileIndex = prisonerFiles.Length - 1; // Переходим к последнему досье если достигли начала
        }

        UpdateUI();
    }

    // Обновление UI
    private void UpdateUI()
    {
        if (prisonerFiles != null && prisonerFiles.Length > 0)
        {
            fileImage.sprite = prisonerFiles[currentFileIndex];
            
            if (pageNumberText != null)
            {
                pageNumberText.text = $"Досье {currentFileIndex + 1}/{prisonerFiles.Length}";
            }

            // Обновляем доступность кнопок навигации
            if (nextButton != null)
                nextButton.interactable = prisonerFiles.Length > 1;
            
            if (previousButton != null)
                previousButton.interactable = prisonerFiles.Length > 1;
        }
    }

    // Метод для добавления нового досье
    public void AddPrisonerFile(Sprite newFile)
    {
        if (prisonerFiles == null)
        {
            prisonerFiles = new Sprite[] { newFile };
        }
        else
        {
            Sprite[] newFiles = new Sprite[prisonerFiles.Length + 1];
            prisonerFiles.CopyTo(newFiles, 0);
            newFiles[newFiles.Length - 1] = newFile;
            prisonerFiles = newFiles;
        }
        UpdateUI();
    }

    // Метод для установки всех досье разом
    public void SetPrisonerFiles(Sprite[] newFiles)
    {
        prisonerFiles = newFiles;
        currentFileIndex = 0;
        UpdateUI();
    }
} 