using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class NewGameButton : MonoBehaviour
{
    [SerializeField] private string firstLevelSceneName = "Game"; // Имя сцены, которая загрузится при новой игре
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartNewGame);
    }

    private void StartNewGame()
    {
        // Начинаем новую игру через GameManager
        GameManager.Instance.StartNewGame();
        
        // Загружаем первую сцену игры
        SceneManager.LoadScene(firstLevelSceneName);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(StartNewGame);
        }
    }
} 