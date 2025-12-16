using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private InputAction pauseAction;

    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject gameUI;

    [SerializeField]
    private float tweenDuration = 0.25f;
    [SerializeField]
    private Ease tweenEase = Ease.OutBack;

    [SerializeField]
    private Fade fade;

    private bool isPaused;

    private Tween pauseMenuTween;
    private Tween gameUITween;

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
        gameUI.SetActive(true);

        pauseMenu.transform.localScale = Vector3.zero;
        gameUI.transform.localScale = Vector3.one;

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
        KillTweens();

        // Tween to size 0 and disable
        gameUITween = gameUI.transform
            .DOScale(Vector3.zero, tweenDuration)
            .SetEase(tweenEase)
            .SetUpdate(true)
            .OnComplete(() => gameUI.SetActive(false));

        // Enable and tween to size 1
        pauseMenu.SetActive(true);
        pauseMenu.transform.localScale = Vector3.zero;

        pauseMenuTween = pauseMenu.transform
            .DOScale(Vector3.one, tweenDuration)
            .SetEase(tweenEase)
            .SetUpdate(true);


        Time.timeScale = 0f; // Freeze game time

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
    }

    public void ResumeGame()
    {
        KillTweens();

        // Tween to size 0 and disable
        pauseMenuTween = pauseMenu.transform
            .DOScale(Vector3.zero, tweenDuration)
            .SetEase(tweenEase)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                pauseMenu.SetActive(false);

                Time.timeScale = 1f; // Resume game time

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            });

        // Enable and tween to size 1
        gameUI.SetActive(true);
        gameUI.transform.localScale = Vector3.zero;

        gameUITween = gameUI.transform
            .DOScale(Vector3.one, tweenDuration)
            .SetEase(tweenEase)
            .SetUpdate(true);

        isPaused = false;
    }

    private void KillTweens()
    {
        pauseMenuTween?.Kill();
        gameUITween?.Kill();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        fade.BeginFade(() =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }

    public void QuitGame()
    {
        fade.BeginFade(() =>
        {
            Debug.Log("Quit pressed"); //for testing in editor
            Application.Quit();
        });
    }

    public bool GameIsPaused()  //to use in other scripts to stop input actions, if needed
    {
        return isPaused;
    }
}
