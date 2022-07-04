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
    MeshRenderer[] juggernautMeshes;
    Material newMat;

    private void Awake()
    {
        juggernautMeshes = GetComponentsInChildren<MeshRenderer>();
        newMat = Instantiate(juggernautMeshes[0].material);
        foreach(MeshRenderer mesh in juggernautMeshes)
        {
            mesh.material = newMat;
        }
    }

    public void ExplodeOnDeath()
    {
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
        newMat.SetFloat("_Opacity", 1f);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        GetComponent<JuggernautAI>().ClearBullets();
        outline.enabled = false;
        if (juggernautWeaponPickup) { juggernautWeaponPickup.SetActive(false); }
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
}
