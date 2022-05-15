using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth)
        {
            playerHealth.RestoreHealth(10);
        }
    }
}
