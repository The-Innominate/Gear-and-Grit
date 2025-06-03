using UnityEngine;

public class MusicPlayer : MonoBehaviour, IOptionObserver
{
    [SerializeField] AudioSource playAudio;
    private float musicVolume = 1.0f;

    void Start()
    {
        playAudio.Play();
        musicVolume = PlayerPrefs.GetFloat("Volume", 1.0f);
        playAudio.volume = musicVolume;

        var manager = FindObjectOfType<OptionsMenuManager>();
        if (manager != null)
        {
            manager.Subscribe(this);
        }
    }

    private void FixedUpdate()
    {
        playAudio.volume = musicVolume;
    }

    public void OnOptionChanged(string option, object value)
    {
        if (option == "Volume")
        {
            musicVolume = (float)value;
        }
    }
}
