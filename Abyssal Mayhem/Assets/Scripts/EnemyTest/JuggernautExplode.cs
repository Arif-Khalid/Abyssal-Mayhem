using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautExplode : MonoBehaviour
{
    //Script to explode juggernaut on death
    MeshCollider[] colliders;
    [SerializeField] JuggernautMissile juggernautMissile;
    [SerializeField] float explosionForce;
    [SerializeField] float explosionRange;
    [SerializeField] Behaviour[] behavioursToDisable;
    [SerializeField] float timeBeforeDestroy;
    void Start()
    {
        colliders = GetComponentsInChildren<MeshCollider>();
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
        foreach(MeshCollider collider in colliders)
        {
            Rigidbody rigidBody = collider.gameObject.AddComponent<Rigidbody>();
            rigidBody.AddExplosionForce(explosionForce, transform.position, explosionRange);
        }
        Invoke(nameof(Destroy), timeBeforeDestroy);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }

}
