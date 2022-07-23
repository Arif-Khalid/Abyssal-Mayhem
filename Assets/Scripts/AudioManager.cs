using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField] float secondsForDrum;
    public Sound[] sounds;
    public static AudioManager instance;
    Sound backgroundSound;
    Sound menuBackgroundSound;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if(s.name == "MenuBackground") { menuBackgroundSound = s; }
            else if(s.name == "Background") { backgroundSound = s; }
        }
        menuBackgroundSound.source.Play();
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null)
        {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else
        {
            s.source.Play();
        }
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else
        {
            s.source.Stop();
        }
    }

    public void StopAllSounds()
    {
        StopAllCoroutines();
        foreach(Sound s in sounds)
        {
            s.source.Stop();
        }
    }

    public void ChangeBackgroundVolume(float newVolume)
    {
        menuBackgroundSound.source.volume = newVolume;
        backgroundSound.source.volume = newVolume;
    }

    public IEnumerator StartGameAudio()
    {
        menuBackgroundSound.source.Stop();
        Play("Drum");
        yield return new WaitForSeconds(secondsForDrum);
        backgroundSound.source.Play();
    }

    public void StopGameAudio()
    {
        StopAllSounds();
        menuBackgroundSound.source.Play();
    }
}
