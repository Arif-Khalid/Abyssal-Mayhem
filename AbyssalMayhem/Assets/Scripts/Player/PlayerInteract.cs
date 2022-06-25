using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    //Script for interacting with pickups and powerups
    PlayerUI playerUI;
    PlayerWeapon playerWeapon;
    Camera cam;
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        cam = GetComponentInChildren<Camera>();
        playerWeapon = GetComponent<PlayerWeapon>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit,  interactDistance, interactableLayer))
        {
            Debug.Log("interactable detected");
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            playerUI.UpdateInteractPrompt(interactable.interactMessage);
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (interactable.weaponToEquip)
                {
                    playerWeapon.Equip(interactable.weaponToEquip);
                }                
                interactable.BaseInteract();
            }
        }
        else
        {
            playerUI.UpdateInteractPrompt(string.Empty);
        }
    }
}
