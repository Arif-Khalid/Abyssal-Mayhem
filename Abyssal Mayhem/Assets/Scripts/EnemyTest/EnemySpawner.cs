using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject monster;
    [SerializeField] float timeBetweenSpawn;
    public Transform player;
    public Transform cameraTransform;
    private bool alreadySpawned;

    private void Start()
    {
        //player = FindObjectOfType<PlayerMovement>().transform;
        //cameraTransform = player.GetComponentInChildren<Camera>().transform;
    }
    // Update is called once per frame
    void Update()
    {
        if (!alreadySpawned && player && cameraTransform)
        {
            SpawnMonster();
            alreadySpawned = true;
            Invoke(nameof(ResetSpawner), timeBetweenSpawn);
        }
    }

    private void SpawnMonster()
    {
        int spawnID = Random.Range(0, spawnPoints.Length - 1); //Choose a random spawnPoint
        GameObject spawnedMonster = Instantiate<GameObject>(monster, spawnPoints[spawnID].position, Quaternion.LookRotation(player.position - transform.position)); //Spawn monster facing the player
        spawnedMonster.GetComponent<EnemyAI>().player = player; //Set target for monster
        spawnedMonster.GetComponent<EnemyHealth>().cameraTransform = cameraTransform; //Set camera for healthbar to face
    }
    private void ResetSpawner()
    {
        alreadySpawned = false;
    }
}
