using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestContent : MonoBehaviour
{
    public GameObject chestContent;
    public Transform createTransform;
    public Animator animator;
    public EnemySpawner enemySpawner;
    public Interactable chestInteract;

    [Header("Weapons")]
    [SerializeField] WeaponPickup[] weaponPickups;


    [Header("Powerups")]
    [SerializeField] PowerupPickup[] powerupPickups;
    private void Start()
    {
        animator = GetComponent<Animator>();;
    }
    
    //Function called by enemySpawner to reset contents of chest
    public void ResetContent(bool spawnWeapon)
    {
        if (spawnWeapon)
        {
            int spawnID = Random.Range(0, weaponPickups.Length);
            chestContent = weaponPickups[spawnID].gameObject;
        }
        else
        {
            int spawnID = Random.Range(0, powerupPickups.Length);
            chestContent = powerupPickups[spawnID].gameObject;
        }
        animator.SetBool("openChest", false); //close back the chest
    }

    public void CreateContent()
    {
        if (chestContent)
        {
            chestContent.SetActive(true);
        }      
    }

    public void MakeAvailable()
    {
        enemySpawner.MakeWeaponChestAvailable(this);
        chestContent = null;
    }

    public void MakeInteractable()
    {
        chestInteract.ResetInteract();
    }

    public void HardReset()
    {
        if (chestContent)
        {
            chestContent.SetActive(false);
            chestContent = null;
        }
        animator.SetBool("openChest", true);
        chestInteract.interactMessage = string.Empty;
    }
}
