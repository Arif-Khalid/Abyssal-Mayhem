using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Variables for bullet movement
    public float speed = 20f; //Speed of bullet
    Rigidbody rigidBody;

    //Variables for collision and damage
    public float snapDistance = 0.5f; //Distance till bullet snaps to target
    public int bulletDamage = 10; //Damage of the bullet   
    [SerializeField] LayerMask bulletCollisionLayerMask; //Things the bullets collide with

    //Variables for destroying stray bullets
    private float timeAlive = 0f; //Variable to track time the bullet stays alive
    [SerializeField] float lifeTime = 2f; //Time of existence before bullet expires

    void Start()
    {
        MoveBullet();        
    }

    private void MoveBullet() //Move the bullet, called in start function
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody)
        {
            rigidBody.velocity = transform.forward * speed; //Send bullet forward
        }
    }

    private void Update()
    {
        CheckPath();
        LimitExistence();
    }

    //Checks the path using raycasts a short distance in front of bullet and 
    //teleports bullet to collision if raycast hit
    private void CheckPath()
    {
        RaycastHit hit;
        //Check a short distance ahead of bullet to check for collider
        if (Physics.Raycast(transform.position, transform.forward, out hit, snapDistance, bulletCollisionLayerMask))
        {
            transform.position = hit.point;

        }
    }

    //Limit the time alive of the bullet and destroys the bullet if
    //alive longer than time alive
    //Called in Update function
    private void LimitExistence()
    {
        timeAlive += Time.deltaTime;
        if (timeAlive >= lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    //Code for hitting something
    private void OnTriggerEnter(Collider other) 
    {
        HitSomething(other);
    }

    //Called when collider triggered
    public virtual void HitSomething(Collider other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>(); //Check for health Script
        if (enemyHealth)
        {
            enemyHealth.TakeDamage(bulletDamage);
        }
        Destroy(this.gameObject);
    }
}
