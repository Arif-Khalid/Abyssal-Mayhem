using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Variables for spawning monsters
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] GameObject walker;
    [SerializeField] GameObject juggernaut;
    [SerializeField] int[] juggernautSpawns;
    [SerializeField] GameObject assassin;
    [SerializeField] int[] assassinSpawns;
    public List<Transform> assassinSpawnPoints = new List<Transform>();
    [SerializeField] Transform[] patrolTransforms;
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

    //Variables for spawning pickups
    public List<ChestContent> weaponChests = new List<ChestContent>();
    List<ChestContent> availableWeaponChests = new List<ChestContent>();
    public float timeBetweenPickups;
    private float pickupTimer;
    public bool pickupsReadyToSpawn;
    public GameObject[] weaponPickups = new GameObject[2];

    private bool isSinglePlayer = false;
    public enum MonsterID { walker, juggernaut, assassin };
    public Dictionary<EnemySpawner.MonsterID, GameObject> Monsters = new Dictionary<EnemySpawner.MonsterID, GameObject>();
    private void Start()
    {
        Monsters.Add(EnemySpawner.MonsterID.walker, walker);
        Monsters.Add(EnemySpawner.MonsterID.juggernaut, juggernaut);
        Monsters.Add(EnemySpawner.MonsterID.assassin, assassin);
        foreach(ChestContent weaponChest in weaponChests)
        {
            availableWeaponChests.Add(weaponChest);
        }
    }
    //Code for allowing monsters to spawn while waiting for player
    public void AllowSpawns()
    {
        isSinglePlayer = true;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && !awayPlayerReady)
        {
            //AllowSpawns();
            //localUI.UpdateWaitingPrompt();
            awayPlayerReadyUp();
        }
        //Spawns monster at regular intervals if both players ready and quota not met
        if (!alreadySpawned && ((localPlayerReady && awayPlayerReady && !metLocalQuota) || isSinglePlayer))
        {
            SpawnMonster(EnemySpawner.MonsterID.walker);
            alreadySpawned = true;
            Invoke(nameof(ResetSpawner), timeBetweenSpawn);
        }
        if (pickupTimer < timeBetweenPickups)
        {
            pickupTimer += Time.deltaTime;           
        }
        else if((localPlayerReady && awayPlayerReady)|| isSinglePlayer)
        {
            pickupTimer = 0;
            //spawn pickups function
            SpawnPickups();
        }
    }

    //Spawns pickups at available weapon chests
    private void SpawnPickups()
    {
        if(availableWeaponChests.Count <= 0)
        {
            return;
        }
        int spawnID = Random.Range(0, availableWeaponChests.Count - 1);
        int pickupID = Random.Range(0, weaponPickups.Length);
        Debug.Log(pickupID);
        if (availableWeaponChests[spawnID].ResetContent(weaponPickups[pickupID]))
        {
            availableWeaponChests.Remove(availableWeaponChests[spawnID]);
        }
        else
        {
            availableWeaponChests.Remove(availableWeaponChests[spawnID]);
            SpawnPickups();
        }
    }

    public void MakeWeaponChestAvailable(ChestContent weaponChest)
    {
        availableWeaponChests.Add(weaponChest);
    }

    //Destroys all pickups available and resets available weapon chests
    public void ResetWeaponSpawns()
    {
        foreach(ChestContent weaponChest in weaponChests)
        {
            weaponChest.HardReset();
            if (!availableWeaponChests.Contains(weaponChest))
            {
                availableWeaponChests.Add(weaponChest);
            }
        }
        pickupTimer = 0;
    }
    //Spawns a mob at a specific vector3 position with the color blue
    public void SpawnMonsterAtPoint(Vector3 position, EnemySpawner.MonsterID monsterID)
    {
        if (!localPlayerReady)
        {
            return;
        }
        if (monsterID == EnemySpawner.MonsterID.assassin) //spawning assassin
        {
            if (assassinSpawnPoints.Count.Equals(0))
            {
                return;
            }
            int spawnID = Random.Range(0, assassinSpawnPoints.Count - 1); //Choose a random spawnPoint
            GameObject spawnedMonster = (GameObject)Instantiate<GameObject>(Monsters[monsterID], assassinSpawnPoints[spawnID].position, assassinSpawnPoints[spawnID].rotation); //Spawn assassin facing predefined rotation
            spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
            EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
            enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
            enemyHealth.enemySpawner = this;
            enemyHealth.startingTransform = assassinSpawnPoints[spawnID];
            assassinSpawnPoints.Remove(assassinSpawnPoints[spawnID]);
            spawnedMonsters.Add(spawnedMonster);
            //Do different stuff for assassin
            WeaponIK weaponIK = spawnedMonster.GetComponent<WeaponIK>();
            weaponIK.patrolTransforms = patrolTransforms;
        }
        else //spawning something else
        {
            GameObject spawnedMonster = Instantiate<GameObject>(Monsters[monsterID], position, Quaternion.LookRotation(localPlayer.position - transform.position)); //Spawn monster facing the player
            spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
            EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
            enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
            enemyHealth.enemySpawner = this;
            spawnedMonsters.Add(spawnedMonster);
        }       
    }

    //Spawns a mob at one of the predefined spawn locations(chosen randomly)
    private void SpawnMonster(EnemySpawner.MonsterID monsterID)
    {
        if (monsterID == EnemySpawner.MonsterID.assassin) //spawning assassin
        {
            if (assassinSpawnPoints.Count.Equals(0)) //no spawn points available
            {
                return;
            }
            int spawnID = Random.Range(0, assassinSpawnPoints.Count - 1); //Choose a random spawnPoint
            GameObject spawnedMonster = (GameObject)Instantiate<GameObject>(Monsters[monsterID], assassinSpawnPoints[spawnID].position, assassinSpawnPoints[spawnID].rotation); //Spawn assassin facing predefined rotation
            spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
            EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
            enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
            enemyHealth.enemySpawner = this;
            enemyHealth.startingTransform = assassinSpawnPoints[spawnID];
            enemyHealth.GetComponent<AssassinAI>().startingTransform = assassinSpawnPoints[spawnID];
            assassinSpawnPoints.Remove(assassinSpawnPoints[spawnID]);
            spawnedMonsters.Add(spawnedMonster);
            //Do different stuff for assassin
            WeaponIK weaponIK = spawnedMonster.GetComponent<WeaponIK>();
            weaponIK.patrolTransforms = patrolTransforms;
        }
        else //spawning anything else
        {
            int spawnID = Random.Range(0, spawnPoints.Length - 1); //Choose a random spawnPoint
            GameObject spawnedMonster = (GameObject)Instantiate<GameObject>(Monsters[monsterID], spawnPoints[spawnID].position, Quaternion.LookRotation(localPlayer.position - transform.position)); //Spawn monster facing the player
            spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
            EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
            enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
            enemyHealth.enemySpawner = this;
            spawnedMonsters.Add(spawnedMonster);
        }
        
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
            ResetWeaponSpawns();
            ResetPlayerPosition();
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
            ResetWeaponSpawns();
            ResetPlayerPosition();
            StartNextRound();
        }
    }

    //Starts the next round
    void StartNextRound()
    {
        KillAll();
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
        SpawnSpecial();
        alreadySpawned = false;
    }

    //Spawns all special monsters for that round
    void SpawnSpecial()
    {
        //spawn juggernauts
        for(int i = 0; i < juggernautSpawns[round]; i++) { SpawnMonster(EnemySpawner.MonsterID.juggernaut); }
        //spawn assassins
        //yet to create special spawnpoints for assassins on towers
        for(int i = 0; i < assassinSpawns[round]; i++) { SpawnMonster(EnemySpawner.MonsterID.assassin); }
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
            ResetWeaponSpawns();
            KillAll();
            localUI.BothPlayersNotReady();
            localUI.ResetRounds();
            ResetPlayerPosition();
        }
    }

    public void AddtoAssassinSpawnPoints(Transform assassinSpawnPoint)
    {
        assassinSpawnPoints.Add(assassinSpawnPoint);
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
    
    //Resets the player to starting position
    private void ResetPlayerPosition()
    {
        PlayerSetup.localPlayerSetup.transform.position = PlayerSpawn.playerSpawn.position;
        PlayerSetup.localPlayerSetup.transform.rotation = PlayerSpawn.playerSpawn.rotation;
        PlayerSetup.localPlayerSetup.ResetWeapons();
    }
}
