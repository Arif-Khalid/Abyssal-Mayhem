using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeelee : MonoBehaviour
{
    public int meeleeDamage = 10;
    public float meeleeShakeDuration = 0.15f;
    public float meeleeShakeMagnitude = 0.4f;
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(meeleeDamage, meeleeShakeDuration, meeleeShakeMagnitude);
            this.gameObject.SetActive(false);
        }
    }
}