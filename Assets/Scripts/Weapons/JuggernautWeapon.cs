using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautWeapon : Weapon
{
    [SerializeField] Transform[] bulletPoints;
    [SerializeField] Animator[] muzzleAnimators;
    private bool fireRight = true;
    public override void Fire()
    {
        if (!allowShooting || reloading || isFiring || isInUI)
        {
            return;
        }
        else
        {
            if (currentAmmo <= 0) //if no ammo to shoot
            {
                if (maxAmmo == 0) //if no ammo at all in gun reserves
                {
                    OutOfAmmo();
                    return;
                }
                Reload();
                return;
            }
            if (fireRight)
            {
                Debug.Log("firing right");
                bulletPoint = bulletPoints[0];
                muzzleAnimators[0].Play("MuzzleFlashHomemade");
                animator.Play(weaponName + "FireRight");
            }
            else
            {
                Debug.Log("firing left");
                bulletPoint = bulletPoints[1];
                muzzleAnimators[1].Play("MuzzleFlashHomemade");
                animator.Play(weaponName + "FireLeft");
            }            
            currentAmmo -= 1;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
            fireRight = !fireRight;
        }
    }
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

    public override void Reload()
    {
        //Do nothing here since this weapon does not reload
    }
    //Function called my empty anim since using same empty as laser
    public void NotAimed()
    {
    }
}
