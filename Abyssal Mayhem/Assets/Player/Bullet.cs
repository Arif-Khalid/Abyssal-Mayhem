using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f; //Speed of bullet
    Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (rigidBody) 
        {
            rigidBody.velocity = transform.forward * speed;
        }
        
    }

    private void OnTriggerEnter(Collider other) //When colliding with something
    {
        Debug.Log("Hit something");
        Destroy(this.gameObject);

    }
}
