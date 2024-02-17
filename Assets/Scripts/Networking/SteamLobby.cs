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

    public int easyID;
    public int mediumID;
    public int hardID;

    //Variables for spawning special monsters
    [SerializeField] TextMeshProUGUI spawnJuggernautText;
    [SerializeField] TextMeshProUGUI spawnJuggernautBossText;
    [SerializeField] TextMeshProUGUI spawnAssassinText;
    [SerializeField] TextMeshProUGUI spawnAssassinBossText;
    [SerializeField] TextMeshProUGUI spawnFeedbackText;
    [SerializeField] Animator animator;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider optionsVolumeSlider;
    [SerializeField] Slider optionsMusicSlider;
    [SerializeField] Slider escapeVolumeSlider;
    [SerializeField] Slider escapeMusicSlider;
    public void SetEasyInfSpawn()
    {
        PlayerPrefs.SetInt(Constants.DIFFICULTY_PREF_KEY, easyID);
        HostInfiniteSpawnLobby();
    }

    public void SetMediumInfSpawn()
    {
        PlayerPrefs.SetInt(Constants.DIFFICULTY_PREF_KEY, mediumID);
        HostInfiniteSpawnLobby();
    }

    public void SetHardInfSpawn()
    {
        PlayerPrefs.SetInt(Constants.DIFFICULTY_PREF_KEY, hardID);
        HostInfiniteSpawnLobby();
    }
    private void Start()
    {
        if (!SteamManager.Initialized) { return; }

        if (PlayerPrefs.HasKey(Constants.SENSITIVITY_PREF_KEY))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat(Constants.SENSITIVITY_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.VOLUME_PREF_KEY))
        {
            optionsVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOLUME_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.MUSIC_PREF_KEY))
        {
            optionsMusicSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_PREF_KEY);
        }
        //manager = GetComponent<CustomNetworkManager>();

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != Constants.MAIN_MENU_SCENE_NAME)
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
            spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
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
            spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_BOSS_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
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
            spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
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
            spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_BOSS_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
        }
        else
        {
            SpawnDisabledUI();
        }
    }

    private void SpawnDisabledUI()
    {
        spawnFeedbackText.color = Color.white;
        spawnFeedbackText.text = Constants.SPAWN_DISABLED_FEEDBACK;
        animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
    }

    //Function called in escape menu mouse sensitivity slider
    public void ChangeMouseSensitivity()
    {
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerPrefs.SetFloat(Constants.SENSITIVITY_PREF_KEY, mouseSensitivitySlider.value);
            PlayerSetup.localPlayerSetup.mouseLook.mouseSensitivity = mouseSensitivitySlider.value;
        }
    }

    //Function that plays button being pressed sound
    public static void ButtonAudio()
    {
        AudioManager.instance.Play("ButtonPress");
    }

    //Function that plays a different button being pressed sound
    public static void ButtonAudio2()
    {
        AudioManager.instance.Play("ButtonPress2");
    }
    //Function that plays UI reverting to previous menu sound
    public static void BackAudio()
    {
        AudioManager.instance.Play("UIBack");
    }

    //Function called when changing music slider in escape menu
    public void ChangeEscapeMusicSlider()
    {
        PlayerPrefs.SetFloat(Constants.MUSIC_PREF_KEY, escapeMusicSlider.value);
        optionsMusicSlider.value = escapeMusicSlider.value;
        //Change the music volume
        AudioManager.instance.ChangeBackgroundVolume(escapeMusicSlider.value);
    }

    //Function called when changing volume slider in escape menu
    public void ChangeEscapeVolumeSlider()
    {
        PlayerPrefs.SetFloat(Constants.VOLUME_PREF_KEY, escapeVolumeSlider.value);
        optionsVolumeSlider.value = escapeVolumeSlider.value;
        //Change the master volume
        AudioListener.volume = escapeVolumeSlider.value;
    }

    //Function called when changing music slider in options menu
    public void ChangeOptionsMusicSlider()
    {
        escapeMusicSlider.value = optionsMusicSlider.value;
    }

    //Function called when changing master volume slider in options menu
    public void ChangeOptionsVolumeSlider()
    {
        escapeVolumeSlider.value = optionsVolumeSlider.value;
    }
}
