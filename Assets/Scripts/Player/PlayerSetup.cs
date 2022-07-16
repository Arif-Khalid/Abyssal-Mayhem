using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerSetup : NetworkBehaviour
{
    //References
    public EnemySpawner enemySpawner;
    public MouseLook mouseLook;
    public CameraShake cameraShake;
    public PlayerPowerups playerPowerups;

    //Objects and behaviours to disable
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject[] gameObjectsToDisable;
    [SerializeField] GameObject awayUI;
    public AudioListener audioListener;
    Camera sceneCamera;
    public PlayerUI playerUI;
    public PlayerWeapon playerWeapon;
    private bool isInMenu = false;
    private bool isInEscapeMenu = false;

    public static PlayerSetup localPlayerSetup;
    //Score to be kept track of on server
    [SyncVar(hook = nameof(scoreChange))] int myScore = 0;
    //Max number of rounds to be set by host
    [SyncVar] public int difficultyID;

    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (!isLocalPlayer)
        {
            AwayPlayerSpawned();
        }
        else
        {
            LocalPlayerSpawned();
        }
    }

    //Function called by enemyspawner when a new game starts or when both players are ready to reset weapon to default
    public void ResetWeapons()
    {
        playerWeapon.Equip(playerWeapon.defaultWeapon);
    }
    //Functions only called on the local player using static variable to lock and unlock the local mouse and shooting on entering and exiting UI and escape menu
    public void EnterUIMenu()
    {
        isInMenu = true;
        mouseLook.UnlockMouse();
        playerWeapon.weapon.EnterUI();
    }

    public void ExitUIMenu()
    {
        isInMenu = false;
        if (!isInEscapeMenu)
        {
            mouseLook.LockMouse();
            playerWeapon.weapon.ExitUI();
        }  
    }

    public void EnterEscapeMenu()
    {
        isInEscapeMenu = true;
        mouseLook.UnlockMouse();
        playerWeapon.weapon.EnterUI();
    }

    public void ExitEscapeMenu()
    {
        isInEscapeMenu = false;
        if (!isInMenu)
        {
            mouseLook.LockMouse();
            playerWeapon.weapon.ExitUI();
        }
    }

    //Disables or enabled components in components to disable
    public void SetComponents(bool isEnabled)
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = isEnabled;
        }
    }
    /*Code for connection and disconnection of clients*/


    //Disable sceneCamera and disable awayUI gameObject
    //Set local variables in enemySpawner
    private void LocalPlayerSpawned()
    {
        
        mouseLook = GetComponentInChildren<MouseLook>();
        playerWeapon = GetComponent<PlayerWeapon>();
        if (SceneManager.GetActiveScene().name == "TempSnipeToWin")
        {
            SetComponents(false);
            for (int i = 0; i < gameObjectsToDisable.Length; i++)
            {
                gameObjectsToDisable[i].SetActive(false);
            }
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<CharacterController>().enabled = false;
            return;
        }
        localPlayerSetup = this;
        if (isServer)
        {
            //Run CMD that sets difficulty rounds
            CmdSetDifficulty();
            GetComponent<PlayerHealth>().SetDamageMultiplier(difficultyID);
            enemySpawner.SetDifficulty(difficultyID);
        }
        else
        {
            //Take difficulty from away player
            enemySpawner.SetHostDamageMultiplier();
            enemySpawner.SetHostDifficulty();
        }
        sceneCamera = Camera.main;
        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(false);
        }
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            mouseLook.mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }
        playerUI.UpdateCrosshair();
        cameraShake.SetStatic();
        enemySpawner.localPlayer = transform;
        enemySpawner.cameraTransform = GetComponentInChildren<Camera>().transform;
        awayUI.SetActive(false);
        playerUI.UpdateLocalScore(0);
        enemySpawner.localUI = playerUI;
        enemySpawner.localPlayerReadyUp();
    }

    //Disable all components and gameObjects except awayUI and PlayerSetup
    //Set away variables in enemySpawner
    private void AwayPlayerSpawned()
    {
        
        SetComponents(false);
        for (int i = 0; i < gameObjectsToDisable.Length; i++)
        {
            gameObjectsToDisable[i].SetActive(false);
        }       
        this.gameObject.layer = 0;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        if (SceneManager.GetActiveScene().name == "TempSnipeToWin")
        {
            //Currently unimplemented snipe to win scene
            //So do nothing
            return;
        }
        playerUI.UpdateAwayScore(0);
        enemySpawner.awayPlayer = this;
        enemySpawner.awayUI = playerUI;
        enemySpawner.awayPlayerReadyUp();
    }
    private void OnDisable()
    {
        if (!enemySpawner)
        {
            return;
        }
        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        if (isLocalPlayer)
        {
            
            enemySpawner.localPlayerReady = false;
            localPlayerSetup = null;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            enemySpawner.UpdateAwayScore(0);
            enemySpawner.awayPlayerReady = false;
            enemySpawner.RestartGame();
        }
    }
    
    /*Code called on Client->Server*/

    [Command]
    //Function that sets the max rounds if local player is host
    private void CmdSetDifficulty()
    {
        difficultyID = PlayerPrefs.GetInt("difficulty");
    }

    [Command]
    //Function to call when player killed an enemy
    //Increases player score and spawns enemy for opponent
    public void killedAnEnemy(Vector3 position, EnemySpawner.MonsterID monsterID)
    {
        spawnEnemy(position, monsterID);
        if (monsterID == EnemySpawner.MonsterID.assassin || monsterID == EnemySpawner.MonsterID.juggernaut)
        {
            myScore += 2;
        }
        else if(monsterID == EnemySpawner.MonsterID.assassinBoss || monsterID == EnemySpawner.MonsterID.juggernautBoss)
        {
            myScore += 4;
        }
        else
        {
            myScore += 1;
        }
    }

    [Command]
    //Function to call when starting a new round or resetting game
    public void ResetScore()
    {
        myScore = 0;
    }

    [Command]
    private void NetworkLocalDeath()
    {
        PlayerDied();
    }

    [Command]
    //Function called by pressing play again button on death or win
    public void PlayAgain()
    {
        RestartLocalGame();
    }

    [Command]
    //Function called on a player surviving all rounds
    public void Survived()
    {
        PlayerSurvived();
    }

    //Functions for networked powerup pickups
    [Command]
    //Network juggernaut spawn pickups
    public void CmdJuggernautSpawn()
    {
        RpcJuggernautSpawn();
    }

    [Command]
    //Network paranoia pickup
    public void CmdParanoia()
    {
        RpcParanoia();
    }

    /*Code called on Server->Client*/


    [ClientRpc]
    //Function that restarts local game for local client and signals away player ready if not local player
    private void RestartLocalGame()
    {
        if (isLocalPlayer)
        {
            //mouseLook.LockMouse();
            //mouseLook.EnableRotation();
            SetComponents(true);
            GetComponent<PlayerHealth>().SetMaxHealth(0);
            ResetScore();
            enemySpawner.LocalPlayAgain();
        }
        else
        {
            enemySpawner.awayPlayerReadyUp();
        }
    }

    [ClientRpc]
    private void PlayerDied()
    {
        if (!isLocalPlayer)
        {
            enemySpawner.LocalWin();
        }
    }
    [ClientRpc]
    //Spawns an enemy for the player that didn't kill it
    public void spawnEnemy(Vector3 position, EnemySpawner.MonsterID monsterID)
    {
        if (isLocalPlayer)
        {
            Debug.Log("You killed something");
        }
        else
        {
            enemySpawner.SpawnMonsterAtPoint(position, monsterID);
        }
    }
    [ClientRpc]
    //Function called on the player that survived and won
    public void PlayerSurvived()
    {
        if (isLocalPlayer)
        {
            enemySpawner.LocalWin();
        }
        else
        {
            LocalLoss();
        }
    }

    //Functions called by server on client to signal powerup pickups
    [ClientRpc]
    //Start juggernaut routine on not local client
    public void RpcJuggernautSpawn()
    {
        if (!isLocalPlayer)
        {
            PlayerSetup.localPlayerSetup.playerPowerups.StartCoroutine(PlayerSetup.localPlayerSetup.playerPowerups.ActivateJuggernautSpawn());
        }
    }

    [ClientRpc]
    //Start paranoia routine on not local client
    public void RpcParanoia()
    {
        if (!isLocalPlayer)
        {
            PlayerSetup.localPlayerSetup.playerPowerups.StartCoroutine(PlayerSetup.localPlayerSetup.playerPowerups.ActivateParanoia());
        }
    }
    //Code called on change of score on server to update local and away score for clients
    private void scoreChange(int oldScore, int newScore)
    {
        if (isLocalPlayer)
        {           
            playerUI.UpdateLocalScore(newScore);
            enemySpawner.UpdateLocalScore(newScore);
        }
        else
        {
            playerUI.UpdateAwayScore(newScore);
            enemySpawner.UpdateAwayScore(newScore);
        }       
    }
    [Client]
    //Function called by away player when other player wins
    public void LocalLoss()
    {
        enemySpawner.LocalLoss();
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
    [Client]
    //Function running on client when local client dies
    public void LocalDeath()
    {
        enemySpawner.LocalDeath();
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        //mouseLook.StopRotation();
        NetworkLocalDeath();
    }
}
