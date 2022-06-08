using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsPlayer;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    //States
    public float attackRange;
    public bool playerInAttackRange;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (player)
        {
            //Check for attack range
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (!playerInAttackRange) ChasePlayer();
            if (playerInAttackRange) AttackPlayer();
        }
    }

    /*Code for attacks*/
    //Reset the ability to attack
    protected void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //Attack implementation called in AttackPlayer state
    public virtual void Attack()
    {

    }


    /*Code for states*/

    //Called in Late update
    //Stops movement and looks at player
    //Attack to be implemented in override
    protected virtual void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            Attack();
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    //Called in LateUpdate
    //Only chases player when out of attack range
    public virtual void ChasePlayer()
    { 
        agent.SetDestination(player.position);
        transform.LookAt(player);
    }
}
