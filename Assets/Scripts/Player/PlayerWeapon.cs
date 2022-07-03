using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon; //stores current active weapon
    public PlayerWeapons defaultWeapon; //stores default rifle weapon
    [SerializeField] public Transform weaponSlot; //Where weapons are equipped to

    
    //Aim variables
    [SerializeField] Transform cameraTransform; //Transform of the camera
    [SerializeField] float distance = 50f; //Distance of raycast to move aim transform
    [SerializeField] LayerMask layerMask; //What aim transform will move to when faced
    public Transform aimTransform; //Where bullets should head

    float fireRate = 1f;

    [SerializeField] GameObject testWeapon;

    [Header("Weapons")]
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject shotgun;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject juggernautWeapon;
    public enum PlayerWeapons { Rifle, Shotgun, Laser, Rocket, JuggernautWeapon, None }

    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
    }

    //Equip a new weapon
    public void Equip(PlayerWeapons weaponName) 
    {
        if (weapon != null)
        {
            weapon.gameObject.SetActive(false);
        }
        if (weaponName == PlayerWeapons.Rifle) 
        { 
            weapon = rifle.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
        }
        else if(weaponName == PlayerWeapons.Shotgun)
        {
            weapon = shotgun.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
        }
        else if (weaponName == PlayerWeapons.Laser) 
        {
            weapon = laser.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
            weapon.GetComponent<LaserWeapon>().fireRate = (2 - fireRate) / 2;
        }
        else if (weaponName == PlayerWeapons.Rocket) 
        {
            weapon = rocket.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
        }
        else if (weaponName == PlayerWeapons.JuggernautWeapon) 
        {
            weapon = juggernautWeapon.GetComponent<Weapon>();
            weapon.playerWeapon = this;
            weapon.gameObject.SetActive(true);
            weapon.animator.SetFloat("FireRate", fireRate);
        }
    }
    
 
    // Update is called once per frame
    void Update()
    {
        if (weapon)
        {
            ShiftAimTransform();
            if (!weapon.isLaser && Input.GetKey(KeyCode.Mouse0))
            {
                weapon.Fire();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                weapon.Reload();
            }
        }
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
            weapon.GetComponent<LaserWeapon>().fireRate = (2 - fireRate) /2;
        }
    }

}
