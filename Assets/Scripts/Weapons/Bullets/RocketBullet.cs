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
    public LayerMask whatIsPlayer;

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
    public float rocketShakeDuration = 0.15f;
    public float rocketShakeMagnitude = 0.4f;

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

    public override void EndOfExistence()
    {
        if (!hasBulletCollided) { Explode(); }
    }
    private void Explode()
    {
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
            Rigidbody rigidBody = enemies[i].GetComponent<Rigidbody>();
            if (rigidBody && rigidBody.isKinematic == false) //Add force directly for non kinematic rigidbodies
            {
                rigidBody.AddExplosionForce(explosionForce, transform.position, explosionRange, 0.01f, ForceMode.Impulse);
            }
        }
        foreach(EnemyHealth enemyHealth in enemyHealths)
        {
            enemyHealth.TakeDamage(explosionDamage);
            if (!enemyHealth.isDead)
            {
                enemyHealth.GetComponent<EnemyAI>().BounceBackUndo();
                enemyHealth.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, 0.01f, ForceMode.Impulse);
            }           
        }
        //Check for player
        Collider[] players = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
        if(players!= null)
        {
            for(int i = 0; i < players.Length; i++)
            {
                PlayerMovement playerMovement = players[i].GetComponent<PlayerMovement>();
                Vector3 dir = players[i].transform.position - transform.position;
                playerMovement.AddImpact(dir, explosionForce * 10);
                CameraShake.cameraShake.StartCoroutine(CameraShake.cameraShake.Shake(rocketShakeDuration, rocketShakeMagnitude));
            }
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
        gameObject.SetActive(false);
    }
    public override void DealDamage(Collider other)
    {
        damageDealt = true;
        Explode();
    }
}
