using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerDeath : MonoBehaviour,IPooledObject
{
    Animator animator;
    [SerializeField] Behaviour[] behavioursToDisable;
    [SerializeField] EnemyAI enemyAI;
    [SerializeField] GameObject hitBox;
    [SerializeField] Outline outline;
    private void Start()
    {
        animator = GetComponent<Animator>();
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
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        enemyAI.ReenableNavMesh();
        foreach (Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = true;
        }
        hitBox.SetActive(false);
    }

    private void OnDisable()
    {
        outline.enabled = false;
    }
}
