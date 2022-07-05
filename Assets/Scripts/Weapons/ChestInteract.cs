using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteract : Interactable
{
    public Animator animator;
    protected override void Interact()
    {
        animator.SetBool("openChest", true); //open the chest
        interactCollider.enabled = false;
        interactMessage = string.Empty;
    }

}
