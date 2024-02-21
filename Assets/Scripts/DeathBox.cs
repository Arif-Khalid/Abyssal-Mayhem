using UnityEngine;

/**
 * Responsible for killing the player when they jump off the map
 * Attached to a big collider below the map
 */
public class DeathBox : MonoBehaviour
{
    [SerializeField] private string _deathByName = "falling to their death";    // What is displayed when player dies
    private void OnTriggerEnter(Collider other) {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        if (enemyHealth) {
            enemyHealth.Death();
            return;
        }
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth) {
            playerHealth.Death(_deathByName);
            return;
        }
    }
}
