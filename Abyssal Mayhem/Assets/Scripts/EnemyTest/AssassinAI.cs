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
    public Transform startingTransform;
    Animator animator;
    LineRenderer laserSight;
    public Transform laserSightOrigin;
    public bool isPatrolling;
    List<Bullet> bullets = new List<Bullet>();

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        weaponIK = GetComponent<WeaponIK>();
        animator = GetComponent<Animator>();
        agent.SetDestination(startingTransform.position);
        laserSight = GetComponent<LineRenderer>();
    }

    //Handles when assassin is moving or stopped
    protected override void LateUpdate()
    {
        if (!isNavMeshAgentEnabled)
        {
            BounceBackRedo();
        }
        if(agent.velocity.magnitude > 0.01f)
        {
            weaponIK.AssassinMoving();
            animator.SetBool("IsMoving", true);
            laserSight.enabled = false;
        }
        if (player && agent.velocity.magnitude <= 0.01f)
        {
            animator.SetBool("IsMoving", false);
            transform.rotation = Quaternion.Slerp(transform.localRotation,startingTransform.rotation, Time.deltaTime);
            agent.SetDestination(startingTransform.position);
            weaponIK.AssassinStopped();
            AttackPlayer();
            //laserSight.SetPosition(0, laserSightOrigin.position);
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
        Bullet spawnedBullet = Instantiate<GameObject>(sniperBullet, aimTransform.position, Quaternion.LookRotation(player.position - aimTransform.position)).GetComponent<Bullet>();
        bullets.Add(spawnedBullet);
        spawnedBullet.enemyAI = this;
    }


    //Update laser sight position 
    void UpdateLaser(Vector3 endPos)
    {
        if (isPatrolling)
        {
            return;
        }
        laserSight.SetPosition(1, endPos);
        laserSight.enabled = true;
    }

    private void OnDisable()
    {
        foreach (Bullet bullet in bullets)
        {
            if (bullet)
            {
                bullet.EndOfExistence();
            }           
        }
        bullets.Clear();
    }

    public override void RemoveFromList(Bullet bullet)
    {
        bullets.Remove(bullet);
    }
}
