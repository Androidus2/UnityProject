using UnityEngine;

// Wrapper class around AudioSource
[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource source;

    private float originalVolume;

    [SerializeField]
    private float minimumPitch = -50;

    [SerializeField]
    private float maximumPitch = -50;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        originalVolume = source.volume;

        if (minimumPitch == -50 || maximumPitch == -50)
        {
            minimumPitch = source.pitch;
            maximumPitch = source.pitch;
        }
    }

    private void Start()
    {
        SetVolume();
        SetPitch();
    }

    private void SetVolume()
    {
        float volumeMultiplier = SoundManager.GetInstance().GetSFXVolume();
        source.volume = volumeMultiplier * originalVolume;
    }

    private void SetPitch()
    {
        float selectedPitch = Random.Range(minimumPitch, maximumPitch);
        source.pitch = selectedPitch;
    }

    public bool IsPlaying()
    {
        return source.isPlaying;
    }

    public void Play()
    {
        SetVolume();
        SetPitch();
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }
}
