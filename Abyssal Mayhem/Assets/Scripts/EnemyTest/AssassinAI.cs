using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AssassinAI : EnemyAI
{
    public GameObject sniperBullet;
    public Transform aimTransform;
    public Transform headTransform;
    public LayerMask playerAndObstacles;
    WeaponIK weaponIK;
    Transform startingTransform;
    Animator animator;
    LineRenderer laserSight;
    public Transform laserSightOrigin;
    public bool isPatrolling;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        weaponIK = GetComponent<WeaponIK>();
        animator = GetComponent<Animator>();
        startingTransform = transform;
        agent.SetDestination(startingTransform.position);
        laserSight = GetComponent<LineRenderer>();
    }

    //Handles when assassin is moving or stopped
    protected override void LateUpdate()
    {
        if(agent.velocity.magnitude > 0.01f)
        {
            weaponIK.AssassinMoving();
            animator.SetBool("IsMoving", true);
            RaycastHit hit;
            if(Physics.Raycast(laserSightOrigin.position, laserSightOrigin.forward, out hit, 200, playerAndObstacles))
            {
                UpdateLaser(hit.point);
            }
            else
            {
                UpdateLaser(laserSightOrigin.position + (laserSightOrigin.forward.normalized * 200));
            }
        }
        if (player && agent.velocity.magnitude <= 0.01f)
        {
            animator.SetBool("IsMoving", false);
            transform.rotation = startingTransform.rotation;
            weaponIK.AssassinStopped();
            AttackPlayer();
        }
    }

    protected override void AttackPlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(headTransform.position, (player.position - headTransform.position), out hit, attackRange, playerAndObstacles))
        {
            UpdateLaser(hit.point);
            Debug.DrawRay(headTransform.position, (player.position-headTransform.position) * 50);
            if (hit.collider.GetComponent<PlayerHealth>())
            {
                isPatrolling = false;
                if(!alreadyAttacked && weaponIK.CanAssassinShoot())
                {
                    Attack();
                    alreadyAttacked = true;
                    Invoke(nameof(ResetAttack), timeBetweenAttacks);
                }
            }
            else
            {
                weaponIK.PlayerOutOfSight();
            }
        }
        else
        {
            weaponIK.PlayerOutOfSight();
        }
    }
    public override void Attack()
    {
        Debug.Log("Attack");
        Instantiate<GameObject>(sniperBullet, aimTransform.position, Quaternion.LookRotation(player.position - aimTransform.position));
    }

    //Update laser sight position 
    void UpdateLaser(Vector3 endPos)
    {
        if (isPatrolling)
        {
            return;
        }
        laserSight.SetPosition(0, laserSightOrigin.position);
        laserSight.SetPosition(1, endPos);
    }
}
