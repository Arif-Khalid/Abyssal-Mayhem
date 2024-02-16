using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JuggernautAI : EnemyAI
{
    public GameObject enemyBullet;
    public Transform[] bulletPoints = new Transform[2];
    public LayerMask playerAndObstacles;
    public Animator animator;
    public Animator muzzleLeftAnimator;
    public Animator muzzleRightAnimator;
    bool fireRight = true;
    List<Bullet> bullets = new List<Bullet>();
    public string bulletTag;
    [SerializeField] AudioSource shootingSource;
    [SerializeField] string bulletDeathByName;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }
    protected override void AttackPlayer()
    {
        transform.LookAt(player);
        RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.forward * 10, Color.red, 10, false);
        if (Physics.Raycast(bulletPoints[0].position, player.position - bulletPoints[0].position, out hit, attackRange, playerAndObstacles) || Physics.Raycast(bulletPoints[1].position, player.position - bulletPoints[1].position, out hit, attackRange, playerAndObstacles)) //Check if bullet points have LOS to player
        {
            if (hit.collider.GetComponent<PlayerHealth>())
            {
                base.AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            ChasePlayer();
        }
    }
    public override void Attack()
    {
        shootingSource.Play();
        int pointID = fireRight ? 0 : 1;
        //Play the muzzleflash
        Bullet spawnedBullet = ObjectPooler.Instance.SpawnFromPool(bulletTag, bulletPoints[pointID].position, Quaternion.LookRotation(player.position - bulletPoints[pointID].position)).GetComponent<Bullet>();//Instantiate<GameObject>(enemyBullet, bulletPoints[pointID].position, Quaternion.LookRotation(player.position - bulletPoints[pointID].position)).GetComponent<Bullet>();
        bullets.Add(spawnedBullet);
        spawnedBullet.enemyAI = this;
        spawnedBullet.shooterPosition = transform.position;
        spawnedBullet.GetComponent<JuggernautBullet>().deathByName = bulletDeathByName;
        //Play the animation
        if (fireRight) { animator.Play("FireRight"); muzzleRightAnimator.Play("MuzzleFlashHomemade"); }
        else { animator.Play("FireLeft"); muzzleLeftAnimator.Play("MuzzleFlashHomemade"); }
        fireRight = !fireRight;
    }

    private void OnDestroy()
    {
        foreach(Bullet bullet in bullets)
        {
            if (bullet)
            {
                bullet.EndOfExistence();
            }           
        }
        bullets.Clear();
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
    }

    public override void RemoveFromList(Bullet bullet)
    {
        bullets.Remove(bullet);
    }
}
