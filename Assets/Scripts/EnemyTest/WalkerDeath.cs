using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WalkerDeath : MonoBehaviour,IPooledObject
{
    [SerializeField] Animator animator;
    [SerializeField] Behaviour[] behavioursToDisable;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] GameObject hitBox;
    [SerializeField] Outline outline;
    [SerializeField] SkinnedMeshRenderer mushroomMesh;
    [SerializeField] MeshRenderer hornMesh;
    Material newMat;
    Color originalColor;
    [SerializeField] float hurtFadeScale; //Higher scale means a slower fade from red to normal color on getting hurt
    [SerializeField] float fadeScale; //Higher fade scale means a slower fade to dissappearing on death

    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnAudio;
    [SerializeField] float timeBetweenSound;
    [SerializeField] AudioClip[] gruntClips;
    [SerializeField] AudioClip[] takeDamageClips;

    private void Awake()
    {
        newMat = Instantiate(mushroomMesh.material);
        mushroomMesh.material = newMat;
        hornMesh.material = newMat;
        originalColor = newMat.color;
        enemyHealth.RegisterToTakeDamage(OnTakeDamage);
    }

    public void DeathAnim()
    {
        foreach(Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = false;
        }
        animator.Play("MushroomDie");
    }

    private void Destroy()
    {
        StartCoroutine(FadeOut());
    }

    public void OnObjectSpawn()
    {
        enemyAI.ReenableNavMesh();
        foreach (Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = true;
        }
        hitBox.SetActive(false);
        newMat.SetFloat("_Opacity", 0);
        newMat.color = originalColor;
        StartCoroutine(FadeIn());
    }

    public void OnTakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine(Hurt());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        outline.enabled = false;
    }
    
    IEnumerator MonsterSound()
    {
        while (true)
        {
            //Assign clip to a random clip and play
            audioSource.Stop();
            audioSource.clip = gruntClips[UnityEngine.Random.Range(0, gruntClips.Length)];
            audioSource.Play();
            yield return new WaitForSeconds(timeBetweenSound);
        }       
    }
    IEnumerator FadeIn()
    {
        audioSource.clip = spawnAudio;
        audioSource.Play();
        enemyAI.enabled = false;
        enemyHealth.isDead = true;
        float i = 0f;
        while (i < 1f)
        {
            i += Time.deltaTime / fadeScale;
            newMat.SetFloat("_Opacity", i);
            yield return null;
        }
        enemyAI.enabled = true;
        enemyHealth.isDead = false;
        StartCoroutine(MonsterSound());
    }
    IEnumerator FadeOut()
    {
        float i = 1f;
        while(i >= 0)
        {
            i -= Time.deltaTime / fadeScale;
            newMat.SetFloat("_Opacity", i);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    IEnumerator Hurt()
    {
        audioSource.Stop();
        audioSource.clip = takeDamageClips[UnityEngine.Random.Range(0, takeDamageClips.Length)];
        audioSource.Play();
        newMat.color = Color.red;
        while (newMat.color != originalColor)
        {
            newMat.color = Color.Lerp(newMat.color, originalColor, Time.deltaTime/hurtFadeScale);
            yield return null;
        }
        StartCoroutine(MonsterSound());
    }

    private void OnDestroy()
    {
        enemyHealth.RemoveFromTakeDamage(OnTakeDamage);
    }
}
