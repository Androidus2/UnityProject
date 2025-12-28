using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Fade fade;

    [SerializeField]
    private Slider soundEffectsSlider;

    [SerializeField]
    private Slider musicSlider;

    [SerializeField]
    private Slider sensitivitySlider;

    [SerializeField]
    private TextMeshProUGUI sensitivityText;

    private void Start()
    {
        soundEffectsSlider.value = SoundManager.GetInstance().GetSFXVolume();
        musicSlider.value = SoundManager.GetInstance().GetMusicVolume();
        float sensitivity = GameManager.GetInstance().GetSensitivity();
        sensitivitySlider.value = sensitivity;
        UpdateSensitivityText(sensitivity);
    }

    public void PlayGame()
    {
        fade.BeginFade(() =>
        {
            // Load scene by incrementing the current scene's build index - from scene list in build profile/settings
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }

    private void UpdateSensitivityText(float newSensitivity)
    {
        sensitivityText.text = "Sensitivity: " + newSensitivity.ToString("F1");
    }

    public void ChangeSFXVolume()
    {
        SoundManager.GetInstance().SetSFXVolume(soundEffectsSlider.value);
    }

    public void ChangeMusicVolume()
    {
        SoundManager.GetInstance().SetMusicVolume(musicSlider.value);
    }

    public void ChangeSensitivity()
    {
        float newSensitivity = sensitivitySlider.value;
        UpdateSensitivityText(newSensitivity);
        GameManager.GetInstance().SetSensitivity(newSensitivity);
    }

    public void ResetSettings()
    {
        soundEffectsSlider.value = 1f;
        ChangeSFXVolume();

        musicSlider.value = 1f;
        ChangeMusicVolume();

        sensitivitySlider.value = 1f;
        ChangeSensitivity();
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
