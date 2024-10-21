using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name; // Name of the sound
    public AudioClip clip; // The actual audio clip
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] BgSounds, sfxSounds;
    public AudioSource BgSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        PlayMusic("Battle For Glory");
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(BgSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not Found");
        }
        else
        {
            BgSource.clip = s.clip;
            BgSource.Play();
        }
    }
    public void BGMVolume(float volume)
    {
        BgSource.volume = volume;
    }
}
