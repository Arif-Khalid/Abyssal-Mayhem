using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeelee : MonoBehaviour
{
    public int meeleeDamage = 10;
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(meeleeDamage);
            this.gameObject.SetActive(false);
        }
    }
}
