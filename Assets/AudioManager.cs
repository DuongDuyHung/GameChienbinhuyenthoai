using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicAudioSource;
    public AudioClip musicAudioClip;

    void Start()
    {
        musicAudioSource.clip = musicAudioClip;
        musicAudioSource.Play();
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusic()
    {
        musicAudioSource.mute = !musicAudioSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }
}
