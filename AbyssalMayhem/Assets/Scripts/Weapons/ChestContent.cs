using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestContent : MonoBehaviour
{
    public GameObject chestContent;
    public GameObject contentInstance;
    public Transform createTransform;
    public Animator animator;
    public EnemySpawner enemySpawner;
    public Interactable chestInteract;

    private void Start()
    {
        animator = GetComponent<Animator>();;
    }
    
    //Function called by enemySpawner to reset contents of chest
    //Returns true if successfully reset and false otherwise
    public bool ResetContent(GameObject newContent)
    {
        if(!contentInstance && !chestContent)
        {
            //enter if content has been spawned and spawned content has been taken
            chestContent = newContent; //reset content with new content
            animator.SetBool("openChest", false); //close back the chest
            return true;
        }
        else
        {
            return false;
        }
    }
    public void CreateContent()
    {
        if (chestContent)
        {
            contentInstance = Instantiate<GameObject>(chestContent, createTransform);
            PistolPickup pistolPickup = contentInstance.GetComponent<PistolPickup>();
            if (pistolPickup)
            {
                pistolPickup.chestContent = this;
            }
            else
            {
                PowerupPickup powerupPickup = contentInstance.GetComponentInChildren<PowerupPickup>();
                powerupPickup.chestContent = this;
            }
            
        }      
        chestContent = null;
    }

    public void MakeAvailable()
    {
        enemySpawner.MakeWeaponChestAvailable(this);
    }

    public void MakeInteractable()
    {
        chestInteract.ResetMessage();
    }

    public void HardReset()
    {
        chestContent = null;
        Destroy(contentInstance);
        animator.SetBool("openChest", true);
        chestInteract.interactMessage = string.Empty;
    }
}
