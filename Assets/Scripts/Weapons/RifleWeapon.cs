using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleWeapon : Weapon
{
    //Default Rifle weapon with default weapon implementations

   
    //Plays animation for close to wall and its reverse
    protected override void CloseToWall()
    {
        animator.SetFloat("Speed", 1.5f);
        base.CloseToWall();
    }

    protected override void NotCloseToWall()
    {
        animator.SetFloat("Speed", -10f);
        base.NotCloseToWall();
    }

    protected override void OutOfAmmo()
    {
        maxAmmo = -1; //Should never be called for Rifle but just in case set ammo to infinite
    }

    //Function that does not do anything since using same empty animation as laser
    public void NotAimed()
    {
    }

}
