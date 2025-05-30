using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public static bool isPaused = false;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
        GameManager.Instance.SetPauseMenu(this);
    }

    // Update is called once per frame
    void Update()
    {
        // Pause functionality is now handled by the state system
    }

    public void PauseGame() 
    {
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }
    public void ReturnToMain() 
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.ChangeState<MainMenuState>();
        SceneManager.LoadScene("Menu");
    }
}
