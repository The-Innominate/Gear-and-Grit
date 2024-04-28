using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    //[SerializeField] AudioSource menuAudio;
    [SerializeField] AudioSource playAudio;
    public Slider volumeSlider;
    private float musicVolume = 1.0f;


    void Start()
    {
        playAudio.Play();
        musicVolume = PlayerPrefs.GetFloat("Volume");
        playAudio.volume = musicVolume;
        volumeSlider.value = musicVolume;
    }
    private void FixedUpdate()
    {
        playAudio.volume = musicVolume;
        PlayerPrefs.SetFloat("Volume", musicVolume);
    }
    public void VolumeChange(float volume) 
    { 
        musicVolume = volume;
    }
}
