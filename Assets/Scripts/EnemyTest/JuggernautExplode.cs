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
        gameObject.SetActive(false);
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
    }
    private void OnDisable()
    {
        GetComponent<JuggernautAI>().ClearBullets();
        outline.enabled = false;
        if (juggernautWeaponPickup) { juggernautWeaponPickup.SetActive(false); }
    }
}
