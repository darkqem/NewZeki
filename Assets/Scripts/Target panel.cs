using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    public GameObject panel; // Привязанный в инспекторе панель
    public GameObject Player;
    
    public bool isMoving = false;
    public Button showButton; // Привязанный в инспекторе кнопка
    public bool isPanelVisible = false;
    private CharacterMover characterMoverScript;
    public Vector3 targetPosition;
    private static bool isAnyPanelActive = false; // Флаг для отслеживания активных панелей
    private static List<PanelManager> allPanels = new List<PanelManager>();

    void Start()
    {
        // Инициализация кнопки
        showButton.onClick.AddListener(ShowPanel);
        characterMoverScript = Player.GetComponent<CharacterMover>();
        
        // Добавляем эту панель в список всех панелей
        if (!allPanels.Contains(this))
        {
            allPanels.Add(this);
        }
    }

    void OnDestroy()
    {
        // Удаляем панель из списка при уничтожении
        allPanels.Remove(this);
    }

    void Update()
    {
        // Если движение активно, перемещаем персонажа
        if (isMoving)
        {
            //currentPositionScript.Movement(targetPosition);
            
            


            // Проверяем, достиг ли персонаж целевой точки
            /*if (Vector3.Distance(Player.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // Останавливаем движение
            }*/
            
        }
    }

    void ShowPanel()
    {
        // Если панель уже видима, закрываем её
        if (isPanelVisible)
        {
            ClosePanel();
            return;
        }

        // Если другая панель активна или персонаж двигается, игнорируем нажатие
        if (isAnyPanelActive || characterMoverScript.isMoving)
        {
            return;
        }

        // Активируем панель и отправляем персонажа к ней
        panel.SetActive(true);
        isPanelVisible = true;
        isAnyPanelActive = true;
        targetPosition = panel.transform.position;
        characterMoverScript.MoveTo(targetPosition);
    }

    public void ClosePanel()
    {
        if (isPanelVisible)
        {
            panel.SetActive(false);
            isPanelVisible = false;
            isAnyPanelActive = false;
            
            // Если персонаж находится у этой панели, отправляем его обратно
            if (Vector3.Distance(Player.transform.position, targetPosition) < 0.5f)
            {
                characterMoverScript.ReturnToStart();
            }
        }
    }

    // Статический метод для сброса состояния всех панелей
    public static void ResetAllPanels()
    {
        isAnyPanelActive = false;
        foreach (var panel in allPanels)
        {
            if (panel != null)
            {
                panel.ResetPanel();
            }
        }
        allPanels.Clear();
    }

    // Метод для сброса состояния конкретной панели
    private void ResetPanel()
    {
        isPanelVisible = false;
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}
