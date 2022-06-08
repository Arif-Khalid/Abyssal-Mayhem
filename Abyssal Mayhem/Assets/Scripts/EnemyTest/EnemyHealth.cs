using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    //Health count variables
    private int maxHealth = 100;
    private int currentHealth;

    //UI variables
    [SerializeField] Slider slider; //Slider controlling UI healthbar
    [SerializeField] Gradient gradient; //color gradient of healthbar
    [SerializeField] Canvas canvas; //canvas housing healthbar in worldspace
    [SerializeField] Image fill; //image of frontfill of healthbar

    //Reference variables
    public Transform cameraTransform; //Reference to player camera
    PlayerSetup playerSetup; //Reference to player setup script
    public EnemySpawner enemySpawner; //Reference to enemy spawner

    // //Variables used to shoot laser through enemies
    // LineRenderer laserRay;
    // public float rayRange;
    // public float laserDuration = 0.05f;
    // public int laserDamage = 10; //Damage of the laser  

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(100);
        playerSetup = GetComponent<EnemyAI>().player.GetComponent<PlayerSetup>();
    }

    private void LateUpdate()
    {
        //Make health bar canvas face player camera
        if (cameraTransform)
        {
            canvas.transform.LookAt(canvas.transform.position + cameraTransform.forward); 
        }
    }
    public void SetMaxHealth(int value) //Set max health to a new value and restore current health
    {
        maxHealth = value;
        currentHealth = maxHealth;
        slider.maxValue = value;
        slider.value = value;
        fill.color = gradient.Evaluate(1f);
    }
    public void TakeDamage(int damage) //Take Damage
    {
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(currentHealth <= 0)
        {
            DeathByPlayer();
        }
    }

    public void DeathByPlayer() //called when an enemy dies by a player
    {
        playerSetup.killedAnEnemy(transform.position);
        Death();
    }

    public void Death() //called when an enemy dies
    {
        enemySpawner.RemoveFromList(this.gameObject);
        Destroy(this.gameObject);
    }

    // public void FireLaser(Ray incomingRay)
    // {
    //     Debug.Log("Firing Railgun");
    //     laserRay.SetPosition(0, incomingRay.origin);
    //     Debug.Log("Can start the ray");
    //     RaycastHit hitThrough;
    //     if(Physics.Raycast(incomingRay, out hitThrough, rayRange))
    //     {
    //         if (hitThrough.transform.gameObject.GetComponent<EnemyHealth>())
    //         {
    //         Debug.Log("Enemy has EnemyHealth");       
    //         laserRay.SetPosition(1, hitThrough.point);
    //         DamageEnemy(hitThrough.collider);
    //         Ray penetratingFurtherRay = new Ray(hitThrough.point, hitThrough.normal);
    //         hitThrough.collider.GetComponent<EnemyHealth>().FireLaser(penetratingFurtherRay);
    //         Debug.Log("Railgun Fired 2/3/4");       
    //         StartCoroutine(ShootLaser());
    //         }
    //         else
    //         {
    //         var pos = incomingRay.GetPoint (rayRange);
    //         laserRay.SetPosition (1, pos);
    //         StartCoroutine(ShootLaser());
    //         }
    //     }
    //     else
    //     {
    //         var pos = incomingRay.GetPoint (rayRange);
    //         laserRay.SetPosition (1, pos);
    //         StartCoroutine(ShootLaser());
    //     }
    // }

    // public virtual void DamageEnemy(Collider other)
    // {
    //     EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>(); //Check for health Script
    //     if (enemyHealth)
    //     {
    //         enemyHealth.TakeDamage(laserDamage);
    //     }
    //     //Destroy(other.gameObject);
    // }
    
    // IEnumerator ShootLaser()
    // {
    //     laserRay.enabled = true;
    //     yield return new WaitForSeconds(laserDuration);
    //     laserRay.enabled = false;
    // }
}
