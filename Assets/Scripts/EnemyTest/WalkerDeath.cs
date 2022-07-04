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
        newMat.SetFloat("_Opacity", 1);
        newMat.color = originalColor;
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
        newMat.color = Color.red;
        while (newMat.color != originalColor)
        {
            newMat.color = Color.Lerp(newMat.color, originalColor, Time.deltaTime/hurtFadeScale);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        enemyHealth.RemoveFromTakeDamage(OnTakeDamage);
    }
}
