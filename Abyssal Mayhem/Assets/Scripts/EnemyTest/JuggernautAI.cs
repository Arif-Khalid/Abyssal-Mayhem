using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JuggernautAI : EnemyAI
{
    public GameObject enemyBullet;
    public Transform[] bulletPoints = new Transform[2];
    public LayerMask playerAndObstacles;

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
            Instantiate<GameObject>(enemyBullet, bulletPoints[i].position, Quaternion.LookRotation(player.position - bulletPoints[i].position));
        }
    }
}
