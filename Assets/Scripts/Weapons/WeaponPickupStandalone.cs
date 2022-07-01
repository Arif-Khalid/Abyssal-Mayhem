using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupStandalone : Interactable
{
    protected override void Interact()
    {
        gameObject.SetActive(false);
    }
}
