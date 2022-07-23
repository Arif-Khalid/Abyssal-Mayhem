using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public AudioClip clip;

    public string name;
    [Range(0f,1f)]
    public float volume;
    [Range(0f,2f)]
    public float pitch;
    public bool loop;

    public string description;
    [HideInInspector]
    public AudioSource source;

}
