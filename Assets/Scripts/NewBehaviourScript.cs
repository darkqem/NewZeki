
using UnityEngine;
using UnityEngine.U2D.Animation;
public class CurrentPosition : MonoBehaviour
{
    private float moveSpeed = 0.5f;
    public GameObject cameraMoveOnTrigger;

    public Sprite[] walkSprites; // Массив спрайтов для ходьбы
    public Sprite[] idleSprites;
    public SpriteRenderer spriteRenderer;
    private int currentFrame = 0; // Текущий кадр анимации
    private float frameRate = 0.1f; // Скорость смены кадров
    private float frameTimer = 0f; // Таймер для анимации

    private bool isMoving = false; // Флаг движения

    public PanelManager panelManagerScript;
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
    private void Update()
    {
        if (isMoving)
        {
            AnimateWalk();
        }
        else
        {
            AnimateIdle();
        }
    }

    private int GetFloor(float yPosition)
    {
        if (yPosition < 8f) return 1; // Первый этаж
        if (yPosition < 9.26f & yPosition > 8f) return 2; // Второй этаж
        if (yPosition < 10f & yPosition > 9.26f) return 3; // Третий этаж
        if (yPosition < 11.3f & yPosition > 10f) return 4; // Четвёртый этаж
        return 5; // Пятый этаж
    }

    private float GetFloorY(int floor)
    {
        switch (floor)
        {
            case 1: return 7.084f; // Первый этаж
            case 2: return 8.26f; // Второй этаж
            case 3: return 9.453f; // Третий этаж
            case 4: return 10.642f; // Четвёртый этаж
            case 5: return 11.83f; // Пятый этаж
            default: return 7.084f; // По умолчанию первый этаж
        }
    }

    public void Movement(Vector3 targetPosition)
    {
        // Перемещаем объект к целевой позиции
        targetPosition.z = 7.63f;

        float currentY = transform.position.y;

        int targetFloor = GetFloor(targetPosition.y);
        float targetY = GetFloorY(targetFloor);
        targetPosition.y = GetFloorY(targetFloor);

        Vector3 centerCurrentFloor = new Vector3(-0.646f, currentY, 7.63f);

        Vector3 targetFloor_1 = new Vector3(0,0, 0);
        targetFloor_1 = targetPosition;
        targetFloor_1.y = targetY;
        

        if (targetPosition.x == -1.5f)
        {
            spriteRenderer.flipX = true;
            targetPosition.x = -1.5f;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (targetPosition.x < -0.6f)
        {
            targetPosition.x = -1.2f;
        }
        else
        {
            targetPosition.x = -0.095f;
        }

        if(targetFloor == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        if (targetFloor > 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        //движ камеры
        float CameraMoveSpeed = 2F;
        Vector3 cameraTargetPosition = transform.position;
        cameraTargetPosition.z = 7f;
        cameraMoveOnTrigger.transform.position = Vector3.MoveTowards(cameraMoveOnTrigger.transform.position, cameraTargetPosition, CameraMoveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) > 0.05f)
        {
            isMoving = true; // Включаем анимацию движения
        }
        else
        {
            // Останавливаем движение и переключаем на Idle
            isMoving = false;
            //AnimateIdle(); 
        }

    }

    private void AnimateWalk()
    {
        // Обновляем таймер
        frameTimer += Time.deltaTime;

        // Меняем кадр, если прошло достаточно времени
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f; // Сбрасываем таймер
            currentFrame = (currentFrame + 1) % walkSprites.Length; // Переходим к следующему кадру
            spriteRenderer.sprite = walkSprites[currentFrame]; // Устанавливаем текущий кадр
        }
    }

    public void AnimateIdle()
    {
        frameTimer += Time.deltaTime;

        // Меняем кадр, если прошло достаточно времени
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f; // Сбрасываем таймер
            currentFrame = (currentFrame + 1) % idleSprites.Length; // Переходим к следующему кадру
            spriteRenderer.sprite = idleSprites[currentFrame]; // Устанавливаем текущий кадр
        }
    }
}