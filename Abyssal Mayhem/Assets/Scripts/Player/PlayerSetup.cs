using UnityEngine;
using Mirror;

public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    Camera sceneCamera;

    private void Start()
    {
        if (!isLocalPlayer)
        {
            /* for (int i = 0; i < componentsToDisable.Length; i++)
             {
                 componentsToDisable[i].enabled = false;
             }*/
            gameObject.SetActive(false);
        }
        else
        {
            sceneCamera = Camera.main;
            if (sceneCamera)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            EnemySpawner enemySpawner = FindObjectOfType<EnemySpawner>();
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
}
