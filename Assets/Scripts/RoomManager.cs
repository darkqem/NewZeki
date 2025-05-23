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
    }

    public Room[] rooms;
    public int numberOfLitRooms = 5;
    public string wireGameSceneName = "WireGame";

    private static int lastClickedRoom = -1;
    private static bool[] roomStates = null; // Сохраняем состояния комнат

    void Start()
    {
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                for (int i = 0; i < rooms.Length; i++)
                {
                    if (hit.collider.gameObject == rooms[i].roomObject && !rooms[i].isLit)
                    {
                        lastClickedRoom = i;
                        StartWireGame();
                        break;
                    }
                }
            }
        }
    }

    void StartWireGame()
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(wireGameSceneName);
    }

    private void CheckAllRoomsLit()
    {
        bool allLit = true;
        foreach (Room room in rooms)
        {
            if (!room.isLit)
            {
                allLit = false;
                break;
            }
        }

        if (allLit)
        {
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