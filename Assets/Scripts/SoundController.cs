using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    AudioSource audioSource;

    public AudioClip killAudio;
    public AudioClip giftAudio;


    public void PlayKillAudio()
    {
        audioSource.clip = killAudio;
        audioSource.Play();
    }
    public void PlayGiftAudio()
    {
        audioSource.clip = giftAudio;
        audioSource.Play();
    }
}
