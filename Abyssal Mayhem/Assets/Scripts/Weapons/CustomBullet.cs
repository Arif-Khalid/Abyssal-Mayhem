using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBullet : MonoBehaviour
{
    //Assignables
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;
 
    //Damage
    public int explosionDamage;
    public float explosionRange;
    public float explosionForce;

    //Lifetime
    public int maxCollisions;
    public float maxLifeTime;
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;    

    void Start()
    {
        Setup();
    }
    
    private void Update()
    {
        //When to explode
        if (collisions > maxCollisions)
        Explode();

        //Count down lifetime
        maxLifeTime -= Time.deltaTime;
        if (maxLifeTime <=0)
        Explode();
    }

    private void Explode()
    {
        //Instantiate explosion
        if(explosion != null) 
        Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        for (int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call Take Damage
            enemies[i].GetComponent<EnemyHealth>().TakeDamage(explosionDamage);

            //Add explosion Force (if enemy has a rigidbody)
            if(enemies[i].GetComponent<Rigidbody>())
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange);
        }

        //Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }

    private void Delay()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Don't Count collision with other bullets
        
        //Count up collisions
        collisions++;

        //Explode if bullet hits an enemy directly and explodeOnTouch is activated
        if(collision.collider.CompareTag("Enemy") && explodeOnTouch)
        Explode();
    }

    private void Setup()
    {
        //Create a new Physic Material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<SphereCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
