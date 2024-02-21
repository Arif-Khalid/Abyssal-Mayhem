using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon; //stores current active weapon
    public PlayerWeapons defaultWeapon; //stores default rifle weapon
    [SerializeField] public Transform weaponSlot; //Where weapons are equipped to
    [SerializeField] PlayerUI playerUI;

    
    //Aim variables
    [SerializeField] Transform cameraTransform; //Transform of the camera
    [SerializeField] float distance = 50f; //Distance of raycast to move aim transform
    [SerializeField] LayerMask layerMask; //What aim transform will move to when faced
    public Transform aimTransform; //Where bullets should head

    float fireRate = 1f;
    int currentWeaponID = 0; //rifle is 0, shotgun is 1, laser is 2, rocket is 3, jug weapon is 4

    [SerializeField] GameObject testWeapon;

    [Header("Weapons")]
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject shotgun;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject juggernautWeapon;
    GameObject[] availableWeapons = new GameObject[5];
    Sprite defaultAmmoSprite;
    public enum PlayerWeapons { Rifle, Shotgun, Laser, Rocket, JuggernautWeapon, None }

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        weapon.NewWeaponEquip();
        playerUI.UpdateAmmoText(25, -1);
        playerUI.EnableWeaponUI(currentWeaponID);
        playerUI.SetActiveWeaponUI(currentWeaponID);
        availableWeapons[0] = rifle;
        //availableWeapons[1] = shotgun;
        //availableWeapons[2] = laser;
        //availableWeapons[3] = rocket;
        //availableWeapons[4] = juggernautWeapon;
    }

    //Equip a new weapon
    public void Equip(PlayerWeapons weaponName) 
    {
        //Do something in player UI on equip
        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
        }
        if (weaponName == PlayerWeapons.Rifle) 
        { 
            weapon = rifle.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.NewWeaponEquip();
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            currentWeaponID = 0;
            playerUI.EnableWeaponUI(currentWeaponID);
            playerUI.SetActiveWeaponUI(currentWeaponID);
        }
        else if(weaponName == PlayerWeapons.Shotgun)
        {
            availableWeapons[1] = shotgun;
            weapon = shotgun.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.NewWeaponEquip();
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            currentWeaponID = 1;
            playerUI.EnableWeaponUI(currentWeaponID);
            playerUI.SetActiveWeaponUI(currentWeaponID);
        }
        else if (weaponName == PlayerWeapons.Laser) 
        {
            availableWeapons[2] = laser;
            weapon = laser.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.NewWeaponEquip();
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            if (fireRate == 1.0f) { weapon.GetComponent<LaserWeapon>().fireRate = (1.6f - fireRate) / 2; }
            else { weapon.GetComponent<LaserWeapon>().fireRate = (1.9f - fireRate) / 2; }
            currentWeaponID = 2;
            playerUI.EnableWeaponUI(currentWeaponID);
            playerUI.SetActiveWeaponUI(currentWeaponID);
        }
        else if (weaponName == PlayerWeapons.Rocket) 
        {
            availableWeapons[3] = rocket;
            weapon = rocket.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.NewWeaponEquip();
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            currentWeaponID = 3;
            playerUI.EnableWeaponUI(currentWeaponID);
            playerUI.SetActiveWeaponUI(currentWeaponID);
        }
        else if (weaponName == PlayerWeapons.JuggernautWeapon) 
        {
            availableWeapons[4] = juggernautWeapon;
            weapon = juggernautWeapon.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.NewWeaponEquip();
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            currentWeaponID = 4;
            playerUI.EnableWeaponUI(currentWeaponID);
            playerUI.SetActiveWeaponUI(currentWeaponID);
        }
    }

    private void ActivateWeapon(int ID)
    {
        weapon.gameObject.SetActive(false);
        weapon = availableWeapons[ID].GetComponent<Weapon>();
        weapon.playerWeapon = this;
        weapon.gameObject.SetActive(true);
        if (ID != 2) { weapon.animator.SetFloat("FireRate", fireRate); }//For not laser weapon
        else
        {
            //For laser weapon
            if (fireRate == 1.0f) { weapon.GetComponent<LaserWeapon>().fireRate = (1.6f - fireRate) / 2; }
            else { weapon.GetComponent<LaserWeapon>().fireRate = (1.9f - fireRate) / 2; }
        }
        playerUI.SetActiveWeaponUI(currentWeaponID);
    }

    public void DeactivateCurrentWeapon()
    {
        //do something in player UI
        playerUI.DisableWeaponUI(currentWeaponID);
        weapon.gameObject.SetActive(false);
        availableWeapons[currentWeaponID] = null;
        currentWeaponID = 0;
        ActivateWeapon(currentWeaponID);
    }

    public void ResetWeapons()
    {
        //Do not remove rifle at index 0
        for(int i = 1; i < availableWeapons.Length; i++)
        {
            availableWeapons[i] = null;
            playerUI.DisableWeaponUI(i);
        }
        Equip(defaultWeapon);
    }
    
 
    // Update is called once per frame
    void Update()
    {
        if(Input.mouseScrollDelta.y > 0)
        {
            ActivateUpperWeapon(currentWeaponID + 1);
        }else if(Input.mouseScrollDelta.y < 0)
        {
            ActivateLowerWeapon(currentWeaponID - 1);
        }
        if (weapon)
        {
            ShiftAimTransform();
            if (!weapon.isLaser && Input.GetKey(KeyCode.Mouse0))
            {
                weapon.Fire();
            }
            if (Input.GetKeyDown(KeyCode.R) && !PlayerManager.localPlayerSetup.chatUI.inputField.enabled)
            {
                weapon.Reload();
            }
        }
    }

    private void ActivateUpperWeapon(int tempID)
    {
        if(tempID > 4) { tempID = 0; }
        if(tempID == currentWeaponID) { return; }
        if(availableWeapons[tempID] != null) { currentWeaponID = tempID; ActivateWeapon(currentWeaponID); }
        else { ActivateUpperWeapon(tempID + 1); }
    }

    private void ActivateLowerWeapon(int tempID)
    {
        if(tempID < 0) { tempID = 4; }
        if(tempID == currentWeaponID) { return; }
        if(availableWeapons[tempID] != null) { currentWeaponID = tempID; ActivateWeapon(currentWeaponID); }
        else { ActivateLowerWeapon(tempID - 1); }
    }
    //Called in Update to shift the aim transform object to where player camera is facing
    private void ShiftAimTransform()
    {
        RaycastHit hit;
        //Find where player is aiming
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, distance, layerMask))
        {
            aimTransform.position = hit.point;
        }
        else
        {
            //Default the aimTransform as right in front of the gun
            aimTransform.position = Camera.main.transform.position + Camera.main.transform.forward * 200;
        }
    }

    //Called by invincibility powerup that changes fire rate of current and future equipped weapons
    public void ChangeFireRate(float newFireRate)
    {
        fireRate = newFireRate;
        weapon.animator.SetFloat("FireRate", fireRate);
        if (weapon.isLaser)
        {
            if (fireRate == 1.0f) { weapon.GetComponent<LaserWeapon>().fireRate = (1.6f - fireRate) / 2; }
            else { weapon.GetComponent<LaserWeapon>().fireRate = (1.9f - fireRate) / 2; }
        }
    }

}
