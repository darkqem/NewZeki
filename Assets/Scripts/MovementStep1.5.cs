using UnityEngine;
public class MovementStep1_5 : MonoBehaviour
{
    public MovementStep2 movementStep2;
    private float moveSpeed = 0.5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    
    void Start()
    {}
    public void StartMovement(Vector3 startPos)
    {
        targetPosition = new Vector3(-0.717f, 7.084f, 10.197f);
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
                movementStep2.StartMovement(targetPosition);
            }
        }
    }
    
    
}