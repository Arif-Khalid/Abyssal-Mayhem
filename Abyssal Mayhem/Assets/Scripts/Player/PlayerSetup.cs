using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    EnemySpawner enemySpawner;
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject[] gameObjectsToDisable;
    [SerializeField]
    GameObject awayUI;
    [SyncVar(hook = nameof(scoreChange))] int myScore = 0;
    Camera sceneCamera;
    PlayerUI playerUI;

    private void Start()
    {
        playerUI = GetComponent<PlayerUI>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (!isLocalPlayer)
        {
            //Disable all components and gameObjects except awayUI and PlayerSetup
            //Set away variables in enemySpawner
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
        else
        {
            //Disable sceneCamera and disable awayUI gameObject
            //Set local variables in enemySpawner
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
    }

    private void OnDisable()
    {
        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
    
    [Command]
    //Function to call when player killed an enemy
    //Increases player score and spawns enemy for opponent
    public void killedAnEnemy(Vector3 position)
    {
        spawnEnemy(position);
        myScore += 1;
    }

    [Command]
    public void ResetScore()
    {
        myScore = 0;
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

    //updates UI and enemySpawner(for quota checking) on changes to score
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
}
