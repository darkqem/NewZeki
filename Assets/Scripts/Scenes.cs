using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public AudioVolumeControl audioControl;
    public void Transition(int scneneNumber)
    {
        audioControl.SaveVolume();
        SceneManager.LoadScene(scneneNumber);
    }
}
