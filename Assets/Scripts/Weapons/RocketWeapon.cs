using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWeapon : Weapon
{
    //Default Rocket weapon with default weaponnew implementations

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
        playerWeapon.Equip(playerWeapon.defaultWeapon);
    }

    //Function that doesn't do anything since using the same empty anim as laser
    public void NotAimed()
    {
    }
}
