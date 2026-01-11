using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    private float sensitivity = 1.0f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (instance == this)
        {
            // We are loading in Start instead of Awake so we make sure SoundManager exists
            LoadGame();
        }
    }

    private void OnApplicationQuit()
    {
        if (instance == this)
            SaveGame();
    }

    private void LoadGame()
    {
        // For now LoadGame will only load the settings
        Data loadedData = SaveSystem.Load();
        if (loadedData != null)
        {
            sensitivity = loadedData.GetSensitivity();
            float musicVolume = loadedData.GetMusicVolume();
            float sfxVolume = loadedData.GetSFXVolume();

            SoundManager soundManager = SoundManager.GetInstance();
            soundManager.SetMusicVolume(musicVolume);
            soundManager.SetSFXVolume(sfxVolume);


            Ending endingManager = Ending.GetInstance();
            Dictionary<string, bool> endings = loadedData.GetUnlockedEndings();
            endingManager.SetUnlockedEndings(endings);
        }
    }

    private void SaveGame()
    {
        // For now SaveGame will only save the settings
        SoundManager soundManager = SoundManager.GetInstance();
        float musicVolume = soundManager.GetMusicVolume();
        float sfxVolume = soundManager.GetSFXVolume();
        Ending endingManager = Ending.GetInstance();
        Dictionary<string, bool> endings = endingManager.GetEndingsState();

        Data saveData = new Data(musicVolume, sfxVolume, sensitivity, endings);
        SaveSystem.Save(saveData);
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public float GetSensitivity()
    {
        return sensitivity;
    }

    public void SetSensitivity(float sensitivity)
    {
        this.sensitivity = sensitivity;
    }

}
