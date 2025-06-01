using UnityEngine;

public class GraphicsSettings : MonoBehaviour, IOptionObserver
{
    void Start()
    {
        // Subscribe to the options menu
        var manager = FindObjectOfType<OptionsMenuManager>();
        if (manager != null)
        {
            manager.Subscribe(this);
        }

        // Apply saved setting on start
        string savedQuality = PlayerPrefs.GetString("GraphicsQuality", "High");
        ApplyGraphicsQuality(savedQuality);
    }

    public void OnOptionChanged(string option, object value)
    {
        if (option == "GraphicsQuality")
        {
            string quality = value.ToString();
            ApplyGraphicsQuality(quality);
        }
    }

    private void ApplyGraphicsQuality(string quality)
    {
        switch (quality)
        {
            case "Low":
                QualitySettings.SetQualityLevel(0); break;
            case "Medium":
                QualitySettings.SetQualityLevel(2); break;
            case "High":
                QualitySettings.SetQualityLevel(5); break;
        }

        Debug.Log($"Graphics quality set to: {quality}");
    }
}
