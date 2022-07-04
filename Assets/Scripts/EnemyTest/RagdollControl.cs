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
    [SerializeField] float fadeScale;
    Material newMat;
    private Rigidbody[] rigidBodies;
    private Collider[] ragdollColliders;

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

        if (state) { Invoke(nameof(Destroy), timeBeforeDestroyed); }
    }

    public void OnObjectSpawn()
    {
        ToggleRagdoll(false);
        enemyAI.ReenableNavMesh();
        newMat.SetFloat("_Opacity", 1f);
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
