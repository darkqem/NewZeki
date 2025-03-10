using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audioStart : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }


        if (PlayerPrefs.HasKey("AudioVolume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("AudioVolume");
        }

        
    }

    
}
