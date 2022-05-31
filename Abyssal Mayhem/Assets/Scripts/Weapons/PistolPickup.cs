using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolPickup : Interactable
{
    protected override void Interact()
    {
        //Do something like an animation or sound or destroy itself
        Destroy(this.gameObject);
    }
}

