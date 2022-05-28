using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    PlayerWeapon playerWeapon; //Reference to player weapon script
    public Transform bulletPoint;//Where the bullets come from, set in inspector in prefab
    [SerializeField] GameObject bullet; //Reference to bullet prefab
    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponentInParent<PlayerWeapon>();
    }

    //Fire a bullet
    //Default implementation spawns a bullet a predefined bulletPoint facing towards aimTransform of player weapon script
    public void Fire()
    {
        Vector3 dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
        Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
    }

}
