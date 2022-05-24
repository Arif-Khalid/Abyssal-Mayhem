using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkerAI : EnemyAI
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitBox;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    //Attack function for walker enemy
    public override void Attack()
    {
        animator.Play("WalkerMeelee");
    }


    /*Functions to enable and disable meelee hitbox called in animation events*/
    public void EnableHitbox()
    {
        hitBox.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitBox.SetActive(false);
    }
}
