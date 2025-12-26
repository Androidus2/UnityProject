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

    public void Play()
    {
        float volumeMultiplier = SoundManager.GetInstance().GetSFXVolume();
        source.volume = volumeMultiplier * originalVolume;

        float selectedPitch = Random.Range(minimumPitch, maximumPitch);
        source.pitch = selectedPitch;

        source.Play();
    }
}
