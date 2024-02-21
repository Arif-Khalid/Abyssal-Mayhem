using System;
using System.Collections;
using UnityEngine;

/**
 * Responsible for playing global, non-spatial audio
 * All audio is played from audio sources attached to this game object
 */
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;        // Singleton instance

    // Sounds
    public Sound[] sounds;                      // List of sounds, each spawns its own audio source component
    // References to specific sounds
    // These sounds can have their volume
    // changed individually
    Sound backgroundSound;
    Sound menuBackgroundSound;

    [SerializeField] float secondsForDrum;      // Number of seconds allowed for the drum sound to play before the bgm

    private void Awake() {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }
        else {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            if (s.name == "MenuBackground") { menuBackgroundSound = s; }
            else if (s.name == "Background") { backgroundSound = s; }
        }
        menuBackgroundSound.source.Play();
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else {
            s.source.Play();
        }
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else {
            s.source.Stop();
        }
    }

    public void StopAllSounds() {
        StopAllCoroutines();
        foreach (Sound s in sounds) {
            s.source.Stop();
        }
    }

    public void ChangeBackgroundVolume(float newVolume) {
        menuBackgroundSound.source.volume = newVolume;
        backgroundSound.source.volume = newVolume;
    }

    public IEnumerator StartGameAudio() {
        menuBackgroundSound.source.Stop();
        Play("Drum");
        yield return new WaitForSeconds(secondsForDrum);
        backgroundSound.source.Play();
    }

    public void StopGameAudio() {
        StopAllSounds();
        menuBackgroundSound.source.Play();
    }
}
