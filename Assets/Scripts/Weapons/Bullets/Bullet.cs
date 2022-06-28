using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Variables for bullet movement
    public float speed = 20f; //Speed of bullet
    protected Rigidbody rigidBody;

    //Variables for collision and damage
    public float snapDistance = 0.5f; //Distance till bullet snaps to target
    public int bulletDamage = 10; //Damage of the bullet   
    [SerializeField] LayerMask bulletCollisionLayerMask; //Things the bullets collide with

    //Variables for destroying stray bullets
    private float timeAlive = 0f; //Variable to track time the bullet stays alive
    [SerializeField] float lifeTime = 2f; //Time of existence before bullet expires
    private Collider bulletCollider;
    private Collider otherCollider;
    public bool hasBulletCollided = false;
    protected bool damageDealt = false;
    public int counter = 0; //remove this
    public EnemyAI enemyAI; //enemy that spawns bullets
    public Vector3 shooterPosition; //Stores position for damage indicator should enemy die before damage indicator triggered

    void Start()
    {
        bulletCollider = GetComponent<Collider>();
        MoveBullet();
        ChildStart();
    }

    //Function to be overriden by children for additional start implementation
    protected virtual void ChildStart()
    {
        //To be overriden
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
        if (hasBulletCollided && !damageDealt) { DealDamage(otherCollider); }
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
            EndOfExistence();
        }
    }

    public virtual void EndOfExistence()
    {
        if (!hasBulletCollided) { Destroy(this.gameObject); }
    }
    //Code for hitting something
    private void OnTriggerEnter(Collider other) 
    {
        if (other.GetComponent<Interactable>())
        {
            return;
        }
        HitSomething(other);
    }

    //Called when collider triggered
    public virtual void HitSomething(Collider other)
    {
        bulletCollider.enabled = false;
        hasBulletCollided = true;
        otherCollider = other;
    }

    //Called the frame after collider triggered
    public virtual void DealDamage(Collider other)
    {
        damageDealt = true;
        EnemyHealth enemyHealth = otherCollider.transform.root.GetComponent<EnemyHealth>(); //Check for health Script
        if (enemyHealth && !enemyHealth.isDead)
        {
            enemyHealth.TakeDamage(bulletDamage);
        }
        Destroy(this.gameObject);
    }
}
