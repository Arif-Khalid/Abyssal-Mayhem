using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    //References
    EnemySpawner enemySpawner;

    //Objects and behaviours to disable
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject[] gameObjectsToDisable;
    [SerializeField] GameObject awayUI;
    Camera sceneCamera;
    PlayerUI playerUI;

    //Score to be kept track of on server
    [SyncVar(hook = nameof(scoreChange))] int myScore = 0;

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

    /*Code for connection and disconnection of clients*/


    //Disable sceneCamera and disable awayUI gameObject
    //Set local variables in enemySpawner
    private void LocalPlayerSpawned()
    {
        
        sceneCamera = Camera.main;
        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(false);
        }
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
        
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        for (int i = 0; i < gameObjectsToDisable.Length; i++)
        {
            gameObjectsToDisable[i].SetActive(false);
        }
        this.gameObject.layer = 0;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<CharacterController>().enabled = false;
        playerUI.UpdateAwayScore(0);
        enemySpawner.awayPlayer = this;
        enemySpawner.awayUI = playerUI;
        enemySpawner.awayPlayerReadyUp();
    }
    private void OnDisable()
    {
        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(true);
        }
        if (isLocalPlayer)
        {
            enemySpawner.localPlayerReady = false;
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
    //Function to call when player killed an enemy
    //Increases player score and spawns enemy for opponent
    public void killedAnEnemy(Vector3 position)
    {
        spawnEnemy(position);
        myScore += 1;
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
    /*Code called on Server->Client*/

    [ClientRpc]
    //Function that restarts local game for local client and signals away player ready if not local player
    private void RestartLocalGame()
    {
        if (isLocalPlayer)
        {
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = true;
            }
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
    public void spawnEnemy(Vector3 position)
    {
        if (isLocalPlayer)
        {
            Debug.Log("You killed something");
        }
        else
        {
            enemySpawner.SpawnMonsterAtPoint(position);
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
    //Function running on client when local client dies
    public void LocalDeath()
    {
        enemySpawner.LocalDeath();
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
        NetworkLocalDeath();
    }
}
