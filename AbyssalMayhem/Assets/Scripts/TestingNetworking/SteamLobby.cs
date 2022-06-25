using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
    //Callbacks
    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> JoinRequest;
    protected Callback<LobbyEnter_t> LobbyEntered;

    //Variables
    public ulong CurrentLobbyID;
    private const string HostAddressKey = "HostAddress";
    public CustomNetworkManager manager;

    //Gameobject
    //public GameObject HostButton;
    //public TMP_Text LobbyNameText;
    //public GameObject StopButton;
    [SerializeField] GameObject escapeMenu;

    bool isEscapeMenuActive = false;

    //Scenes
    [Scene] public string InfiniteSpawnScene;
    [Scene] public string SnipeToWinScene;
    [Scene] public string MainMenuScene;

    [SerializeField] int easyID;
    [SerializeField] int mediumID;
    [SerializeField] int hardID;

    //Variables for spawning special monsters
    [SerializeField] TextMeshProUGUI spawnJuggernautText;
    [SerializeField] TextMeshProUGUI spawnJuggernautBossText;
    [SerializeField] TextMeshProUGUI spawnAssassinText;
    [SerializeField] TextMeshProUGUI spawnAssassinBossText;
    [SerializeField] TextMeshProUGUI spawnFeedbackText;
    [SerializeField] Animator animator;
    [SerializeField] Slider mouseSensitivitySlider;
    public void SetEasyInfSpawn()
    {
        PlayerPrefs.SetInt("difficulty", easyID);
        HostInfiniteSpawnLobby();
    }

    public void SetMediumInfSpawn()
    {
        PlayerPrefs.SetInt("difficulty", mediumID);
        HostInfiniteSpawnLobby();
    }

    public void SetHardInfSpawn()
    {
        PlayerPrefs.SetInt("difficulty", hardID);
        HostInfiniteSpawnLobby();
    }
    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        //manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    private void Update()
    {
        //REPLACE MAIN MENU STEAM WITH MAIN MENU NAME IF CHANGED MAIN MENU SCENE NAME
        //USING MAIN MENU SCENE VARIABLE DOES NOT WORK
        if (SceneManager.GetActiveScene().name != "MainMenuSteam" && Input.GetKeyDown(KeyCode.Escape))
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
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            Debug.Log("quitting");
            manager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            manager.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            manager.StopServer();
        }
    }
        public void HostInfiniteSpawnLobby()
    {
        manager.onlineScene = InfiniteSpawnScene;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }

    public void HostSnipeToWinLobby()
    {
        manager.onlineScene = SnipeToWinScene;
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
    }
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created succesfully");

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAddressKey, SteamUser.GetSteamID().ToString());

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");

        manager.StartHost();

    }
    
    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Request To Join Lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        //Everyone
        /*HostButton.SetActive(false);
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        LobbyNameText.gameObject.SetActive(true);
        LobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");*/
        CurrentLobbyID = callback.m_ulSteamIDLobby;
        //Clients
        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();
    }

    /*Functions for spawning monsters in single player*/
    public void SpawnJuggernaut()
    {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady)
        {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernaut);
            spawnFeedbackText.color = spawnJuggernautText.color;
            spawnFeedbackText.text = "juggernaut spawned";
            animator.Play("SpawnFeedback", animator.GetLayerIndex("Base Layer"), 0);
        }
        else
        {
            SpawnDisabledUI();
        }
    }

    public void SpawnJuggernautBoss()
    {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady)
        {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernautBoss);
            spawnFeedbackText.color = spawnJuggernautBossText.color;
            spawnFeedbackText.text = "juggernaut boss spawned";
            animator.Play("SpawnFeedback", animator.GetLayerIndex("Base Layer"), 0);
        }
        else
        {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassin()
    {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady)
        {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassin);
            spawnFeedbackText.color = spawnAssassinText.color;
            spawnFeedbackText.text = "assassin spawned";
            animator.Play("SpawnFeedback", animator.GetLayerIndex("Base Layer"), 0);
        }
        else
        {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassinBoss()
    {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady)
        {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassinBoss);
            spawnFeedbackText.color = spawnAssassinBossText.color;
            spawnFeedbackText.text = "assassin boss spawned";
            animator.Play("SpawnFeedback", animator.GetLayerIndex("Base Layer"), 0);
        }
        else
        {
            SpawnDisabledUI();
        }
    }

    private void SpawnDisabledUI()
    {
        spawnFeedbackText.color = Color.white;
        spawnFeedbackText.text = "spawning disabled";
        animator.Play("SpawnFeedback", animator.GetLayerIndex("Base Layer"), 0);
    }

    //Function called in escape menu mouse sensitivity slider
    public void ChangeMouseSensitivity()
    {
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerSetup.localPlayerSetup.mouseLook.mouseSensitivity = mouseSensitivitySlider.value;
        }
    }
}
