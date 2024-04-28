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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            ResumeGame();
        }
        else if (Input.GetKeyDown(KeyCode.P) && !isPaused)
        {
            Cursor.lockState = CursorLockMode.Confined;
        PauseGame();
        }
    }

    public void PauseGame() 
    {
        pauseMenu.SetActive(true);
        Cursor.visible = true;
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        isPaused = false;
    }
    public void ReturnToMain() 
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Menu");
    }
}
