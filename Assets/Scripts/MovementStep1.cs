using UnityEngine;

public class MovementStep1 : MonoBehaviour
{
    public GameObject cameraMoveOnTrigger;
    public MovementStep1_5 movementStep1_5;
    private float moveSpeed = 0.5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    public PanelManager panelManagerScript;
    
    public SpriteRenderer spriteRenderer;
    void Start()
    {
        Vector3 startposition;
        startposition.x = 1.183f;
        startposition.y = 7.084f;
        startposition.z = 8.248f;
        transform.position = startposition;
        // Получаем Sprite Resolver на объекте персонажа
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void StartMovement(Vector3 finalTarget)
    {
        // Определяем первую контрольную точку
        if (finalTarget.y < 7.892f)
            targetPosition = new Vector3(-1.2f, 7.084f, 7.63f);
        else
            targetPosition = new Vector3(-0.717f, 7.084f, 9.247f);
        
        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
                movementStep1_5.StartMovement(targetPosition);
            }
        }
    }
}
