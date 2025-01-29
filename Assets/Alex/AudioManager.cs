
using UnityEngine;

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
[CreateAssetMenu(fileName="audioLib",menuName ="Audio Collection")]
//[RequireComponent(typeof(AudioSource))]
public class AudioManager : ScriptableObject
{
    [SerializeField] private AudioClip[] soundList;
    [SerializeField] private AudioClip[] SoundTrackList;


    /// <summary>
    /// Plays a one-shot sound effect.
    /// </summary>
    public void PlaySound(AudioSource soundSource, ESoundType sound, float volume = 1)
    {
        soundSource.volume = Mathf.Clamp01(volume);
        soundSource.PlayOneShot(soundList[(int)sound], volume);
    }

    /// <summary>
    /// Plays a looping soundtrack.
    /// </summary>
    public void PlaySoundtrack(AudioSource soundSource, ESoundTrack soundTrack, float volume = 1)
    {
        soundSource.loop = true;
        soundSource.volume = Mathf.Clamp01(volume);
        soundSource.PlayOneShot(SoundTrackList[(int)soundTrack], volume);
    }

    /// <summary>
    /// Stops the currently playing soundtrack.
    /// </summary>
    public void StopSoundtrack(AudioSource soundSource)
    {
        soundSource.Stop();
    }
}
