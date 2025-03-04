using UnityEngine;
public class MovementStep3 : MonoBehaviour
{
    private float moveSpeed = 0.5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    
    public void StartMovement(Vector3 startPos)
    {
        targetPosition = startPos;
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
            }
        }
    }
}