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
            s.Source = gameObject.AddComponent<AudioSource>();
            s.Source.clip = s.Clip;
            s.Source.volume = s.Volume;
            s.Source.pitch = s.Pitch;
            s.Source.loop = s.Loop;
            if (s.Name == "MenuBackground") { menuBackgroundSound = s; }
            else if (s.Name == "Background") { backgroundSound = s; }
        }
        menuBackgroundSound.Source.Play();
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s == null) {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else {
            s.Source.Play();
        }
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, sound => sound.Name == name);
        if (s == null) {
            Debug.LogWarning("Sound with name: <" + name + "> does not exist");
        }
        else {
            s.Source.Stop();
        }
    }

    public void StopAllSounds() {
        StopAllCoroutines();
        foreach (Sound s in sounds) {
            s.Source.Stop();
        }
    }

    public void ChangeBackgroundVolume(float newVolume) {
        menuBackgroundSound.Source.volume = newVolume;
        backgroundSound.Source.volume = newVolume;
    }

    public IEnumerator StartGameAudio() {
        menuBackgroundSound.Source.Stop();
        Play("Drum");
        yield return new WaitForSeconds(secondsForDrum);
        backgroundSound.Source.Play();
    }

    public void StopGameAudio() {
        StopAllSounds();
        menuBackgroundSound.Source.Play();
    }
}
