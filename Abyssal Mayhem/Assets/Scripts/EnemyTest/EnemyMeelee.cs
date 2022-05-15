using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeelee : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.TakeDamage(10);
        }
    }
}
