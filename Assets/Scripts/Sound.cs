using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

/**
 * Responsible for representing a sound
 * along with how it should be played
 */
[System.Serializable]
public class Sound
{
    [FormerlySerializedAs("clip")] public AudioClip Clip;

    [FormerlySerializedAs("name")] public string Name;
    [Range(0f,1f)]
    [FormerlySerializedAs("volume")] public float Volume;
    [Range(0f,2f)]
    [FormerlySerializedAs("pitch")] public float Pitch;
    [FormerlySerializedAs("loop")] public bool Loop;

    [FormerlySerializedAs("description")] public string Description;
    [HideInInspector]
    [FormerlySerializedAs("source")] public AudioSource Source;

}
