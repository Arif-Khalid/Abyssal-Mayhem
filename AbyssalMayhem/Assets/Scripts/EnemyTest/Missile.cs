using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public JuggernautMissile juggernautMissile; //Reference to remove from list
    public JuggernautAI juggernautAI; //Reference to AI for player position
    public int speed;
    public float scalingFactor;
    public float upTime;
    public float snapDistance;
    public int missileDamage;
    public float missileShakeDuration;
    public float missileShakeMagnitude;
    [SerializeField] float lifeTime = 10f;
    private float timeAlive = 0;
    public LayerMask bulletCollisionLayerMask;
    public LayerMask whatIsPlayer;
    [SerializeField]Rigidbody rigidBody;
    GameObject explosionInstance;
    public GameObject explosion;
    public float explosionForce;
    public float explosionRange;
    private void Start()
    {
        StartCoroutine(MissileTravel());
    }

    private void Update()
    {
        CheckPath();
        if (timeAlive > lifeTime)
        {
            EndOfExistence();
        }
        else
        {
            timeAlive += Time.deltaTime;
        }
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
    IEnumerator MissileTravel()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.up);
        float timeTravelledUp = 0;
        while (timeTravelledUp < upTime)
        {
            rigidBody.MovePosition(transform.position + transform.forward * speed);
            timeTravelledUp += Time.deltaTime;
            yield return null;
        }
        while (true)
        {
            transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.LookRotation(juggernautAI.player.position - transform.position), Time.deltaTime / scalingFactor);
            rigidBody.MovePosition(transform.position + transform.forward * speed);
            yield return null;
        }
    }

    private void EndOfExistence()
    {
        Explode();
    }

    private void OnTriggerEnter(Collider other)
    {
        Explode();
    }
    private void Explode()
    {
        //Instantiate explosion
        if (explosion != null)
        {
            explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);
        }

        //Check for player
        Collider[] players = Physics.OverlapSphere(transform.position, explosionRange, whatIsPlayer);
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                PlayerMovement playerMovement = players[i].GetComponent<PlayerMovement>();
                PlayerHealth playerHealth = players[i].GetComponent<PlayerHealth>();
                Vector3 dir = players[i].transform.position - transform.position;
                playerMovement.AddImpact(dir, explosionForce * 10);
                playerHealth.TakeDamage(missileDamage, missileShakeDuration, missileShakeMagnitude);
                //CameraShake.cameraShake.StartCoroutine(CameraShake.cameraShake.Shake(missileShakeDuration, missileShakeMagnitude));
            }
        }

        Invoke(nameof(Delay), 0.05f);
    }

    private void Delay()
    {
        if (juggernautMissile)
        {
            juggernautMissile.RemoveFromList(this);
        }
        Destroy(this.gameObject);
    }
}
