using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkerAI : EnemyAI
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject hitBox;
    [SerializeField] AudioSource hitSoundSource;
    private void Start()
    {
    }

    public override void ChasePlayer()
    {
        agent.SetDestination(new Vector3(player.position.x, player.position.y - 0.834f, player.position.z));
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

    public void PlayHitSound()
    {
        hitSoundSource.Play();
    }

}
