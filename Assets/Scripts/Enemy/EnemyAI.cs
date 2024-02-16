//using System.Diagnostics;
using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public Collider enemyFeet;

    public LayerMask whatIsPlayer;

    public Rigidbody enemyRigidBody;

    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

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
    protected virtual void LateUpdate()
    {
        if (isNavMeshAgentEnabled)
        {
            if (player && agent.enabled)
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
    public void ResetAttack()
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
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    public void BounceBackUndo()
    {
        Invoke("SetBoolean", 0.05f);
        enemyFeet.enabled = true;
        agent.enabled = false;        
        enemyRigidBody.isKinematic = false;
        Debug.Log("Undone");
    }

    public void BounceBackRedo()
    {
        if(enemyRigidBody.velocity.magnitude <= 0.00001)
        {
            ReenableNavMesh();
        }
    }

    public void ReenableNavMesh()
    {
        isNavMeshAgentEnabled = true;
        enemyFeet.enabled = false;
        agent.enabled = true;
        enemyRigidBody.isKinematic = true;
    }

    public void SetBoolean()
    {
        isNavMeshAgentEnabled  = false;
    }

    //Function to remove destroyed bullets from list for enemies that shoot bullets
    public virtual void RemoveFromList(Bullet bullet)
    {
        //To be overriden
    }
}
