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

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        weaponIK = GetComponent<WeaponIK>();
    }
    protected override void LateUpdate()
    {
        if (player)
        {
            AttackPlayer();
        }
    }

    protected override void AttackPlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(headTransform.position, (player.position - headTransform.position), out hit, 50, playerAndObstacles))
        {
            Debug.DrawRay(headTransform.position, (player.position-headTransform.position) * 50);
            if (hit.collider.GetComponent<PlayerHealth>())
            {
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
}
