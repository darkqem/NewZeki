using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAUSE : MonoBehaviour
{
    public GameObject panel;
    public void pause()
    {
        Time.timeScale = 0f;
    }
}
