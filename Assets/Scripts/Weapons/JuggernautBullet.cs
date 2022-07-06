using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautBullet : Bullet
{
    public float bulletShakeDuration = 0.15f;
    public float bulletShakeMagnitude = 0.4f;
    [SerializeField] string indicatorID;
    [SerializeField] float strength;
    
    public override void HitSomething(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            if (!playerHealth.dead) 
            {
                if (enemyAI) { IndicatorProManager.Activate(indicatorID, enemyAI.transform.position, strength); }
                else { IndicatorProManager.Activate(indicatorID, shooterPosition, strength); }
                AudioManager.instance.Play(indicatorID);
            }
            playerHealth.TakeDamage(bulletDamage, bulletShakeDuration, bulletShakeMagnitude);
        }
        if (enemyAI)
        {
            
            enemyAI.RemoveFromList(this);
        }
        gameObject.SetActive(false);
    }
}
