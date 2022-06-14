using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollControl : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] Rigidbody rootBody;
    [SerializeField] Collider rootCollider;
    [SerializeField] float timeBeforeDestroyed;
    [SerializeField] Behaviour[] behavioursToDisable;
    private Rigidbody[] rigidBodies;
    private Collider[] ragdollColliders;

    // Start is called before the first frame update
    void Start()
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
        
    }


    public void ToggleRagdoll(bool state)
    {
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

        Invoke(nameof(Destroy), timeBeforeDestroyed);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
