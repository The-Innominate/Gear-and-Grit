// Observer interface
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

interface IOptionObserver
{
    void OnOptionChanged(string option, object value);
}

// Subject
class OptionsMenuManager : MonoBehaviour
{
    private List<IOptionObserver> observers = new();

    public void Subscribe(IOptionObserver observer) 
    {
        if (!observers.Contains(observer)) observers.Add(observer);
    } 
    public void Unsubscribe(IOptionObserver observer)
    {
        if (observers.Contains(observer)) observers.Remove(observer);
    }


    public void NotifyOptionChanged(string option, object value)
    {
        foreach (var observer in observers)
        {
            observer.OnOptionChanged(option, value);
        }
    }
    public void OnVolumeSliderChanged(float volume)
    {
        PlayerPrefs.SetFloat("Volume", volume);
        NotifyOptionChanged("Volume", volume);
    }

    private readonly string[] qualityLevels = { "Low", "Medium", "High" };

    public void OnGraphicsQualityChanged(int index)
    {
        string selectedQuality = qualityLevels[index];
        PlayerPrefs.SetString("GraphicsQuality", selectedQuality);
        NotifyOptionChanged("GraphicsQuality", selectedQuality);
    }
}