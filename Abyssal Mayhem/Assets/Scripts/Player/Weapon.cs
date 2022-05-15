using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerWeapon playerWeapon;
    public Transform bulletPoint;//Where the bullets come from
    [SerializeField] GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponentInParent<PlayerWeapon>();
    }

    //Fire a bullet
    public void Fire()
    {
        Vector3 dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
        Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
    }

}
