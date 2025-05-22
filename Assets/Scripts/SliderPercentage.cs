using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderPercentage : MonoBehaviour
{
    [SerializeField] private Slider slider;              // Ссылка на слайдер
    [SerializeField] private TextMeshProUGUI percentText; // Текст для отображения процента
    [SerializeField] private string suffix = "%";        // Суффикс (например, "%")
    [SerializeField] private bool showDecimals = false;  // Показывать ли десятичные знаки

    private void Start()
    {
        // Если слайдер не назначен, пробуем найти его на том же объекте
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        // Если текст не назначен, пробуем найти его в дочерних объектах
        if (percentText == null)
        {
            percentText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Проверяем, что все необходимые компоненты найдены
        if (slider != null)
        {
            // Подписываемся на событие изменения значения слайдера
            slider.onValueChanged.AddListener(UpdatePercentageText);
            
            // Обновляем текст с начальным значением
            UpdatePercentageText(slider.value);
        }
        else
        {
            Debug.LogError("Slider not found! Please assign a slider in the inspector or attach this script to a GameObject with a Slider component.");
        }
    }

    private void UpdatePercentageText(float value)
    {
        if (percentText != null)
        {
            // Конвертируем значение слайдера (0-1) в проценты (0-100)
            float percentage = value * 100f;

            // Форматируем текст в зависимости от настройки десятичных знаков
            string percentageText = showDecimals 
                ? percentage.ToString("F1") // Один знак после запятой
                : Mathf.RoundToInt(percentage).ToString(); // Округляем до целого

            // Добавляем суффикс
            percentText.text = percentageText + suffix;
        }
    }

    // Публичный метод для обновления значения извне
    public void SetValue(float value)
    {
        if (slider != null)
        {
            slider.value = Mathf.Clamp01(value); // Ограничиваем значение от 0 до 1
        }
    }

    // Публичный метод для получения текущего значения
    public float GetValue()
    {
        return slider != null ? slider.value : 0f;
    }

    // Публичный метод для получения текущего процента
    public float GetPercentage()
    {
        return slider != null ? slider.value * 100f : 0f;
    }
} 