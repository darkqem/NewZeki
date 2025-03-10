using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioVolumeControl audioControl;

    public void LoadNextScene()
    {
        audioControl.SaveVolume(); // Сохраняем громкость
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
