using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(1.183f, 7.084f, 8.248f);
    public Vector3 elevatorPosition = new Vector3(-0.717f, 7.084f, 10.125f);
    public Vector3 doorPosition = new Vector3(0.563f, 7.084f, 8.248f);
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
    private bool isReturning = false;
    private Stack<Vector3> movementPath = new Stack<Vector3>(); // Стек для хранения пути

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
        if (isMoving || isReturning)
        {
            return;
        }

        // Очищаем стек пути перед новым движением
        movementPath.Clear();
        // Добавляем стартовую позицию как первую точку возврата
        movementPath.Push(transform.position);

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

    public void ReturnToStart()
    {
        if (!isReturning && !isMoving)
        {
            isReturning = true;
            StartCoroutine(ReturnRoutine());
        }
    }

    private IEnumerator ReturnRoutine()
    {
        while (movementPath.Count > 0)
        {
            Vector3 nextPosition = movementPath.Pop();
            yield return StartCoroutine(MoveToPosition(nextPosition));
        }

        isReturning = false;
        isMoving = false;
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition)
    {
        int currentFloor = GetFloor(transform.position.y);
        int targetFloor = GetFloor(targetPosition.y);
        float floorY = GetFloorY(targetFloor);

        // Если мы начинаем из стартовой позиции
        if (Vector3.Distance(transform.position, startPosition) < 0.05f)
        {
            movementPath.Push(doorPosition);
            yield return StartCoroutine(MoveToPosition(doorPosition));
        }

        if (currentFloor == targetFloor)
        {
            Vector3 finalPosition = new Vector3(targetPosition.x, floorY, targetPosition.z);
            movementPath.Push(finalPosition);
            yield return StartCoroutine(MoveToPosition(finalPosition));
        }
        else
        {
            // Двигаемся к лифту
            Vector3 elevatorPos = new Vector3(elevatorPosition.x, transform.position.y, elevatorPosition.z);
            movementPath.Push(elevatorPos);
            yield return StartCoroutine(MoveToPosition(elevatorPos));
            
            // Поднимаемся на нужный этаж
            Vector3 elevatorTarget = new Vector3(elevatorPosition.x, floorY, elevatorPosition.z);
            movementPath.Push(elevatorTarget);
            yield return StartCoroutine(MoveToPosition(elevatorTarget));
            
            // Двигаемся к целевой точке
            Vector3 finalPosition = new Vector3(targetPosition.x, floorY, targetPosition.z);
            movementPath.Push(finalPosition);
            yield return StartCoroutine(MoveToPosition(finalPosition));
        }

        isMoving = false;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            if (target.x < transform.position.x)
            {
                spriteRenderer.flipX = true;
            }
            else if (target.x > transform.position.x)
            {
                spriteRenderer.flipX = false;
            }
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            float CameraMoveSpeed = 2F;
            Vector3 cameraTargetPosition = transform.position;
            cameraTargetPosition.z = 7f;
            cameraMoveOnTrigger.transform.position = Vector3.MoveTowards(cameraMoveOnTrigger.transform.position, cameraTargetPosition, CameraMoveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
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
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[currentFrame];
        }
    }

    private void AnimateIdle()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % idleSprites.Length;
            spriteRenderer.sprite = idleSprites[currentFrame];
        }
    }
}
