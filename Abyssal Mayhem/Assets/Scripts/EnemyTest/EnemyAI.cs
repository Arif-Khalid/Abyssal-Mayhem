//using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;

    [HideInInspector]
    public Transform player;

    public Collider enemyFeet;

    public LayerMask whatIsPlayer;

    public LayerMask whatIsSelf;

    public Rigidbody enemyRigidBody;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float attackRange;
    public bool playerInAttackRange;
    public bool isNavMeshAgentEnabled = true;

    // Start is called before the first frame update
    void Start()
    {
        enemyRigidBody = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isNavMeshAgentEnabled)
        {
            if (player)
            {
                //Check for attack range
                playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

                if (!playerInAttackRange) ChasePlayer();
                if (playerInAttackRange) AttackPlayer();
            }
        } else {
            BounceBackRedo();
        }
    }

    /*Code for attacks*/
    //Reset the ability to attack
    private void ResetAttack()
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
    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

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
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    public void BounceBackUndo(Vector3 direction)
    {
        Invoke("SetBoolean", 0.05f);
        // isNavMeshAgentEnabled = false;
        agent.enabled = false;
        enemyFeet.enabled = true;
        Debug.Log("Undone");
        enemyRigidBody.isKinematic = false;
    }

    public void BounceBackRedo()
    {
        if(enemyRigidBody.velocity.magnitude <= 0.00001)
        {
            Debug.Log("Redone");
            isNavMeshAgentEnabled = true;
            enemyFeet.enabled = false;
            agent.enabled = true;
            enemyRigidBody.isKinematic = true;
        }
    }

    public void SetBoolean()
    {
        isNavMeshAgentEnabled  = false;
    }
}
