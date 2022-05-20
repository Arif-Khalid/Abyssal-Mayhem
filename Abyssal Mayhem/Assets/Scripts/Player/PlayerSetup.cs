using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    EnemySpawner enemySpawner;
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject[] gameObjectsToDisable;

    Camera sceneCamera;

    private void Start()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        if (!isLocalPlayer)
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
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera)
            {
                sceneCamera.gameObject.SetActive(false);
            }            
            enemySpawner.player = gameObject.transform;
            enemySpawner.cameraTransform = GetComponentInChildren<Camera>().transform;
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
    public void killedAnEnemy(Vector3 position)
    {
        spawnEnemy(position);
    }

    [ClientRpc]
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
}
