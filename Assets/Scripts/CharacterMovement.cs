using System.Collections;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(1.183f, 7.084f, 8.248f);
    public Vector3 elevatorPosition = new Vector3(-0.717f, 7.084f, 10.125f);
    public PanelManager panelManagerScript;
    public GameObject cameraMoveOnTrigger;
    public Sprite[] walkSprites; // Массив спрайтов для ходьбы
    public Sprite[] idleSprites;
    public SpriteRenderer spriteRenderer;
    private int currentFrame = 0; // Текущий кадр анимации
    private float frameRate = 0.1f; // Скорость смены кадров
    private float frameTimer = 0f; // Таймер для анимации
    public float moveSpeed = 0.5f;
    public bool isMoving = false;

    private void Start()
    {
        transform.position = startPosition;
        AnimateIdle();
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

    public void MoveTo(Vector3 targetPosition)
    {
        targetPosition.z = 7.63f;
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
        StartCoroutine(MoveRoutine(targetPosition));
        

    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        int currentFloor = GetFloor(transform.position.y);
        int targetFloor = GetFloor(targetPosition.y);
        float floorY = GetFloorY(targetFloor);

        

        if (currentFloor == targetFloor)
        {
            yield return StartCoroutine(MoveToPosition(new Vector3(targetPosition.x, floorY, targetPosition.z)));
            
        }
        else
        {
            // Двигаемся к лифту
            yield return StartCoroutine(MoveToPosition(new Vector3(elevatorPosition.x, transform.position.y, elevatorPosition.z)));
            
            // Поднимаемся на нужный этаж
            Vector3 elevatorTarget = new Vector3(elevatorPosition.x, floorY, elevatorPosition.z);
            yield return StartCoroutine(MoveToPosition(elevatorTarget));
            
            // Двигаемся к целевой точке
            Vector3 finalPosition = new Vector3(targetPosition.x, floorY, targetPosition.z);
            yield return StartCoroutine(MoveToPosition(finalPosition));
            
        }

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            isMoving = true;
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            float CameraMoveSpeed = 2F;
            Vector3 cameraTargetPosition = transform.position;
            cameraTargetPosition.z = 7f;
            cameraMoveOnTrigger.transform.position = Vector3.MoveTowards(cameraMoveOnTrigger.transform.position, cameraTargetPosition, CameraMoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target; // Фиксируем точное положение
    }
    private int GetFloor(float yPosition)
    {
        if (yPosition < 8f) return 1;
        if (yPosition < 9.26f && yPosition > 8f) return 2;
        if (yPosition < 10f && yPosition > 9.26f) return 3;
        if (yPosition < 11.3f && yPosition > 10f) return 4;
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
