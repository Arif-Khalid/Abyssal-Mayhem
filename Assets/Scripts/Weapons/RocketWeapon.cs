using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWeapon : Weapon
{
    [SerializeField] SkinnedMeshRenderer rocketBulletMesh;

    public void EquipEnableRocketBulletMesh()
    {
        if(currentAmmo > 0)
        {
            rocketBulletMesh.enabled = true;
        }
        else
        {
            rocketBulletMesh.enabled = false;
        }
    }
    public void EnableRocketBulletMesh()
    {
        rocketBulletMesh.enabled = true;
    }

    public void DisableRocketBulletMesh()
    {
        rocketBulletMesh.enabled = false;
    }
    
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

    //Function that doesn't do anything since using the same empty anim as laser
    public void NotAimed()
    {
    }

}
