using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public void Transition(int sceneNumber)
    {
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadScene(sceneNumber);
        }
        else
    {
            SceneManager.LoadScene(sceneNumber);
        }
    }
}
