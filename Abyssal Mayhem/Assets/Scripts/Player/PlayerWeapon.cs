using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon; //stores current active weapon
    [SerializeField] public Transform weaponSlot; //Where weapons are equipped to
    
    //Aim variables
    [SerializeField] Transform cameraTransform; //Transform of the camera
    [SerializeField] float distance = 50f; //Distance of raycast to move aim transform
    [SerializeField] LayerMask layerMask; //What aim transform will move to when faced
    public Transform aimTransform; //Where bullets should head
    Transform bulletPoint; //Where bullets shoot from


    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        bulletPoint = weapon.bulletPoint;
    }

    //Equip a new weapon
    public void Equip(GameObject newWeapon) 
    {
        if (weapon != null)
        {
            Destroy(weapon.gameObject); //remove current weapon
        }
        weapon = Instantiate<GameObject>(newWeapon, weaponSlot).GetComponent<Weapon>(); //create new weapon
        weapon.playerWeapon = this;
        bulletPoint = weapon.bulletPoint; 
    }
    
 
    // Update is called once per frame
    void Update()
    {
        if (weapon)
        {
            ShiftAimTransform();
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Debug.Log("Firing weapon");
                weapon.Fire();
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
            aimTransform.position = bulletPoint.position + bulletPoint.forward;
        }
    }

}
