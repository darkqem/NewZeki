using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShellGameController : MonoBehaviour
{
    public GameObject[] shells; // 0, 1, 2 — напёрстки
    public GameObject ball;     // шарик
    public float shuffleTime = 1.5f; // время перемешивания
    public int shuffleCount = 5;     // сколько раз перемешать
    public float moveSpeed = 5f; // Скорость перемещения объектов
    public float ballOffset = 0.5f; // Расстояние от напёрстка до шарика
    public float liftHeight = 2f; // Высота подъема напёрстков
    public float liftSpeed = 3f;  // Скорость подъема

    private bool[] shellHasBall; // Флаги для каждого напёрстка
    private bool canChoose = false;
    private bool isMoving = false;
    private Vector3[] originalPositions; // Сохраняем исходные позиции напёрстков

    void Start()
    {
        shellHasBall = new bool[shells.Length];
        originalPositions = new Vector3[shells.Length];
        for (int i = 0; i < shells.Length; i++)
        {
            originalPositions[i] = shells[i].transform.position;
        }
        StartGame();
    }

    public void StartGame()
    {
        // Возвращаем напёрстки в исходные позиции
        for (int i = 0; i < shells.Length; i++)
        {
            shells[i].transform.position = originalPositions[i];
            shellHasBall[i] = false; // Сбрасываем все флаги
        }

        // Случайно выбираем напёрсток для шарика и устанавливаем его флаг
        int randomShell = Random.Range(0, shells.Length);
        shellHasBall[randomShell] = true;
        
        StartCoroutine(PlaceBallSmooth(randomShell));
    }

    Vector3 GetBallPositionUnderShell(int shellIndex)
    {
        // Позиционируем шарик чуть ниже напёрстка
        return new Vector3(
            shells[shellIndex].transform.position.x,
            shells[shellIndex].transform.position.y - ballOffset,
            shells[shellIndex].transform.position.z
        );
    }

    IEnumerator PlaceBallSmooth(int shellIndex)
    {
        Vector3 targetPosition = GetBallPositionUnderShell(shellIndex);
        ball.SetActive(true);
        
        while (Vector3.Distance(ball.transform.position, targetPosition) > 0.01f)
        {
            ball.transform.position = Vector3.MoveTowards(
                ball.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        ball.transform.position = targetPosition;
        StartCoroutine(ShuffleShells());
    }

    IEnumerator SwapShellsSmooth(int indexA, int indexB)
    {
        isMoving = true;
        Vector3 positionA = shells[indexA].transform.position;
        Vector3 positionB = shells[indexB].transform.position;
        
        float elapsedTime = 0f;
        float swapDuration = Vector3.Distance(positionA, positionB) / moveSpeed; // Рассчитываем время перемещения

        Vector3 shellAStart = shells[indexA].transform.position;
        Vector3 shellBStart = shells[indexB].transform.position;
        
        // Находим под каким напёрстком шарик
        int ballShellIndex = -1;
        for (int i = 0; i < shellHasBall.Length; i++)
        {
            if (shellHasBall[i])
            {
                ballShellIndex = i;
                break;
            }
        }

        // Запоминаем начальную позицию шарика и вычисляем конечную
        Vector3 ballStart = ball.transform.position;
        Vector3 ballTarget = ballStart;
        
        // Если шарик под одним из перемещаемых напёрстков, двигаем его вместе с напёрстком
        if (ballShellIndex == indexA)
        {
            ballTarget = new Vector3(positionB.x, ballStart.y, ballStart.z);
        }
        else if (ballShellIndex == indexB)
        {
            ballTarget = new Vector3(positionA.x, ballStart.y, ballStart.z);
        }

        while (elapsedTime < swapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / swapDuration;

            shells[indexA].transform.position = Vector3.Lerp(shellAStart, positionB, t);
            shells[indexB].transform.position = Vector3.Lerp(shellBStart, positionA, t);

            if (ballShellIndex == indexA || ballShellIndex == indexB)
            {
                ball.transform.position = Vector3.Lerp(ballStart, ballTarget, t);
            }

            yield return null;
        }

        shells[indexA].transform.position = positionB;
        shells[indexB].transform.position = positionA;
        
        if (ballShellIndex == indexA || ballShellIndex == indexB)
        {
            ball.transform.position = ballTarget;
        }
        
        isMoving = false;
    }

    IEnumerator ShuffleShells()
    {
        canChoose = false;
        ball.SetActive(false); // Скрываем шарик на время перемешивания
        
        for (int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, shells.Length);
            int b;
            do { b = Random.Range(0, shells.Length); } while (b == a);

            yield return StartCoroutine(SwapShellsSmooth(a, b));
            yield return new WaitForSeconds(0.2f);
        }
        
        canChoose = true;
    }

    void Update()
    {
        if (!canChoose || isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click detected");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit something: " + hit.collider.gameObject.name);
                for (int i = 0; i < shells.Length; i++)
                {
                    if (hit.collider.gameObject == shells[i])
                    {
                        Debug.Log("Clicked on shell " + i);
                        StartCoroutine(RevealSmooth(i));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator RevealSmooth(int chosenIndex)
    {
        canChoose = false;
        ball.SetActive(true);

        // Поднимаем все напёрстки
        yield return StartCoroutine(LiftAllShells());

        // Показываем результат
        if (shellHasBall[chosenIndex])
        {
            Debug.Log("Угадал! Шарик был под напёрстком " + chosenIndex);
        }
        else
        {
            Debug.Log("Не угадал! Попробуй ещё раз");
        }

        yield return new WaitForSeconds(2f);
        StartGame();
    }

    IEnumerator LiftAllShells()
    {
        // Сохраняем начальные позиции
        Vector3[] startPositions = new Vector3[shells.Length];
        Vector3[] targetPositions = new Vector3[shells.Length];

        for (int i = 0; i < shells.Length; i++)
        {
            startPositions[i] = shells[i].transform.position;
            targetPositions[i] = startPositions[i] + Vector3.up * liftHeight;
        }

        float elapsedTime = 0f;
        float liftDuration = liftHeight / liftSpeed;

        // Поднимаем все напёрстки одновременно
        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / liftDuration;

            // Используем плавную интерполяцию для более естественного движения
            float smoothT = Mathf.SmoothStep(0, 1, t);

            for (int i = 0; i < shells.Length; i++)
            {
                shells[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], smoothT);
            }

            yield return null;
        }

        // Убеждаемся, что напёрстки достигли конечной позиции
        for (int i = 0; i < shells.Length; i++)
        {
            shells[i].transform.position = targetPositions[i];
        }
    }

    // Добавим метод для возврата напёрстков в исходное положение
    IEnumerator LowerAllShells()
    {
        Vector3[] startPositions = new Vector3[shells.Length];
        Vector3[] targetPositions = new Vector3[shells.Length];

        for (int i = 0; i < shells.Length; i++)
        {
            startPositions[i] = shells[i].transform.position;
            targetPositions[i] = originalPositions[i];
        }

        float elapsedTime = 0f;
        float liftDuration = liftHeight / liftSpeed;

        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / liftDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            for (int i = 0; i < shells.Length; i++)
            {
                shells[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], smoothT);
            }

            yield return null;
        }

        for (int i = 0; i < shells.Length; i++)
        {
            shells[i].transform.position = targetPositions[i];
        }
    }
}