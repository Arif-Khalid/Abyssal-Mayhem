using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautBullet : Bullet
{
    public override void HitSomething(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(bulletDamage);
        }
        if (enemyAI)
        {
            enemyAI.RemoveFromList(this);
        }
        Destroy(this.gameObject);
    }
}