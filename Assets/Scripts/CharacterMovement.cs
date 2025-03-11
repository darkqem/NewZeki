using System.Collections;
using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(1.183f, 7.084f, 8.248f);
    public Vector3 elevatorPosition = new Vector3(-0.717f, 7.084f, 10.125f);
    public PanelManager panelManagerScript;
    public GameObject cameraMoveOnTrigger;
    public Sprite[] walkSprites;
    public Sprite[] idleSprites;
    public SpriteRenderer spriteRenderer;
    private int currentFrame = 0;
    private float frameRate = 0.1f;
    private float frameTimer = 0f;
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

        if (transform.position == startPosition)
        {
            yield return StartCoroutine(MoveToPosition(new Vector3(0.597f, 7.084f, 8.248f)));
        }

        if (currentFloor == targetFloor)
        {
            AdjustSpriteDirection(targetPosition.x);
            yield return StartCoroutine(MoveToPosition(new Vector3(targetPosition.x, floorY, targetPosition.z)));
        }
        else
        {
            AdjustSpriteDirection(elevatorPosition.x);
            yield return StartCoroutine(MoveToPosition(new Vector3(elevatorPosition.x, transform.position.y, elevatorPosition.z)));
            
            Vector3 elevatorTarget = new Vector3(elevatorPosition.x, floorY, elevatorPosition.z);
            yield return StartCoroutine(MoveToPosition(elevatorTarget));
            
            AdjustSpriteDirection(targetPosition.x);
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
        transform.position = target;
    }

    private void AdjustSpriteDirection(float targetX)
    {
        spriteRenderer.flipX = targetX < transform.position.x;
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

    public void AnimateIdle()
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
