using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[RequireComponent(typeof(LineRenderer))]
public class LaserWeapon : Weapon
{
    public Transform laserOrigin; 
    public float gunRange = 50f; 
    public float fireRate = 0.2f; 
    public float laserDuration = 0.05f;
    public int laserDamage = 50;
    [SerializeField] private LayerMask walls; 
    [SerializeField] private LayerMask enemies; 


    //Variables for ADS
    private Vector3 originalPosition;
    public Vector3 aimPosition;
    public float aimDownSightSpeed;
    public bool isAiming = false;


    LineRenderer laserLine;
    float fireTimer;
 
    protected override void ChildStart()
    {
        //in child start
        originalPosition = transform.localPosition;
        laserLine = GetComponent<LineRenderer>();
    }
 
    protected override void ChildUpdate()
    {
        if (fireTimer < fireRate) { fireTimer += Time.deltaTime; }
        if(Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            if (!allowShooting || reloading)
            {
                HipFire();
                return;
            }
            AimDownSight();
            if (!isAiming || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
            {
                return;
            }
            if (fireTimer >= fireRate && Input.GetKey(KeyCode.Mouse0) && !isFiring)
            {
                if (currentAmmo <= 0) //if no ammo to shoot
                {
                    if (maxAmmo == 0) //if no ammo at all in gun reserves
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
                fireTimer = 0;
                laserLine.SetPosition(0, laserOrigin.position);
                Vector3 rayOrigin = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
                Ray ray = new(rayOrigin, Camera.main.transform.forward);
                RaycastHit blockHit;
                Vector3 laserEndPos = ray.GetPoint(gunRange);
                float newRange;
                if (Physics.Raycast(ray, out blockHit, gunRange, walls))
                {
                    laserEndPos = blockHit.point;
                    newRange = blockHit.distance;
                    Debug.Log("Railgun hitted wall");
                }
                else
                {
                    newRange = gunRange;
                }
                RaycastHit[] hitInfos = Physics.RaycastAll(ray, newRange, enemies);
                laserLine.SetPosition(1, laserEndPos);
                StartCoroutine(ShootLaser());
                if (hitInfos != null)
                {
                    List<EnemyHealth> enemyHealths = new List<EnemyHealth>();
                    foreach (RaycastHit hitInfo in hitInfos)
                    {
                        EnemyHealth enemyHealth = hitInfo.transform.root.GetComponent<EnemyHealth>();
                        if (!enemyHealths.Contains(enemyHealth))
                        {
                            enemyHealths.Add(enemyHealth);
                        }
                    }
                    foreach (EnemyHealth enemyHealth in enemyHealths)
                    {
                        DealDamage(enemyHealth);
                    }
                }
            }
        }
        else
        {
            HipFire();
        }
        if (laserLine.enabled)
        {
            laserLine.SetPosition(0, laserOrigin.position);
        }
    }
    
    public virtual void DealDamage(EnemyHealth enemyHealth)
    {
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
        /*transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aimDownSightSpeed);
        if(transform.localPosition == aimPosition)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }*/
        animator.SetFloat("ADS Speed", 2f);
    }

    private void HipFire()
    {
        /* transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimDownSightSpeed);
         isAiming = false;*/
        animator.SetFloat("ADS Speed", -3f);
    }

    //Plays animation for close to wall and its reverse
    protected override void CloseToWall()
    {
        animator.enabled = true;
        animator.SetFloat("ADS Speed", -3f);
        animator.SetFloat("Speed", 1.5f);
        base.CloseToWall();
    }

    protected override void NotCloseToWall()
    {
        animator.SetFloat("Speed", -10f);
        base.NotCloseToWall();
    }

    protected override void OutOfAmmo()
    {
        playerWeapon.Equip(playerWeapon.defaultWeapon);
    }


    public void Aimed()
    {
        Debug.Log("Aimed");
        isAiming = true;
    }

    public void NotAimed()
    {
        Debug.Log("Not Aimed");
        isAiming = false;
    }

    public void BackToEmpty()
    {
        animator.SetBool("GoToEmpty", true);
    }

    public void InEmpty()
    {
        animator.SetBool("GoToEmpty", false);
    }
}