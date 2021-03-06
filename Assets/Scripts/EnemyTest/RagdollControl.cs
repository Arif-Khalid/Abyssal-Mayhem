using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollControl : MonoBehaviour,IPooledObject
{
    [SerializeField] private Animator animator;
    [SerializeField] Rigidbody rootBody;
    [SerializeField] Collider rootCollider;
    [SerializeField] float timeBeforeDestroyed;
    [SerializeField] Behaviour[] behavioursToDisable;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] Outline outline;
    [SerializeField] SkinnedMeshRenderer assassinMesh;
    [SerializeField] MeshRenderer sniperMesh;
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] float fadeScale;
    [SerializeField] float hurtFadeScale;
    [SerializeField] float maxSaturation;
    [SerializeField] float defaultSaturation;
    Material newMat;
    Color originalColor;
    private Rigidbody[] rigidBodies;
    private Collider[] ragdollColliders;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnAudio;
    [SerializeField] AudioClip[] takeDamageClips;
    [SerializeField] AudioClip deathClip;
    // Start is called before the first frame update
    void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        foreach(Collider collider in ragdollColliders)
        {
            if (collider != rootCollider) { collider.isTrigger = true; }
        }
        foreach(Rigidbody rb in rigidBodies)
        {
            rb.isKinematic = true;
        }
        newMat = Instantiate(assassinMesh.material);
        assassinMesh.material = newMat;
        sniperMesh.material = newMat;
        originalColor = newMat.color;
        enemyHealth.RegisterToTakeDamage(OnTakeDamage);
    }


    public void ToggleRagdoll(bool state)
    {
        audioSource.clip = deathClip;
        audioSource.Play();
        animator.enabled = !state;
        foreach(Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = !state;
        }

        foreach(Rigidbody rb in rigidBodies)
        {
            if(rb != rootBody)
            {
                rb.isKinematic = !state;
            }
        }

        foreach(Collider collider in ragdollColliders)
        {
            if(collider != rootCollider)
            {
                collider.isTrigger = !state;
            }
        }

        if (state) { Invoke(nameof(Destroy), timeBeforeDestroyed); }
    }

    public void OnTakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine(Hurt());
    }

    public void OnObjectSpawn()
    {
        ToggleRagdoll(false);
        enemyAI.ReenableNavMesh();
        newMat.SetFloat("_Opacity", 0f);
        newMat.color = originalColor;
        newMat.SetFloat("_Saturation", defaultSaturation);
        StartCoroutine(FadeIn());
    }
    private void Destroy()
    {
        StartCoroutine(FadeOut());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //Clear bullets
        GetComponent<AssassinAI>().ClearBullets();
        outline.enabled = false;
    }
    public IEnumerator FadeIn()
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
    }
    IEnumerator FadeOut()
    {
        float i = 1f;
        while (i >= 0)
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
        audioSource.clip = takeDamageClips[Random.Range(0, takeDamageClips.Length)];
        audioSource.Play();
        newMat.color = Color.red;
        newMat.SetFloat("_Saturation", maxSaturation);
        float currentSat = maxSaturation;
        while (newMat.color != originalColor || currentSat > defaultSaturation)
        {
            currentSat = Mathf.Lerp(currentSat, defaultSaturation, Time.deltaTime / hurtFadeScale);
            newMat.color = Color.Lerp(newMat.color, originalColor, Time.deltaTime / hurtFadeScale);
            newMat.SetFloat("_Saturation", currentSat);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        enemyHealth.RemoveFromTakeDamage(OnTakeDamage);
    }
}
