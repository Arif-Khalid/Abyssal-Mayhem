using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemySpawner : MonoBehaviour
{
    //Variables for spawning monsters
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject monster;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float timeBetweenRounds;
    private bool alreadySpawned;
    
    //Variables for local player
    public Transform localPlayer;
    public bool localPlayerReady = false;
    public Transform cameraTransform;
    private bool metLocalQuota;
    private bool newRoundStarted = false;
    public PlayerUI localUI;

    //Variables for away player
    public PlayerSetup awayPlayer;
    public bool awayPlayerReady = false;
    public PlayerUI awayUI;
    private bool metAwayQuota;

    //Rounds and their quotas array
    [SerializeField] int[] Quotas;
    private int round = 0;

    List<GameObject> spawnedMonsters = new List<GameObject>();

    private bool isSinglePlayer = false;

    //Code for allowing monsters to spawn while waiting for player
    public void AllowSpawns()
    {
        isSinglePlayer = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && !awayPlayerReady)
        {
            AllowSpawns();
            localUI.UpdateWaitingPrompt();
        }
        //Spawns monster at regular intervals if both players ready and quota not met
        if (!alreadySpawned && ((localPlayerReady && awayPlayerReady && !metLocalQuota) || isSinglePlayer))
        {
            SpawnMonster();
            alreadySpawned = true;
            Invoke(nameof(ResetSpawner), timeBetweenSpawn);
        }
    }
    
    //Spawns a mob at a specific vector3 position with the color blue
    public void SpawnMonsterAtPoint(Vector3 position)
    {
        if (!localPlayerReady)
        {
            return;
        }
        GameObject spawnedMonster = Instantiate<GameObject>(monster, position, Quaternion.LookRotation(localPlayer.position - transform.position)); //Spawn monster facing the player
        spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
        spawnedMonster.GetComponent<Renderer>().material.color = Color.blue;
        EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
        enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
        enemyHealth.enemySpawner = this;
        spawnedMonsters.Add(spawnedMonster);
    }

    //Spawns a mob at one of the predefined spawn locations(chosen randomly)
    private void SpawnMonster()
    {
        int spawnID = Random.Range(0, spawnPoints.Length - 1); //Choose a random spawnPoint
        GameObject spawnedMonster = (GameObject)Instantiate<GameObject>(monster, spawnPoints[spawnID].position, Quaternion.LookRotation(localPlayer.position - transform.position)); //Spawn monster facing the player
        spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
        EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
        enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
        enemyHealth.enemySpawner = this;
        spawnedMonsters.Add(spawnedMonster);
    }
        private void ResetSpawner()
    {
        alreadySpawned = false;
    }

    //Ready local player and start round if both players ready
    public void localPlayerReadyUp()
    {
        localPlayerReady = true;
        if (awayPlayerReady)
        {
            isSinglePlayer = false;
            PlayerSetup.localPlayerSetup.SetComponents(true);
            DisablePlayerUI();
            round = 0; //Ensure round is 0
            localPlayer.GetComponent<PlayerHealth>().SetMaxHealth(0);
            StartNextRound();
        }
    }

    //Ready away player and start round if both players ready
    public void awayPlayerReadyUp()
    {
        awayPlayerReady = true;
        if (localPlayerReady)
        {
            isSinglePlayer = false;
            PlayerSetup.localPlayerSetup.SetComponents(true);
            DisablePlayerUI();
            round = 0;
            localPlayer.GetComponent<PlayerHealth>().SetMaxHealth(0);
            StartNextRound();
        }
    }

    //Starts the next round
    void StartNextRound()
    {
        KillAll();
        alreadySpawned = false;
        Debug.Log("next round started");
        round += 1;
        if(round >= Quotas.Length)
        {
            Debug.Log("you finished the damn game");
            return;
        }
        localUI.BothPlayersReady(); //Removes waiting for player text
        localUI.UpdateRoundText(round, Quotas[round]);
        metLocalQuota = false;
        localPlayer.GetComponent<PlayerSetup>().ResetScore(); //Resets scores
        newRoundStarted = false; //allows new round to be started
    }

    /*Code for score updates*/

    //Checks if local score >= Quota and starts new round if quota for both players have been reached
    public void UpdateLocalScore(int newScore)
    {
        if(round >= Quotas.Length)
        {
            return;
        }
        metLocalQuota = (newScore >= Quotas[round]);
        if (metLocalQuota)
        {
            if (metAwayQuota)
            {
                if (!newRoundStarted)
                {
                    newRoundStarted = true;
                    Invoke(nameof(StartNextRound), timeBetweenRounds);
                }
            }
            else
            {
                if(round == Quotas.Length - 1)
                {
                    localPlayer.GetComponent<PlayerSetup>().Survived();
                }                
            }           
        }
    }

    //Checks if away score >= quota and start new round if quota for both players have been reached
    public void UpdateAwayScore(int newScore)
    {
        if(round >= Quotas.Length)
        {
            return;
        }
        metAwayQuota = (newScore >= Quotas[round]);
        if (metAwayQuota)
        {
            if (metLocalQuota)
            {
                if (!newRoundStarted)
                {
                    newRoundStarted = true;
                    Invoke(nameof(StartNextRound), timeBetweenRounds); //delay by a short time to allow both clients to call this function
                }               
            }
            else
            {
                Debug.Log("BOOO you are slower");
                //put some sort of bad job message
            }            
        }
    }


    /*Code that manipulates the list of spawned monsters*/

    //Restarts the game but keeps scores for the local player
    //used for if opponent disconnects
    public void RestartGame()
    {
        round = 0;
        if (localPlayer != null)
        {
            KillAll();
            localUI.BothPlayersNotReady();
            localUI.ResetRounds();
        }
    }

    
    public void RemoveFromList(GameObject spawnedMonster)
    {
        spawnedMonsters.Remove(spawnedMonster);
    }

    //Kills all spawned monsters
    private void KillAll()
    {
        foreach(var spawnedMonster in spawnedMonsters)
        {
            Destroy(spawnedMonster);
        }
        spawnedMonsters.Clear();
    }

    /*Code that handles player's death*/
    //Called when a player dies
    public void LocalDeath()
    {
        PlayerSetup.localPlayerSetup.EnterUIMenu();
        localUI.EnableDeathUI();
        if (!isSinglePlayer) { localPlayerReady = false; } //Only unready local player if local player isnt in single player mode
        isSinglePlayer = false; //Disable single player mode to stop spawning
        awayPlayerReady = false;
    }

    //Called when a player wins
    public void LocalWin()
    {
        PlayerSetup.localPlayerSetup.EnterUIMenu();
        localUI.EnableWinUI();
        localPlayerReady = false;
        awayPlayerReady = false;
        KillAll();
    }

    //Called when other player survives all rounds
    public void LocalLoss()
    {
        PlayerSetup.localPlayerSetup.EnterUIMenu();
        localUI.EnableDeathUI(); //To be replaced with different UI for loss and not death
        localPlayerReady = false;
        awayPlayerReady = false;
        KillAll();
    }

    //Function that hard resets the game for local player
    public void LocalPlayAgain()
    {
        PlayerSetup.localPlayerSetup.ExitUIMenu();
        localUI.DisableWinUI();
        localUI.DisableDeathUI();
        localUI.ResetWaitingPrompt();
        if (awayUI)
        {
            awayUI.UpdateAwayScore(0);
        }
        UpdateAwayScore(0);
        UpdateLocalScore(0);
        RestartGame();
        localPlayerReadyUp();
    }

    private void DisablePlayerUI()
    {
        PlayerSetup.localPlayerSetup.ExitUIMenu();
        localUI.DisableWinUI();
        localUI.DisableDeathUI();
    }
    
}
