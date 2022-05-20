using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public Weapon weapon; //stores current active weapon
    [SerializeField] Transform weaponSlot; //Where weapons are equipped to
    [SerializeField] Transform cameraTransform; //Transform of the camera
    [SerializeField] float distance = 50f;
    [SerializeField] LayerMask layerMask;
    public Transform aimTransform; //Where bullets should head
    Transform bulletPoint; //Where bullets shoot from


    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInChildren<Weapon>();
        bulletPoint = weapon.bulletPoint;
    }

    public void Equip(GameObject newWeapon) //Equip a new weapon
    {
        if (weapon != null)
        {
            Destroy(weapon.gameObject); //remove current weapon
        }
        weapon = Instantiate<GameObject>(newWeapon, weaponSlot).GetComponent<Weapon>(); //create new weapon
        bulletPoint = weapon.bulletPoint; 
    }
    
 
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        //Find where player is aiming
        if(Physics.Raycast(cameraTransform.position, cameraTransform.forward , out hit, distance, layerMask)){
            aimTransform.position = hit.point;
        }
        else
        {
            //Default the aimTransform as right in front of the gun
            aimTransform.position = bulletPoint.position + bulletPoint.forward; 
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weapon.Fire();
        }
    }

}
