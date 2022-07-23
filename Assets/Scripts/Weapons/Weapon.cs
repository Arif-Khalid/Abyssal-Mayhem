using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public PlayerWeapon playerWeapon; //Reference to player weapon script
    public Transform bulletPoint;//Where the bullets come from, set in inspector in prefab
    [SerializeField]public GameObject bullet; //Reference to bullet prefab
    [SerializeField] public int maxAmmo = -1; //Storing the max ammo of the current weapon instance
    [SerializeField] int clipSize = 10; //Size of weapon clip
    [SerializeField] int defaultMaxAmmo = -1; //The max ammo of the weapon when equipped, -1 means infinite ammo
    protected int currentAmmo; //current ammo in weapon
    public PlayerUI playerUI;
    public Animator animator;
    public Animator muzzleAnimator;
    public Vector3 dir;
    protected bool closeToWall = false;
    public LayerMask whatIsNotPlayer;
    protected bool allowShooting = true;
    [SerializeField]protected bool reloading = true;
    [SerializeField]public bool isLaser;
    [SerializeField] bool isRocket;
    public bool isFiring = false;
    public string weaponName;
    public bool isInUI = false;
    [SerializeField] AmmoImage ammoImage;

    public string spawnTag;
    // Start is called before the first frame update
    void Start()
    {        
        ChildStart();
    }

    private void OnEnable()
    {       
        playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        playerUI.UpdateAmmoSprite(ammoImage);
    }

    public void NewWeaponEquip()
    {
        maxAmmo = defaultMaxAmmo;
        currentAmmo = clipSize;
    }
    //Start function to be overwriten by children should anything be needed to be added to start
    protected virtual void ChildStart()
    {
        //To be overriden
    }
    private void Update()
    {
        if(Physics.CheckSphere(playerWeapon.weaponSlot.position, 0.45f, whatIsNotPlayer))
        {
            CloseToWall();
        }
        else
        {
            NotCloseToWall();     
        }
        ChildUpdate();
    }

    //Function to be overriden by children should anything be needed to be called in update
    protected virtual void ChildUpdate()
    {
        //To be overriden
    }

    //Start reloading weapon
    public virtual void Reload()
    {
        if(isFiring || reloading || currentAmmo == clipSize)
        {
            return;
        }
        if(maxAmmo == 0) //cant reload if no ammo left or nothing to reload
        {
            if(currentAmmo == 0) { OutOfAmmo(); }
            return;
        }
        if(maxAmmo < -1) //should never reach this point but just in case
        {
            playerWeapon.Equip(playerWeapon.defaultWeapon);
        }
        reloading = true;
        //Play some animation that calls reload finish on its last frame
        //For now just put ReloadFinish here
        animator.Play(weaponName +"Reload");
    }

    //Called in the middle of reload animation to signal when ammo is reloaded like when clip enters the gun
    public void RestoreAmmo() 
    {
        int ammoToReload = clipSize - currentAmmo;
        if (maxAmmo >= ammoToReload) //if enough ammo to reload full clip
        {
            maxAmmo -= ammoToReload;
            currentAmmo = clipSize;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
        else if (maxAmmo > 0) //if enough ammo to reload partial clip
        {
            currentAmmo += maxAmmo;
            maxAmmo = 0;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
        else if (maxAmmo == -1) //if max ammo is set to infinity
        {
            currentAmmo = clipSize;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
    }
    //Called at the end of the reload animation
    public void ReloadFinish()
    {
        reloading = false;
    }
    //Fire a bullet
    //Default implementation spawns a bullet a predefined bulletPoint facing towards aimTransform of player weapon script
    public virtual void Fire()
    {     
        if (!allowShooting || reloading || isFiring || isInUI)
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
            muzzleAnimator.Play("MuzzleFlashHomemade");
            animator.Play(weaponName + "Fire");
            currentAmmo -= 1;
            playerUI.UpdateAmmoText(currentAmmo, maxAmmo);
        }
    }

    protected virtual void FireBullet()
    {
        dir = playerWeapon.aimTransform.position - bulletPoint.position; //Get direction of where to aim
        //Instantiate(bullet, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir)); //Fire at bulletPoint, facing the direction found
        ObjectPooler.Instance.SpawnFromPool(spawnTag, new Vector3(bulletPoint.position.x, bulletPoint.position.y, bulletPoint.position.z), Quaternion.LookRotation(dir));
    }
    protected virtual void OutOfAmmo()
    {
        playerWeapon.DeactivateCurrentWeapon();
    }
    protected virtual void CloseToWall()
    {       
        closeToWall = true;
        DisableShooting();
    }

    protected virtual void NotCloseToWall()
    {        
        closeToWall = false;
        EnableShooting();
    }

    //Called when performing animations of weapon
    public void DisableShooting()
    {
        allowShooting = false;
    }

    //Called in empty initial state of weapon animation
    public virtual void EnableShooting()
    {
        allowShooting = true;
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
        if (isLaser)
        {
            animator.Play("AimDownSight", 0, 1f);
        }
    }

    public void FinishedEquipAnim()
    {
        reloading = false;
    }

    public void EnterUI()
    {
        isInUI = true;
    }

    public void ExitUI()
    {
        isInUI = false;
    }

    private void OnDisable()
    {
        isInUI = false;
        reloading = true;
        isFiring = false;
        ChildOnDisable();
    }

    protected virtual void ChildOnDisable()
    {
        //TO be overriden
    }

    //Plays specific weapon sound called in animation
    public virtual void FiringSound()
    {
        AudioManager.instance.Play(weaponName + "Fire");
    }

    public virtual void RemoveClipSound()
    {
        AudioManager.instance.Play(weaponName + "RemoveClip");
    }

    public virtual void InsertClipSound()
    {
        AudioManager.instance.Play(weaponName + "InsertClip");
    }

    public virtual void EquipSound()
    {
        AudioManager.instance.Play(weaponName + "Equip");
    }
}
