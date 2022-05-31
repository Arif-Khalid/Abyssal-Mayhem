using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlayerWeapon playerWeapon; //Reference to player weapon script
    public Transform bulletPoint;//Where the bullets come from, set in inspector in prefab
    [SerializeField] GameObject bullet; //Reference to bullet prefab
    public Animator animator;
    Vector3 dir;
    private bool closeToWall = false;
    public LayerMask whatIsNotPlayer;
    private bool allowShooting = true;
    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponentInParent<PlayerWeapon>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(Physics.CheckSphere(playerWeapon.weaponSlot.position, 0.5f, whatIsNotPlayer))
        {
            if (!closeToWall)
            {
                CloseToWall();
            }
        }
        else
        {
            if (closeToWall)
            {
                NotCloseToWall();
            }      
        }
    }
    //Fire a bullet
    //Default implementation spawns a bullet a predefined bulletPoint facing towards aimTransform of player weapon script
    public void Fire()
    {
        Debug.Log("Pistol firing");
        if (!allowShooting)
        {
            return;
        }
        else
        {
            Debug.Log("spawning bullets");
            dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
            Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
        }
    }

    public virtual void CloseToWall()
    {
        DisableShooting();
        closeToWall = true;
    }

    public virtual void NotCloseToWall()
    {
        closeToWall = false;
    }

    //Called when performing animations of weapon
    public void DisableShooting()
    {
        allowShooting = false;
    }

    //Called in empty initial state of weapon animation
    public void EnableShooting()
    {
        allowShooting = true;
    }
}
