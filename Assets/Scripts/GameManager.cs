using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Base interface for all game states
public interface IGameState
{
    void EnterState();
    void UpdateState();
    void ExitState();
}

// Game Manager class that will handle state transitions
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    _instance = go.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    private IGameState _currentState;
    private Dictionary<System.Type, IGameState> _states;
    private PauseMenu _pauseMenu;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeStates();
    }

    private void InitializeStates()
    {
        _states = new Dictionary<System.Type, IGameState>
        {
            { typeof(MainMenuState), new MainMenuState() },
            { typeof(PlayingState), new PlayingState() },
            { typeof(PausedState), new PausedState() },
            { typeof(GameOverState), new GameOverState() },
            { typeof(LoadingState), new LoadingState() }
        };

        // Set initial state
        ChangeState<MainMenuState>();
    }

    private void Update()
    {
        _currentState?.UpdateState();
    }

    public void ChangeState<T>() where T : IGameState
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
        }

        _currentState = _states[typeof(T)];
        _currentState.EnterState();
    }

    public void SetPauseMenu(PauseMenu pauseMenu)
    {
        _pauseMenu = pauseMenu;
    }
}

// Concrete state implementations
public class MainMenuState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entering Main Menu State");
        // Initialize main menu UI
        // Load main menu scene if needed
    }

    public void UpdateState()
    {
        // Handle main menu updates
    }

    public void ExitState()
    {
        Debug.Log("Exiting Main Menu State");
        // Clean up main menu resources
    }
}

public class PlayingState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entering Playing State");
        // Initialize game world
        // Start game systems
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateState()
    {
        // Handle gameplay updates
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.ChangeState<PausedState>();
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Playing State");
        // Clean up game resources
    }
}

public class PausedState : IGameState
{
    private PauseMenu _pauseMenu;

    public void EnterState()
    {
        Debug.Log("Entering Paused State");
        _pauseMenu = Object.FindObjectOfType<PauseMenu>();
        if (_pauseMenu != null)
        {
            _pauseMenu.PauseGame();
        }
        else
        {
            Debug.LogWarning("PauseMenu component not found in scene!");
        }
    }

    public void UpdateState()
    {
        // Handle pause menu updates
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.ChangeState<PlayingState>();
        }

        // Check if pause menu is still active
        if (_pauseMenu != null && !PauseMenu.isPaused)
        {
            GameManager.Instance.ChangeState<PlayingState>();
        }
    }

    public void ExitState()
    {
        Debug.Log("Exiting Paused State");
        if (_pauseMenu != null)
        {
            _pauseMenu.ResumeGame();
        }
    }
}

public class GameOverState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entering Game Over State");
        // Show game over screen
        // Display final score
    }

    public void UpdateState()
    {
        // Handle game over screen updates
    }

    public void ExitState()
    {
        Debug.Log("Exiting Game Over State");
        // Clean up game over resources
    }
}

public class LoadingState : IGameState
{
    public void EnterState()
    {
        Debug.Log("Entering Loading State");
        // Show loading screen
        // Start loading process
    }

    public void UpdateState()
    {
        // Handle loading progress
    }

    public void ExitState()
    {
        Debug.Log("Exiting Loading State");
        // Hide loading screen
    }
} 