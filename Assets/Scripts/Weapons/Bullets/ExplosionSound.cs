using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ExplosionSound : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnEnable()
    {
        audioSource.Play();
    }
}
