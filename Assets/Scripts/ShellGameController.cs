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
    public float liftHeight = 2f; // Высота подъема напёрстков
    public float liftSpeed = 3f;  // Скорость подъема
    public Button playAgainButton; // Кнопка "Сыграть снова"

    [Header("Результат игры")]
    public GameObject winObject; // Объект для показа победы
    public GameObject loseObject; // Объект для показа поражения

    private const int WINNING_SHELL_INDEX = 1; // Всегда выигрышный напёрсток с индексом 1
    private bool canChoose = false;
    private bool isMoving = false;
    private Vector3[] originalPositions; // Сохраняем исходные позиции напёрстков
    private Vector3 ballPosition; // Позиция шарика

    void Start()
    {
        originalPositions = new Vector3[shells.Length];
        for (int i = 0; i < shells.Length; i++)
        {
            originalPositions[i] = shells[i].transform.position;
        }
        
        // Настраиваем кнопку "Сыграть снова"
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(StartGame);
            playAgainButton.gameObject.SetActive(false); // Скрываем кнопку в начале игры
        }
        
        // Скрываем объекты результата
        if (winObject != null) winObject.SetActive(false);
        if (loseObject != null) loseObject.SetActive(false);
        
        StartGame();
    }

    public void StartGame()
    {
        // Скрываем объекты результата при начале новой игры
        if (winObject != null) winObject.SetActive(false);
        if (loseObject != null) loseObject.SetActive(false);
        
        // Скрываем кнопку "Сыграть снова" при начале новой игры
        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(false);
        }
        
        // Возвращаем напёрстки в исходные позиции
        for (int i = 0; i < shells.Length; i++)
        {
            shells[i].transform.position = originalPositions[i];
        }
        
        // Позиционируем шарик под центральным напёрстком (индекс 1)
        PositionBallUnderShell();
        
        StartCoroutine(LowerShellsAndStartGame());
    }

    void PositionBallUnderShell()
    {
        // Устанавливаем шарик под центральный напёрсток (индекс 1)
        ball.transform.position = new Vector3(
            shells[WINNING_SHELL_INDEX].transform.position.x,
            ball.transform.position.y,
            shells[WINNING_SHELL_INDEX].transform.position.z
        );
        
        // Запоминаем позицию шарика
        ballPosition = ball.transform.position;
        
        // Показываем шарик
        ball.SetActive(true);
    }

    IEnumerator LowerShellsAndStartGame()
    {
        // Опускаем напёрстки
        yield return StartCoroutine(LowerShells());
        
        // Начинаем перемешивание
        StartCoroutine(ShuffleShells());
    }
    
    IEnumerator LowerShells()
    {
        Vector3[] startPositions = new Vector3[shells.Length];
        Vector3[] targetPositions = new Vector3[shells.Length];

        for (int i = 0; i < shells.Length; i++)
        {
            startPositions[i] = shells[i].transform.position;
            
            // Вычисляем расстояние от центра стаканчика до шарика
            float distanceToBall = startPositions[i].y - ballPosition.y;
            
            // Опускаем стаканчик на половину расстояния до шарика
            targetPositions[i] = new Vector3(
                startPositions[i].x,
                startPositions[i].y - (distanceToBall / 2),
                startPositions[i].z
            );
        }

        float elapsedTime = 0f;
        // Используем среднее расстояние для расчета продолжительности
        float averageDistance = Mathf.Abs(startPositions[0].y - targetPositions[0].y);
        float lowerDuration = averageDistance / liftSpeed;

        while (elapsedTime < lowerDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lowerDuration;
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
        
        yield return new WaitForSeconds(0.5f);
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
        
        // Если перемещается выигрышный напёрсток, запоминаем начальную и конечную позиции для шарика
        Vector3 ballStart = ball.transform.position;
        Vector3 ballTarget = ballStart;
        
        if (indexA == WINNING_SHELL_INDEX)
        {
            ballTarget = new Vector3(positionB.x, ballStart.y, positionB.z);
        }
        else if (indexB == WINNING_SHELL_INDEX)
        {
            ballTarget = new Vector3(positionA.x, ballStart.y, positionA.z);
        }

        while (elapsedTime < swapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / swapDuration;

            shells[indexA].transform.position = Vector3.Lerp(shellAStart, positionB, t);
            shells[indexB].transform.position = Vector3.Lerp(shellBStart, positionA, t);
            
            // Если перемещается выигрышный напёрсток, двигаем шарик вместе с ним
            if (indexA == WINNING_SHELL_INDEX || indexB == WINNING_SHELL_INDEX)
            {
                ball.transform.position = Vector3.Lerp(ballStart, ballTarget, t);
            }

            yield return null;
        }

        shells[indexA].transform.position = positionB;
        shells[indexB].transform.position = positionA;
        
        // Убеждаемся, что шарик достиг конечной позиции
        if (indexA == WINNING_SHELL_INDEX || indexB == WINNING_SHELL_INDEX)
        {
            ball.transform.position = ballTarget;
        }
        
        isMoving = false;
    }

    IEnumerator ShuffleShells()
    {
        canChoose = false;
        
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

        // Поднимаем все напёрстки
        yield return StartCoroutine(LiftAllShells());
        
        // Показываем шарик под центральным напёрстком (индекс 1)
        PositionBallUnderShell();

        // Показываем результат
        if (chosenIndex == WINNING_SHELL_INDEX)
        {
            if (winObject != null) winObject.SetActive(true);
            if (loseObject != null) loseObject.SetActive(false);
            Debug.Log("Угадал! Шарик был под напёрстком " + chosenIndex);
        }
        else
        {
            if (loseObject != null) loseObject.SetActive(true);
            if (winObject != null) winObject.SetActive(false);
            Debug.Log("Не угадал! Шарик был под напёрстком " + WINNING_SHELL_INDEX);
        }

        // Показываем кнопку "Сыграть снова"
        if (playAgainButton != null)
        {
            playAgainButton.gameObject.SetActive(true);
        }
    }

    IEnumerator LiftAllShells()
    {
        // Сохраняем начальные позиции
        Vector3[] startPositions = new Vector3[shells.Length];
        Vector3[] targetPositions = new Vector3[shells.Length];

        for (int i = 0; i < shells.Length; i++)
        {
            startPositions[i] = shells[i].transform.position;
            targetPositions[i] = new Vector3(
                shells[i].transform.position.x,
                originalPositions[i].y + liftHeight,
                shells[i].transform.position.z
            );
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
}