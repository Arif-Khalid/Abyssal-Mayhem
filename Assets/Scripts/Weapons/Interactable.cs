using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactMessage;
    public string defaultMessage;
    public Collider interactCollider;
    public PlayerWeapon.PlayerWeapons weaponToEquip;
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        //To be overriden
    }

    public void ResetInteract()
    {
        interactCollider.enabled = true;
        interactMessage = defaultMessage;
    }
}
