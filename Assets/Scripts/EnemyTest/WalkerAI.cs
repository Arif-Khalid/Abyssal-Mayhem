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

    public override void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetFloat("velocity", agent.velocity.magnitude);
    }

    protected override void AttackPlayer()
    {
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        base.AttackPlayer();
        animator.SetFloat("velocity", agent.velocity.magnitude);
    }
    //Attack function for walker enemy
    public override void Attack()
    {
        animator.Play("Attack");
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
