using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DayDisplay : MonoBehaviour
{
    private TextMeshProUGUI dayText;
    private Animator animator; // Опционально, если захотите добавить анимацию
    
    [SerializeField] private string dayFormat = "День {0}"; // Формат отображения дня

    private void Start()
    {
        dayText = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>(); // Опционально
        
        // Подписываемся на события изменения дня
        GameManager.Instance.onNewDay.AddListener(UpdateDayDisplay);
        
        // Показываем текущий день при старте
        UpdateDayDisplay();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.onNewDay.RemoveListener(UpdateDayDisplay);
        }
    }

    private void UpdateDayDisplay()
    {
        int currentDay = GameManager.Instance.GetCurrentDay();
        dayText.text = string.Format(dayFormat, currentDay);
        
        // Опционально: воспроизводим анимацию при смене дня
        if (animator != null)
        {
            animator.SetTrigger("DayChange");
        }
    }
} 