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
    [SerializeField] GameObject juggernautBoss;
    [SerializeField] int[] juggernautBossSpawns;
    [SerializeField] GameObject assassin;    
    [SerializeField] int[] assassinSpawns;
    [SerializeField] GameObject assassinBoss;
    [SerializeField] int[] assassinBossSpawns;
    public List<Transform> assassinSpawnPoints = new List<Transform>();
    [SerializeField] Transform[] patrolTransforms;
    [SerializeField] float timeBetweenSpawn;
    [SerializeField] float timeBetweenRounds;
    private bool alreadySpawned;
    public int maxRounds = 5;
    
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
    public GameObject[] powerupPickups = new GameObject[4];

    //Invincibility powerups variables
    bool isInvincible = false;
    private bool isSinglePlayer = false;
    public enum MonsterID { walker, juggernaut, assassin, juggernautBoss, assassinBoss };
    public Dictionary<EnemySpawner.MonsterID, GameObject> Monsters = new Dictionary<EnemySpawner.MonsterID, GameObject>();
    private void Start()
    {
        Monsters.Add(EnemySpawner.MonsterID.walker, walker);
        Monsters.Add(EnemySpawner.MonsterID.juggernaut, juggernaut);
        Monsters.Add(EnemySpawner.MonsterID.assassin, assassin);
        Monsters.Add(EnemySpawner.MonsterID.juggernautBoss, juggernautBoss);
        Monsters.Add(EnemySpawner.MonsterID.assassinBoss, assassinBoss);
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
            awayPlayerReadyUp();
            //AllowSpawns();
            localUI.UpdateWaitingPrompt();
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
            SpawnPickups(weaponPickups);
            SpawnPickups(powerupPickups);
        }
    }

    //Spawns pickups at available weapon chests
    private void SpawnPickups(GameObject[] pickups)
    {
        if(availableWeaponChests.Count <= 0)
        {
            return;
        }
        int spawnID = Random.Range(0, availableWeaponChests.Count - 1);
        int pickupID = Random.Range(0, pickups.Length);
        Debug.Log(pickups.Length);
        if (availableWeaponChests[spawnID].ResetContent(pickups[pickupID]))
        {
            availableWeaponChests.Remove(availableWeaponChests[spawnID]);
        }
        else
        {
            availableWeaponChests.Remove(availableWeaponChests[spawnID]);
            SpawnPickups(pickups);
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
        if (position == Vector3.zero) //spawns monster randomly if no position given
        {
            SpawnMonster(monsterID);
            return;
        }
        if (monsterID == EnemySpawner.MonsterID.assassin || monsterID == EnemySpawner.MonsterID.assassinBoss) //spawning assassin
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
            enemyHealth.GetComponent<AssassinAI>().startingTransform = assassinSpawnPoints[spawnID];
            assassinSpawnPoints.Remove(assassinSpawnPoints[spawnID]);
            spawnedMonsters.Add(spawnedMonster);
            //Do different stuff for assassin
            WeaponIK weaponIK = spawnedMonster.GetComponent<WeaponIK>();
            weaponIK.patrolTransforms = patrolTransforms;
            if (isInvincible)
            {
                spawnedMonster.GetComponent<Outline>().enabled = true;
            }
        }
        else //spawning something else
        {
            GameObject spawnedMonster = Instantiate<GameObject>(Monsters[monsterID], position, Quaternion.LookRotation(localPlayer.position - transform.position)); //Spawn monster facing the player
            spawnedMonster.GetComponent<EnemyAI>().player = localPlayer; //Set target for monster
            EnemyHealth enemyHealth = spawnedMonster.GetComponent<EnemyHealth>();
            enemyHealth.cameraTransform = cameraTransform; //Set camera for healthbar to face
            enemyHealth.enemySpawner = this;
            spawnedMonsters.Add(spawnedMonster);
            if (isInvincible)
            {
                spawnedMonster.GetComponent<Outline>().enabled = true;
            }
        }       
    }

    //Spawns a mob at one of the predefined spawn locations(chosen randomly)
    private void SpawnMonster(EnemySpawner.MonsterID monsterID)
    {
        if (monsterID == EnemySpawner.MonsterID.assassin || monsterID == EnemySpawner.MonsterID.assassinBoss) //spawning assassin
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
            if (isInvincible)
            {
                spawnedMonster.GetComponent<Outline>().enabled = true;
            }
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
            if (isInvincible)
            {
                spawnedMonster.GetComponent<Outline>().enabled = true;
            }
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
            localUI.BothPlayersReady(); //Removes waiting for player text
            KillAll();
            ResetAppliedPowerups();
            localUI.StartNewRoundCount();
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
            localUI.BothPlayersReady(); //Removes waiting for player text
            KillAll();
            ResetAppliedPowerups();
            localUI.StartNewRoundCount();
        }
    }

    //Starts the next round
    public void StartNextRound()
    {
        if(!localPlayerReady || !awayPlayerReady)
        {
            return;
        }
        KillAll();
        Debug.Log("next round started");
        round += 1;
        if(round >= Quotas.Length)
        {
            Debug.Log("you finished the damn game");
            return;
        }
        
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
        //spawn juggernautBoss
        for(int i = 0; i < juggernautBossSpawns[round]; i++) { SpawnMonster(EnemySpawner.MonsterID.juggernautBoss); }
        //spawn assassins
        for(int i = 0; i < assassinSpawns[round]; i++) { SpawnMonster(EnemySpawner.MonsterID.assassin); }
        //spawn assassinBoss
        for(int i = 0; i < assassinBossSpawns[round]; i++) { SpawnMonster(EnemySpawner.MonsterID.assassinBoss); }
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
                    localUI.StartNewRoundCount();
                }
            }
            else
            {
                if(round == maxRounds)
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
                    localUI.StartNewRoundCount();
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
            ResetAppliedPowerups();
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
        PlayerSetup.localPlayerSetup.GetComponent<CharacterController>().enabled = false;
        PlayerSetup.localPlayerSetup.transform.SetPositionAndRotation(PlayerSpawn.playerSpawn.position, PlayerSpawn.playerSpawn.rotation);
        PlayerSetup.localPlayerSetup.ResetWeapons();
        PlayerSetup.localPlayerSetup.GetComponent<CharacterController>().enabled = true;
        PlayerSetup.localPlayerSetup.GetComponent<PlayerMovement>().ResetImpact();
    }

    /*Invincibility powerups code*/
    public void EnableOutline()
    {
        isInvincible = true;
        foreach(GameObject monster in spawnedMonsters)
        {
            monster.GetComponent<Outline>().enabled = true;
        }
    }

    public void DisableOutline()
    {
        isInvincible = false;
        foreach(GameObject monster in spawnedMonsters)
        {
            monster.GetComponent<Outline>().enabled = false;
        }
    }

    private void ResetAppliedPowerups()
    {
        PlayerPowerups playerPowerups = localPlayer.GetComponent<PlayerPowerups>();
        playerPowerups.ResetAppliedPowerups();
    }

    public void Respawn() //Called if an extra life is possessed
    {
        ResetPlayerPosition();
        localPlayer.GetComponent<PlayerHealth>().SetMaxHealth(0);
    }

    //Function called when non-host connects to set max rounds based on away player rounds
    public void SetHostRounds()
    {
        maxRounds = awayPlayer.maxRounds;
    }
}
