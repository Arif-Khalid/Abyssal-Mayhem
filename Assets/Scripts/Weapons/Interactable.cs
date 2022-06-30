using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string interactMessage;
    public string defaultMessage;
    public PlayerWeapon.PlayerWeapons weaponToEquip;
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        //To be overriden
    }

    public void ResetMessage()
    {
        interactMessage = defaultMessage;
    }
}
