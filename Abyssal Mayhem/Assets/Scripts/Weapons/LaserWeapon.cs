using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(LineRenderer))]
public class LaserWeapon : MonoBehaviour
{
    public Camera playerCamera;
    public PlayerWeapon playerWeapon; //Reference to player weapon script
    public Transform laserOrigin;
    public float gunRange = 50f;
    public float fireRate = 0.2f;
    public float laserDuration = 0.05f;
    public int laserDamage = 10; //Damage of the laser
    private bool allowShooting = true;
    private bool reloading = false;
    [SerializeField] public int maxAmmo = -1; //Set to -1 when infinite ammo
    [SerializeField] int clipSize = 10; //Size of weapon clip
    int currentAmmo; //current ammo in weapon
    public Animator animator;
    PlayerUI playerUI;
    [SerializeField] private LayerMask walls;
    [SerializeField] private LayerMask enemies;


    //Variables for ADS
    private Vector3 originalPosition;
    public Vector3 aimPosition;
    public float aimDownSightSpeed;
    public bool isAiming = false;


    LineRenderer laserLine;
    float fireTimer;
 
    void Start()
    {
        playerWeapon = GetComponentInParent<PlayerWeapon>();
        playerUI = GetComponentInParent<PlayerUI>();
        animator = GetComponent<Animator>();
        currentAmmo = clipSize;
        playerUI.UpdateAmmoText(currentAmmo, maxAmmo);

        originalPosition = transform.localPosition;
        laserLine = GetComponent<LineRenderer>();        
    }
 
    void Update()
    {
        fireTimer += Time.deltaTime;
        gunRange = 50f;
        if(Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            AimDownSight();
            if (!allowShooting || reloading || !isAiming)
            {
                return;
            }
            else
            {
                if(fireTimer >= fireRate && Input.GetKey(KeyCode.Mouse0))
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
                    fireTimer = 0;
                    laserLine.SetPosition(0, laserOrigin.position);
                    Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                    var mousePos = Input.mousePosition;
                    var rayMouse = playerCamera.ScreenPointToRay(mousePos);
                    Ray ray = new Ray(rayOrigin, playerCamera.transform.forward);
                    RaycastHit blockHit;
                    if (Physics.Raycast(ray, out blockHit, gunRange, LayerMask.GetMask("walls")))
                    {
                        gunRange = blockHit.distance;
                        Debug.Log("Railgun hitted wall");        
                    }
                    RaycastHit[] hitInfos = Physics.RaycastAll(ray, gunRange, LayerMask.GetMask("enemies"));
                    if (hitInfos != null)
                    {
                        laserLine.SetPosition(1, blockHit.point);                        
                        foreach (RaycastHit hitInfo in hitInfos)
                        {
                            if (hitInfo.collider.GetComponent<EnemyHealth>())
                            {
                                HitSomething(hitInfo.collider);
                                Debug.Log("Railgun Hitted " + hitInfo.collider.name);        
                                StartCoroutine(ShootLaser());    
                            }
                            // else 
                            // {
                            //     var pos = rayMouse.GetPoint (gunRange);
                            //     laserLine.SetPosition (1, pos);
                            //     StartCoroutine(ShootLaser());
                            // }
                        }
                    }
                    else
                    {
                        var pos = rayMouse.GetPoint (gunRange);
                        laserLine.SetPosition (1, pos);
                        StartCoroutine(ShootLaser());
                    }

                    // RaycastHit hit;
                    // if(Physics.Raycast(ray, out hit, gunRange))
                    // {
                    //     if (hit.transform.gameObject.GetComponent<EnemyHealth>())
                    //     {
                    //     laserLine.SetPosition(1, hit.point);
                    //     HitSomething(hit.collider);
                    //     Ray penetratingRay = new Ray(hit.point, hit.normal);
                    //     hit.collider.GetComponent<EnemyHealth>().FireLaser(penetratingRay);
                    //     Debug.Log("Railgun Fired 1");        
                    //     StartCoroutine(ShootLaser());
                    //     }
                    //     else
                    //     {
                    //     var pos = rayMouse.GetPoint (gunRange);
                    //     laserLine.SetPosition (1, pos);
                    //     StartCoroutine(ShootLaser());
                    //     }
                    // }
                    // else
                    // {
                    //     var pos = rayMouse.GetPoint (gunRange);
                    //     laserLine.SetPosition (1, pos);
                    //     StartCoroutine(ShootLaser());
                    // }
                }
            }
        }
        else
        {
            HipFire();
        }
    }
    
    protected virtual void OutOfAmmo()
    {
        //To be overriden in inherited class
        //Function to call when no more ammo in clip or reserves and trying to shoot
    }

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

    public virtual void HitSomething(Collider other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>(); //Check for health Script
        if (enemyHealth)
        {
            enemyHealth.TakeDamage(laserDamage);
        }
        //Destroy(other.gameObject);
    }
 
    IEnumerator ShootLaser()
    {
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    private void AimDownSight()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aimDownSightSpeed);
        if(transform.localPosition == aimPosition)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
    }

    private void HipFire()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimDownSightSpeed);
        isAiming = false;
    }
}