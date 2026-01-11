using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    [SerializeField] private float musicVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private float sensitivity;
    [SerializeField] private Dictionary<string, bool> unlockedEndings;

    // TODO: Add game state to this
    public Data(float _musicVolume, float _sfxVolume, float _sensitivity, Dictionary<string, bool> _endings)
    {
        musicVolume = _musicVolume;
        sfxVolume = _sfxVolume;
        sensitivity = _sensitivity;
        unlockedEndings = _endings;
    }

    public float GetMusicVolume() { return musicVolume; }
    public float GetSFXVolume() { return sfxVolume; }
    public float GetSensitivity() { return sensitivity; }

    public Dictionary<string, bool> GetUnlockedEndings()
    {
        return unlockedEndings;
    }

    public void SetUnlockedEnding(string ending, bool isUnlocked)
    {
        if (unlockedEndings == null)
        {
            unlockedEndings = new Dictionary<string, bool>();
        }
        if (unlockedEndings.ContainsKey(ending))
        {
            unlockedEndings[ending] = isUnlocked;
        }
        else
        {
            unlockedEndings.Add(ending, isUnlocked);
        }
    }

}