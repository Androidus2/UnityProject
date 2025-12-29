using UnityEngine;

[System.Serializable]
public class Data
{
    [SerializeField] private float musicVolume;
    [SerializeField] private float sfxVolume;
    [SerializeField] private float sensitivity;

    // TODO: Add game state to this
    public Data(float _musicVolume, float _sfxVolume, float _sensitivity)
    {
        musicVolume = _musicVolume;
        sfxVolume = _sfxVolume;
        sensitivity = _sensitivity;
    }

    public float GetMusicVolume() { return musicVolume; }
    public float GetSFXVolume() { return sfxVolume; }
    public float GetSensitivity() { return sensitivity; }

}