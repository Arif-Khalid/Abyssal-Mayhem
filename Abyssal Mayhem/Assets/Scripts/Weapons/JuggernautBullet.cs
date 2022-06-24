using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautBullet : Bullet
{
    public float bulletShakeDuration = 0.15f;
    public float bulletShakeMagnitude = 0.4f;
    public override void HitSomething(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(bulletDamage, bulletShakeDuration, bulletShakeMagnitude);
        }
        if (enemyAI)
        {
            enemyAI.RemoveFromList(this);
        }
        Destroy(this.gameObject);
    }
}
