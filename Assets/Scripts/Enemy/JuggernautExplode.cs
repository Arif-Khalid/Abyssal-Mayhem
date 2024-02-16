using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautExplode : MonoBehaviour, IPooledObject
{
    //Script to explode juggernaut on death
    [SerializeField] Rigidbody[] rigidBodies;
    [SerializeField] JuggernautMissile juggernautMissile;
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRange;
    [SerializeField] Behaviour[] behavioursToDisable;
    [SerializeField] float timeBeforeDestroy;
    [SerializeField] GameObject juggernautWeaponPickup;
    [SerializeField] bool isBoss = false;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] Outline outline;
    [SerializeField] float fadeScale;
    [SerializeField] float hurtFadeScale; //Higher fade scale means a slower fade to original color on hurt
    [SerializeField] EnemyHealth enemyHealth;
    MeshRenderer[] juggernautMeshes;
    Material newMat;
    Color originalColor;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource gruntSource;
    [SerializeField] AudioClip spawnAudio;
    [SerializeField] AudioClip[] takeDamageClips;
    private void Awake()
    {
        juggernautMeshes = GetComponentsInChildren<MeshRenderer>();
        newMat = Instantiate(juggernautMeshes[0].material);
        foreach(MeshRenderer mesh in juggernautMeshes)
        {
            mesh.material = newMat;
        }
        originalColor = newMat.color;
        enemyHealth.RegisterToTakeDamage(OnTakeDamage);
    }

    public void ExplodeOnDeath()
    {
        gruntSource.Stop();
        ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
        if (juggernautMissile)
        {
            juggernautMissile.StopAllCoroutines();
        }
        foreach(Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = false;
        }
        foreach(Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = false;
            rigidBody.AddExplosionForce(explosionForce, transform.position, explosionRange);
        }
        if (isBoss) {
            juggernautWeaponPickup.SetActive(true);
        }
        
        Invoke(nameof(Destroy), timeBeforeDestroy);
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
        foreach (Rigidbody rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
        }
        newMat.SetFloat("_Opacity", 0f);
        newMat.color = originalColor;
        if (juggernautMissile) { juggernautMissile.missilesShot = true; }
        StartCoroutine(FadeIn());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        GetComponent<JuggernautAI>().ClearBullets();
        outline.enabled = false;
        if (juggernautWeaponPickup) { juggernautWeaponPickup.SetActive(false); }
    }

    public void OnTakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine(Hurt());
    }

    private void MonsterSound()
    {
        gruntSource.Play();
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
        MonsterSound();
        enemyHealth.isDead = false;
        if (juggernautMissile) { juggernautMissile.missilesShot = false; }
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
