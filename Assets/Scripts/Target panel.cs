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
    private CharacterMover characterMoverScript;
    private MovementStep1 movementStep1;
    public Vector3 targetPosition;

     

    void Start()
    {
        // Инициализация кнопки
        showButton.onClick.AddListener(ShowPanel);
        currentPositionScript = Player.GetComponent<CurrentPosition>();
        movementStep1 = Player.GetComponent<MovementStep1>();
        characterMoverScript = Player.GetComponent<CharacterMover>();
        
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
        if (!isPanelVisible)
        {
            panel.SetActive(true);
            isPanelVisible = true;

            targetPosition = panel.transform.position;
            characterMoverScript.MoveTo(targetPosition);
            // Запускаем движение персонажа
            
        }
        else
        {
            panel.SetActive(false);
            isPanelVisible = false;
            
            //currentPositionScript.AnimateIdle();
        }
    }

    
}
