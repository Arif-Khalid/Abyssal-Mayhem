using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AssassinAI : EnemyAI
{
    public GameObject sniperBullet;
    public string sniperBulletTag;
    public Transform aimTransform;
    public Transform headTransform;
    public LayerMask playerAndObstacles;
    WeaponIK weaponIK;
    public Transform startingTransform;
    Animator animator;
    public LineRenderer laserSight;
    public Transform laserSightOrigin;
    public bool isPatrolling;
    List<Bullet> bullets = new List<Bullet>();
    public bool isBoss = false;
    [SerializeField] AudioSource shootSource;
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
            laserSight.enabled = false;
            weaponIK.AssassinMoving();
            return;
        }
        if(agent.velocity.magnitude > 0.01f)
        {
            weaponIK.AssassinMoving();
            animator.SetBool("IsMoving", true);
            laserSight.enabled = false;
        }
        if (player && agent.enabled && agent.desiredVelocity.magnitude <= 0.01f)
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
        if (isBoss) //always lock onto player if isBoss
        {
            UpdateLaser(player.position);
            isPatrolling = false;
            weaponIK.PlayerInSight();
            /*if (!alreadyAttacked && weaponIK.CanAssassinShoot())
            {
                Attack();
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }*/
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(headTransform.position, (player.position - headTransform.position), out hit, attackRange, playerAndObstacles))
            {
                UpdateLaser(hit.point);
                Debug.DrawRay(headTransform.position, (player.position - headTransform.position) * 50);
                if (hit.collider.GetComponent<PlayerHealth>())
                {
                    isPatrolling = false;
                    weaponIK.PlayerInSight();
                    /*if (!alreadyAttacked && weaponIK.CanAssassinShoot())
                    {
                        Attack();
                        alreadyAttacked = true;
                        Invoke(nameof(ResetAttack), timeBetweenAttacks);
                    }*/
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
    }
    public override void Attack()
    {
        shootSource.Play();
        Bullet spawnedBullet = ObjectPooler.Instance.SpawnFromPool(sniperBulletTag, aimTransform.position, Quaternion.LookRotation(player.position - aimTransform.position)).GetComponent<Bullet>();//Instantiate<GameObject>(sniperBullet, aimTransform.position, Quaternion.LookRotation(player.position - aimTransform.position)).GetComponent<Bullet>();
        bullets.Add(spawnedBullet);
        spawnedBullet.enemyAI = this;
        spawnedBullet.shooterPosition = transform.position;
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
        laserSight.enabled = false;
    }
    private void OnDestroy()
    {
        foreach (Bullet bullet in bullets)
        {
            if (bullet)
            {
                bullet.EndOfExistence();
            }           
        }
        bullets.Clear();
        laserSight.enabled = false;
    }

    public void ClearBullets()
    {
        foreach (Bullet bullet in bullets)
        {
            if (bullet)
            {
                bullet.EndOfExistence();
            }
        }
        bullets.Clear();
        laserSight.enabled = false;
    }

    public override void RemoveFromList(Bullet bullet)
    {
        bullets.Remove(bullet);
    }
}
