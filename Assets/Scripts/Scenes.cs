using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public int scneneNumber;

    public void Transition()
    {
        SceneManager.LoadScene(scneneNumber);
    }
}
