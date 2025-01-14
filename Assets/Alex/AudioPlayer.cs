using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    AudioManager manager;
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
            manager.PlaySound(audioSource, ESoundType.NumberCardSound);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            manager.PlaySoundtrack(audioSource, ESoundTrack.DefaultSoundtrack);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            manager.StopSoundtrack(audioSource);
        }
    }
}
