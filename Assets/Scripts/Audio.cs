using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeControl : MonoBehaviour
{
    public AudioSource audioSource; // Ссылка на AudioSource
    public Slider volumeSlider; // UI-слайдер для управления громкостью
    
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
        
        if (volumeSlider != null)
        {
            volumeSlider.value = audioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }
    
    void Update()
    {
        
    }
    
    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("AudioVolume", audioSource.volume);
        PlayerPrefs.Save();
    }
    
}
