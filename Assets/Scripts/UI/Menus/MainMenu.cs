using System.Collections.Generic;
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

    private Dictionary<string, bool> unlockedEndings;

    [SerializeField]
    private GameObject EndingTextBoxSaint;


    [SerializeField]
    private GameObject EndingTextBoxDelinquent;



    [SerializeField]
    private GameObject EndingTextBoxOneOfUs;


    [SerializeField]
    private GameObject EndingTextBoxLoser;


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
            EndingContext.SetQuit();
            SceneManager.LoadScene("Ending");
            //Application.Quit();
        });
    }

    public void ChangeEndingsDisplay()
    {
        Dictionary<string, bool> endings = Ending.GetInstance().GetEndingsState();

        if (endings["Saint"])
        {
            UnlockAchievement(EndingTextBoxSaint);
        }

        if (endings["Delinquent"])
        {
            UnlockAchievement(EndingTextBoxDelinquent);
        }
        if (endings["OneOfUs"])
        {
            UnlockAchievement(EndingTextBoxOneOfUs);
        }
        if (endings["Loser"])
        {
            UnlockAchievement(EndingTextBoxLoser);
            EndingTextBoxLoser.GetComponentInChildren<TextMeshProUGUI>().text = "LOSER";
        }
       
    }

    private void UnlockAchievement(GameObject endingTextBox)
    {
        Transform unlockedTransform = endingTextBox.transform.Find("UNLOCKED");
        if (unlockedTransform != null)
        {
            GameObject unlocked = unlockedTransform.gameObject;
            unlocked.SetActive(true);
        }

        Transform lockedTransform = endingTextBox.transform.Find("LOCKED");
        if (lockedTransform != null)
        {
            GameObject locked = lockedTransform.gameObject;
            locked.SetActive(true);
        }
    }
}
