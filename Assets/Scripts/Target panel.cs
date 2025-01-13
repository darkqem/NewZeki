using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject panel; // Привязанный в инспекторе панель
    public GameObject Player;
    
    public bool isMoving = false;
    public Button showButton; // Привязанный в инспекторе кнопка
    public bool isPanelVisible = false;
    private CurrentPosition currentPositionScript;
    public Vector3 targetPosition;

     

    void Start()
    {
        // Инициализация кнопки
        showButton.onClick.AddListener(ShowPanel);
        currentPositionScript = Player.GetComponent<CurrentPosition>();
        
    }
    void Update()
    {
        // Если движение активно, перемещаем персонажа
        if (isMoving)
        {
            currentPositionScript.Movement(targetPosition);

            // Проверяем, достиг ли персонаж целевой точки
            /*if (Vector3.Distance(Player.transform.position, targetPosition) < 0.01f)
            {
                isMoving = false; // Останавливаем движение
            }*/
            
        }
    }

    void ShowPanel()
    {
        if (!isPanelVisible)
        {
            panel.SetActive(true);
            isPanelVisible = true;

            targetPosition = panel.transform.position;

            // Запускаем движение персонажа
            isMoving = true;
        }
        else
        {
            panel.SetActive(false);
            isPanelVisible = false;
            isMoving = false;
            currentPositionScript.AnimateIdle();
        }
    }

    
}
