using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class KCPLobby : MonoBehaviour
{
    [SerializeField]CustomNetworkManager customNetworkManager;
    [SerializeField] GameObject escapeMenu;
    [Scene] public string InfiniteSpawnScene;
    [Scene] public string SnipeToWinScene;
    [Scene] public string MainMenuScene;
    private bool isEscapeMenuActive = false;
    public void HostInfiniteSpawnLobby()
    {
        customNetworkManager.onlineScene = InfiniteSpawnScene;
        customNetworkManager.StartHost();
    }

    public void HostSnipeToWinLobby()
    {
        customNetworkManager.onlineScene = SnipeToWinScene;
        customNetworkManager.StartHost();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name != "MainMenuKCP" && Input.GetKeyDown(KeyCode.Escape))
        {
            if (isEscapeMenuActive)
            {
                DisableEscapeMenu();
            }
            else
            {
                EnableEscapeMenu();
            }           
        }
    }
    public void EnableEscapeMenu()
    {
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerSetup.localPlayerSetup.EnterEscapeMenu();
        }
        escapeMenu.SetActive(true);
        isEscapeMenuActive = true;
    }

    public void DisableEscapeMenu()
    {
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerSetup.localPlayerSetup.ExitEscapeMenu();
        }
        escapeMenu.SetActive(false);
        isEscapeMenuActive = false;
    }
    public void QuitToMenu()
    {
        Debug.Log("Entering QuitToMenu Function");
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("quitting");
           customNetworkManager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            customNetworkManager.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            customNetworkManager.StopServer();
        }
    }
}
