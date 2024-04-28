using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

    private void Start()
    {
        optionsMenu.SetActive(false);
    }
    private void Update()
    {
    }


    public void ToggleMainMenu()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);
        if (optionsMenu.active) optionsMenu.SetActive(false);
    }
    public void ToggleOptionsMenu()
    {
        optionsMenu.SetActive(!optionsMenu.activeSelf);
        if (mainMenu.active) mainMenu.SetActive(false);
    }
    public void LoadGameScene() 
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void LoadGameOverScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void QuitGame() 
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
   
}

