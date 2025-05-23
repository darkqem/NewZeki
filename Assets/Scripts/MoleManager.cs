using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class MoleManager : MonoBehaviour
{
    public MoleSlot[] slots;
    private int frameCounter = 0;
    
    public int nextSceneNumber = 1;
    private bool transitionStarted = false;

    void Update()
    {
        frameCounter++;
        if (frameCounter >= 30)
        {
            frameCounter = 0;
            TrySpawnMole();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }

        if (!transitionStarted && AllRoomsCompleted())
        {
            transitionStarted = true;
            Invoke("TransitionToNextScene", 1f); // Переход через 1 секунду
        }
    }

    bool AllRoomsCompleted()
    {
        foreach (var slot in slots)
        {
            if (!slot.wasLightOffAfterActivation)
                return false;
        }
        return true;
    }

    void TransitionToNextScene()
    {
        // Используем SceneTransitionManager для плавного перехода если он доступен
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(nextSceneNumber);
        }
        else
        {
            // Если SceneTransitionManager недоступен, используем стандартный переход
            SceneManager.LoadScene(nextSceneNumber);
        }
    }

    void TrySpawnMole()
    {
        int activeCount = 0;
        foreach (var slot in slots)
            if (slot.IsActive) activeCount++;

        if (activeCount >= 2) return;

        // Случайный неактивный слот
        for (int i = 0; i < 10; i++)
        {
            int index = Random.Range(0, slots.Length);
            if (!slots[index].IsActive && !slots[index].lightActivated)
            {
                slots[index].SpawnMole();
                break;
            }
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MoleSlot slot = hit.collider.GetComponent<MoleSlot>();
            if (slot != null)
            {
                slot.Hit(); // Удар по кроту
            }
        }
    }
}
