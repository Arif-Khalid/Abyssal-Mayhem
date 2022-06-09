using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlayerWeapon playerWeapon; //Reference to player weapon script
    public Transform bulletPoint;//Where the bullets come from, set in inspector in prefab
    [SerializeField] GameObject bullet; //Reference to bullet prefab
    [SerializeField] public int maxAmmo = -1; //Set to -1 when infinite ammo
    [SerializeField] int clipSize = 10; //Size of weapon clip
    int currentAmmo; //current ammo in weapon
    PlayerUI playerUI;
    public Animator animator;
    Vector3 dir;
    private bool closeToWall = false;
    public LayerMask whatIsNotPlayer;
    private bool allowShooting = true;
    private bool reloading = false;

    public ParticleSystem muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
        playerWeapon = GetComponentInParent<PlayerWeapon>();
        playerUI = GetComponentInParent<PlayerUI>();
        animator = GetComponent<Animator>();
        currentAmmo = clipSize;
        playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
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

    //Start reloading weapon
    public void Reload()
    {
        if(maxAmmo == 0 || currentAmmo == clipSize) //cant reload if no ammo left
        {
            return;
        }
        if(maxAmmo < -1) //should never reach this point but just in case
        {
            playerWeapon.Equip(playerWeapon.defaultWeapon);
        }
        reloading = true;
        //Play some animation that calls reload finish on its last frame
        //For now just put ReloadFinish here
        ReloadFinish();
    }

    //Called at the end of the reload animation
    public void ReloadFinish()
    {
        if(maxAmmo >= clipSize) //if enough ammo to reload full clip
        {
            maxAmmo -= clipSize;
            currentAmmo = clipSize;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }else if(maxAmmo > 0) //if enough ammo to reload partial clip
        {
            currentAmmo += maxAmmo;
            maxAmmo = 0;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
        else if(maxAmmo == -1) //if max ammo is set to infinity
        {
            currentAmmo = clipSize;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
        reloading = false;
    }
    //Fire a bullet
    //Default implementation spawns a bullet a predefined bulletPoint facing towards aimTransform of player weapon script
    public virtual void Fire()
    {
        muzzleFlash.Play();
        if (!allowShooting || reloading)
        {
            return;
        }
        else
        {
            if(currentAmmo <= 0) //if no ammo to shoot
            {
                if(maxAmmo == 0) //if no ammo at all in gun reserves
                {
                    OutOfAmmo();
                    return;
                }
                Reload();
                return;
            }
            currentAmmo -= 1;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
            dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
            Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
        }
    }

    protected virtual void OutOfAmmo()
    {
        //To be overriden in inherited class
        //Function to call when no more ammo in clip or reserves and trying to shoot
    }
    protected virtual void CloseToWall()
    {
        DisableShooting();
        closeToWall = true;
    }

    protected virtual void NotCloseToWall()
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
