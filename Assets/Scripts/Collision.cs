using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    public float detectionRadius = 10f; // Радиус обнаружения объектов
    public float avoidanceSpeed = 5f; // Скорость избегания объектов
    public LayerMask obstacleLayers; // Маска слоев для обнаружения объектов

    private Vector3 lastPosition; // Последнее известное положение камеры
    private bool isAvoiding = false;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentPosition = transform.position;
        
        if (IsNearObstacle(currentPosition))
        {
            AvoidObstacle();
        }
        else
        {
            isAvoiding = false;
        }

        lastPosition = currentPosition;
    }

    bool IsNearObstacle(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, detectionRadius, obstacleLayers);
        
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
            {
                return true;
            }
        }
        
        return false;
    }

    void AvoidObstacle()
    {
        if (!isAvoiding)
        {
            isAvoiding = true;
            Vector3 directionToAvoid = lastPosition - transform.position;
            directionToAvoid.y = 0; // Ограничиваем движение только по горизонтали
            transform.Translate(directionToAvoid.normalized * avoidanceSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AvoidObstacle();
        }
    }
}
