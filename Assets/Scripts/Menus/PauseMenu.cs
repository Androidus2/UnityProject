using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private InputAction pauseAction;

    [SerializeField]
    private GameObject pauseMenu;

    private bool isPaused;

    void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    void OnEnable()
    {
        pauseAction.Enable();
    }

    void OnDisable()
    {
        pauseAction.Disable();
    }

    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    void Update()
    {
        if (pauseAction.triggered)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game time

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed"); //for testing in editor
        Application.Quit();
    }

    public bool GameIsPaused()  //to use in other scripts to stop input actions, if needed
    {
        return isPaused;
    }
}
