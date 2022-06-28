using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeelee : MonoBehaviour
{
    public int meeleeDamage = 10;
    public float meeleeShakeDuration = 0.15f;
    public float meeleeShakeMagnitude = 0.4f;
    [SerializeField] string indicatorID;
    [SerializeField] float strength;
    bool isPlayerDead = false;
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (playerHealth.dead) { isPlayerDead = true; }
            playerHealth.TakeDamage(meeleeDamage, meeleeShakeDuration, meeleeShakeMagnitude);
            if(!isPlayerDead) { IndicatorProManager.Activate(indicatorID, transform.root.position, strength); }
            this.gameObject.SetActive(false);
        }
    }
}
