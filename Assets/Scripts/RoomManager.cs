using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [System.Serializable]
    public class Room
    {
        public GameObject roomObject;
        public Light roomLight;
        public bool isLit = false;
        public bool hasCorrectColor = false;
        [HideInInspector]
        public float blinkTimer = 0f;
        public Color originalColor;
    }

    public Room[] rooms;
    public int numberOfLitRooms = 5;
    public string wireGameSceneName = "WireGame";
    public float blinkSpeed = 1f; // Скорость мигания
    public Color targetColor = new Color(0.906f, 0.843f, 0.235f); // E7D73C в RGB
    public Color redColor = Color.red;

    private static int lastClickedRoom = -1;
    private static bool[] roomStates = null; // Сохраняем состояния комнат

    void Start()
    {
        // Сохраняем оригинальные цвета ламп
        foreach (Room room in rooms)
        {
            if (room.roomLight != null)
            {
                room.originalColor = room.roomLight.color;
            }
        }

        // Инициализируем массив состояний, если это первый запуск
        if (roomStates == null || roomStates.Length != rooms.Length)
        {
            InitializeRooms();
        }
        else
        {
            // Восстанавливаем сохраненные состояния
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].isLit = roomStates[i];
                if (rooms[i].roomLight != null)
                {
                    rooms[i].roomLight.enabled = roomStates[i];
                }
            }

            // Если вернулись из мини-игры, зажигаем последнюю нажатую комнату
            if (lastClickedRoom >= 0 && lastClickedRoom < rooms.Length)
            {
                rooms[lastClickedRoom].isLit = true;
                if (rooms[lastClickedRoom].roomLight != null)
                {
                    rooms[lastClickedRoom].roomLight.enabled = true;
                    rooms[lastClickedRoom].roomLight.color = targetColor;
                }
                roomStates[lastClickedRoom] = true; // Сохраняем новое состояние
                lastClickedRoom = -1;
                CheckAllRoomsLit();
            }
        }
    }

    private void InitializeRooms()
    {
        roomStates = new bool[rooms.Length]; // Создаем массив состояний

        // Сначала выключаем все комнаты
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].roomLight != null)
            {
                rooms[i].roomLight.enabled = false;
                rooms[i].isLit = false;
                roomStates[i] = false;
            }
        }

        // Случайно выбираем комнаты для освещения
        List<int> availableRooms = new List<int>();
        for (int i = 0; i < rooms.Length; i++)
        {
            availableRooms.Add(i);
        }

        for (int i = 0; i < numberOfLitRooms && availableRooms.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableRooms.Count);
            int roomIndex = availableRooms[randomIndex];
            
            if (rooms[roomIndex].roomLight != null)
            {
                rooms[roomIndex].roomLight.enabled = true;
                rooms[roomIndex].isLit = true;
                roomStates[roomIndex] = true; // Сохраняем состояние
            }
            
            availableRooms.RemoveAt(randomIndex);
        }
    }

    void Update()
    {
        HandleBlinking();
        CheckAllRoomsLit();

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    if (hit.collider.gameObject == rooms[i].roomObject && (!rooms[i].isLit || !rooms[i].hasCorrectColor))
                    {
                        lastClickedRoom = i;
                        StartWireGame();
                        break;
                    }
                }
            }
        }
    }

    private void HandleBlinking()
    {
        foreach (Room room in rooms)
        {
            if (!room.isLit && room.roomLight != null)
            {
                room.blinkTimer += Time.deltaTime * blinkSpeed;
                room.roomLight.enabled = true;
                
                // Мигание между красным и выключенным состоянием
                if (Mathf.Sin(room.blinkTimer) > 0)
                {
                    room.roomLight.color = redColor;
                }
                else
                {
                    room.roomLight.enabled = false;
                }
            }
            else if (room.isLit && room.roomLight != null)
            {
                // Проверяем правильный ли цвет
                room.hasCorrectColor = ColorApproximatelyEquals(room.roomLight.color, targetColor);
                
                // Для отладки
                if (!room.hasCorrectColor)
                {
                    Debug.Log($"Room color mismatch. Current: {room.roomLight.color}, Target: {targetColor}");
                }
            }
        }
    }

    private bool ColorApproximatelyEquals(Color color1, Color color2, float tolerance = 0.01f)
    {
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }

    void StartWireGame()
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(wireGameSceneName);
    }

    private void CheckAllRoomsLit()
    {
        bool allCorrect = true;
        foreach (Room room in rooms)
        {
            if (!room.isLit || !room.hasCorrectColor)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            // Для отладки
            Debug.Log("All rooms are lit and have correct color!");
            
            // Сбрасываем все статические переменные
            lastClickedRoom = -1;
            roomStates = null;
            SceneManager.LoadScene(1);
        }
    }

    public void ResetProgress()
    {
        lastClickedRoom = -1;
        roomStates = null; // Сбрасываем сохраненные состояния
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
} 