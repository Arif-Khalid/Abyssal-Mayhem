using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Interactable
{
    public ChestContent chestContent;
    protected override void Interact()
    {
        //Do something like an animation or sound or destroy itself
        chestContent.MakeAvailable();
        AudioManager.instance.Play("WeaponPickup");
        gameObject.SetActive(false);     
    }
}

