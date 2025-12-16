using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Fade fade;

    public void PlayGame()
    {
        fade.BeginFade(() =>
        {
            // Load scene by incrementing the current scene's build index - from scene list in build profile/settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
}
