using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum ESoundType
{
    NumberCardSound,
    OperatorCardSound,
    ActionCardSound
}

public enum ESoundTrack
{
    DefaultSoundtrack,

}
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] SoundTrackList;
    public static AudioManager instance;
    private AudioSource soundSource;
    private AudioSource musicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) Destroy(gameObject);
    }

    private void Start()
    {
        soundSource = GetComponent<AudioSource>();
        musicSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlaySound(ESoundType.NumberCardSound);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            PlaySoundtrack(ESoundTrack.DefaultSoundtrack);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StopSoundtrack();
        }
    }

    /// <summary>
    /// Plays a one-shot sound effect.
    /// </summary>
    public static void PlaySound(ESoundType sound, float volume = 1)
    {
        instance.soundSource.volume = Mathf.Clamp01(volume);
        instance.soundSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }

    /// <summary>
    /// Plays a looping soundtrack.
    /// </summary>
    public void PlaySoundtrack(ESoundTrack soundTrack, float volume = 1)
    {
        musicSource.loop = true;
        musicSource.volume = Mathf.Clamp01(volume);
        instance.soundSource.PlayOneShot(instance.SoundTrackList[(int)soundTrack], volume);
    }

    /// <summary>
    /// Stops the currently playing soundtrack.
    /// </summary>
    public void StopSoundtrack()
    {
        musicSource.Stop();
    }
}
