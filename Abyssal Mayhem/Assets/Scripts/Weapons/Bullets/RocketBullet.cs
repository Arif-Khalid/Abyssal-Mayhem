using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RocketBullet : Bullet
{
    //Assignables
    public GameObject explosion; 
    public GameObject explosionInstance;
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
    public bool explodeOnTouch = true;

    int collisions;
    PhysicMaterial physics_mat;

    protected override void ChildStart()
    {
        //Create a new Physic Material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<CapsuleCollider>().material = physics_mat;

        //Set gravity
        rigidBody.useGravity = useGravity;
    }

    protected override void EndOfExistence()
    {
        if (!hasBulletCollided) { Explode(); }
    }
    private void Explode()
    {
        counter++;
        Debug.Log(counter);
        //Instantiate explosion
        if(explosion != null) 
        explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);

        //Check for enemies
        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, whatIsEnemies);
        List<EnemyHealth> enemyHealths = new List<EnemyHealth>();
        for(int i = 0; i < enemies.Length; i++)
        {
            EnemyHealth enemyHealth = enemies[i].transform.root.GetComponent<EnemyHealth>();
            if (!enemyHealths.Contains(enemyHealth))
            {
                enemyHealths.Add(enemyHealth);
            }
        }
        foreach(EnemyHealth enemyHealth in enemyHealths)
        {
            enemyHealth.TakeDamage(explosionDamage);
            enemyHealth.GetComponent<EnemyAI>().BounceBackUndo();
            enemyHealth.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 0.01f, ForceMode.Impulse);
        }
        /*for (int i = 0; i < enemies.Length; i++)
        {
            //Get component of enemy and call Take Damage
            if(enemies[i].GetComponent<EnemyHealth>())
            {
                enemies[i].GetComponent<EnemyHealth>().TakeDamage(explosionDamage);
                enemies[i].GetComponent<EnemyAI>().BounceBackUndo();
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 0.01f, ForceMode.Impulse);

            }
                
        }*/

        //Add a little delay, just to make sure everything works fine
        Invoke("Delay", 0.05f);
    }

        
    private void Delay()
    {
        Destroy(gameObject);
    }
    public override void DealDamage(Collider other)
    {
        damageDealt = true;
        Explode();
    }
}
