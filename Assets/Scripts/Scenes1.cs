using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes1 : MonoBehaviour
{
    public void Transition1(int sceneNumber)
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