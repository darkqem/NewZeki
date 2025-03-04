using UnityEngine;
public class MovementStep2 : MonoBehaviour
{
    public MovementStep3 movementStep3;
    private float moveSpeed = 0.5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    
    public void StartMovement(Vector3 startPos)
    {
        int targetFloor = GetFloor(startPos.y);
        targetPosition = new Vector3(startPos.x, GetFloorY(targetFloor), startPos.z);
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
                movementStep3.StartMovement(targetPosition);
            }
        }
    }
    
    private int GetFloor(float yPosition)
    {
        if (yPosition < 8f) return 1;
        if (yPosition < 9.26f) return 2;
        if (yPosition < 10f) return 3;
        if (yPosition < 11.3f) return 4;
        return 5;
    }
    
    private float GetFloorY(int floor)
    {
        switch (floor)
        {
            case 1: return 7.084f;
            case 2: return 8.26f;
            case 3: return 9.453f;
            case 4: return 10.642f;
            case 5: return 11.83f;
            default: return 7.084f;
        }
    }
}