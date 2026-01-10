using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource musicSource;

    private float originalMusicVolume;

    private static SoundManager instance;

    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        originalMusicVolume = musicSource.volume;
    }

    public static SoundManager GetInstance()
    {
        return instance;
    }

    public void SetMusicVolume(float newMusicVolume)
    {
        musicVolume = newMusicVolume;
        musicSource.volume = originalMusicVolume * newMusicVolume;
    }

    public void SetSFXVolume(float newSFXVolume)
    {
        sfxVolume = newSFXVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

}
