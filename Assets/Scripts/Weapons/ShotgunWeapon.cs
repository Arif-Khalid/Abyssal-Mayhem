using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunWeapon : Weapon
{
    public float spread;
    public float numberOfBullets;
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

    protected override void FireBullet()
    {

        dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
        dir.Normalize();
        for(int i = 0; i < numberOfBullets; i++)
        {
            Debug.Log(i);
            dir.y += Random.Range(-spread, spread);
            dir.x += Random.Range(-spread, spread);
            dir.z += Random.Range(-spread, spread);
            Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
        }
        //Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
    }
    //Function that does not do anything since using same empty animation as laser
    public void NotAimed()
    {
    }

}
