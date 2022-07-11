using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using TMPro;
using UnityEngine.UI;

public class KCPLobby : MonoBehaviour
{
    [SerializeField]CustomNetworkManager customNetworkManager;
    [SerializeField] GameObject escapeMenu;
    [Scene] public string InfiniteSpawnScene;
    [Scene] public string SnipeToWinScene;
    [Scene] public string MainMenuScene;
    private bool isEscapeMenuActive = false;

    [SerializeField] int easyID;
    [SerializeField] int mediumID;
    [SerializeField] int hardID;

    [SerializeField] TextMeshProUGUI spawnJuggernautText;
    [SerializeField] TextMeshProUGUI spawnJuggernautBossText;
    [SerializeField] TextMeshProUGUI spawnAssassinText;
    [SerializeField] TextMeshProUGUI spawnAssassinBossText;
    [SerializeField] TextMeshProUGUI spawnFeedbackText;
    [SerializeField] Animator animator;
    [SerializeField] Slider mouseSensitivitySlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sensitivity"))
        {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat("Sensitivity");
        }
    }
    public void SetEasyInfSpawn()
    {
        ButtonAudio();
        PlayerPrefs.SetInt("difficulty", easyID);
        HostInfiniteSpawnLobby();
    }

    public void SetMediumInfSpawn()
    {
        ButtonAudio();
        PlayerPrefs.SetInt("difficulty", mediumID);
        HostInfiniteSpawnLobby();
    }

    public void SetHardInfSpawn()
    {
        ButtonAudio();
        PlayerPrefs.SetInt("difficulty", hardID);
        HostInfiniteSpawnLobby();
    }
    public void HostInfiniteSpawnLobby()
    {
        customNetworkManager.onlineScene = InfiniteSpawnScene;
        customNetworkManager.StartHost();
    }

    public void HostSnipeToWinLobby()
    {
        ButtonAudio();
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
        AudioManager.instance.StopAllSounds();
        ButtonAudio();
        if (NetworkServer.active && NetworkClient.isConnected)
        {
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

    /*Functions for spawning monsters in single player*/
    public void SpawnJuggernaut()
    {
        ButtonAudio();
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
        ButtonAudio();
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
        ButtonAudio();
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
        ButtonAudio();
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

    //Function called to change mouse sensitivity called in escape menu slider
    public void ChangeMouseSensitivity()
    {
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerPrefs.SetFloat("Sensitivity", mouseSensitivitySlider.value);
            PlayerSetup.localPlayerSetup.mouseLook.mouseSensitivity = mouseSensitivitySlider.value;
        }
    }

    public static void ButtonAudio()
    {
        AudioManager.instance.Play("ButtonPress");
    }
}
