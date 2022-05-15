using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; //Speed of bullet
    public float snapDistance = 2f; //Distance till bullet snaps to target
    public int bulletDamage = 10;
    [SerializeField] LayerMask layerMask;
    Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody) 
        {
            rigidBody.velocity = transform.forward * speed; //Send bullet forward
        }
        
    }

    private void Update()
    {
        RaycastHit hit;
        //Check a short distance ahead of bullet to check for collider
        if(Physics.Raycast(transform.position, transform.forward,out hit, snapDistance, layerMask))
        {
            transform.position = hit.point;
            
        }
    }
    private void OnTriggerEnter(Collider other) //When colliding with something
    {
        HitSomething(other);
    }

    private void HitSomething(Collider other)
    {
        Debug.Log("Hit something");
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>(); //Check for health Script
        if (enemyHealth)
        {
            enemyHealth.TakeDamage(bulletDamage);
        }
        //Destroy bullet
        Destroy(this.gameObject);
    }
}
