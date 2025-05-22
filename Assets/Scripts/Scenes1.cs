using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes1 : MonoBehaviour
{
    public void Transition1(int scneneNumber1)
    {
        SceneManager.LoadScene(scneneNumber1);
    }
}
