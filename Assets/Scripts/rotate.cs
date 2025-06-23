using UnityEngine;

public class TiltObject : MonoBehaviour
{
    public float tiltAngle = 10f;   // Максимальный угол наклона
    public float speed = 1f;        // Скорость наклона
    private float targetAngle = 0f; // Целевой угол наклона
    private float currentAngle = 0f; // Текущий угол наклона

    private void Update()
    {
        // Плавно изменяем угол наклона
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);

        // Применяем текущий угол наклона
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        // Меняем направление наклона, если достигнут предел
        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            // Меняем направление наклона
            if (targetAngle == -tiltAngle)
                targetAngle = tiltAngle;
            else
                targetAngle = -tiltAngle;
        }
    }
}
