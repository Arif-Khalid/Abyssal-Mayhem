using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolWeapon : Weapon
{
    //Default pistol weapon with default weapon implementations

    //Plays animation for close to wall and its reverse
    public override void CloseToWall()
    {
        animator.SetFloat("Speed", 1.5f);
        base.CloseToWall();
    }

    public override void NotCloseToWall()
    {
        animator.SetFloat("Speed", -10f);
        base.NotCloseToWall();
    }

    protected override void OutOfAmmo()
    {
        maxAmmo = -1; //Should never be called for pistol but just in case set ammo to infinite
    }
}
