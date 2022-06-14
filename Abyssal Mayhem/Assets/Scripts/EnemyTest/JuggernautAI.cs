using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JuggernautAI : EnemyAI
{
    public GameObject enemyBullet;
    public Transform[] bulletPoints = new Transform[2];
    public LayerMask playerAndObstacles;
    List<Bullet> bullets = new List<Bullet>();

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
        if (Physics.Raycast(transform.position, transform.forward, out hit, 50, playerAndObstacles))
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
        for (int i = 0; i < bulletPoints.Length; i++)
        {
            Bullet spawnedBullet = Instantiate<GameObject>(enemyBullet, bulletPoints[i].position, Quaternion.LookRotation(player.position - bulletPoints[i].position)).GetComponent<Bullet>();
            bullets.Add(spawnedBullet);
            spawnedBullet.enemyAI = this;
        }
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

    public override void RemoveFromList(Bullet bullet)
    {
        bullets.Remove(bullet);
    }
}
