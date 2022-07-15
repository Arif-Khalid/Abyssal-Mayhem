using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautBullet : Bullet
{
    public float bulletShakeDuration = 0.15f;
    public float bulletShakeMagnitude = 0.4f;
    [SerializeField] string indicatorID;
    [SerializeField] float strength;
    public int numberOfHitSounds;
    

    public override void DealDamage(Collider other)
    {
        damageDealt = true;
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            if (!playerHealth.dead)
            {
                if (enemyAI) { IndicatorProManager.Activate(indicatorID, enemyAI.transform.position, strength); }
                else { IndicatorProManager.Activate(indicatorID, shooterPosition, strength); }
                AudioManager.instance.Play(indicatorID + Random.Range(0, numberOfHitSounds).ToString());
                playerHealth.TakeDamage(bulletDamage, bulletShakeDuration, bulletShakeMagnitude);
            }
        }
        else
        {
            HitWallSound();
        }
        if (enemyAI)
        {

            enemyAI.RemoveFromList(this);
        }
        Invoke(nameof(DisableGameObject), 1f);
    }
}
