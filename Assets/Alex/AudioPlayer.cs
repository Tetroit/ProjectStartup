
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    AudioManager audioManager;
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            audioManager.PlaySound(audioSource, ESoundType.NumberCardSound);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            audioManager.PlaySoundtrack(audioSource, ESoundTrack.DefaultSoundtrack);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            audioManager.StopSoundtrack(audioSource);
        }
    }
}
