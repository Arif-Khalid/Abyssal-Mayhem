using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    private CustomNetworkManager manager;
    private bool isEscapeMenuActive = false;
    private Canvas escapeMenuCanvas;

    //Variables for spawning special monsters
    [SerializeField] TextMeshProUGUI spawnJuggernautText;
    [SerializeField] TextMeshProUGUI spawnJuggernautBossText;
    [SerializeField] TextMeshProUGUI spawnAssassinText;
    [SerializeField] TextMeshProUGUI spawnAssassinBossText;
    [SerializeField] TextMeshProUGUI spawnFeedbackText;
    [SerializeField] Animator animator;
    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider escapeVolumeSlider;
    [SerializeField] Slider escapeMusicSlider;
    private void Start() {
        manager = GameObject.FindFirstObjectByType<CustomNetworkManager>();
        escapeMenuCanvas = GetComponent<Canvas>();
        if (!manager) {
            Debug.LogWarning("Network manager cannot be found");
        }

        if (PlayerPrefs.HasKey(Constants.SENSITIVITY_PREF_KEY)) {
            mouseSensitivitySlider.value = PlayerPrefs.GetFloat(Constants.SENSITIVITY_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.VOLUME_PREF_KEY)) {
            escapeVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOLUME_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.MUSIC_PREF_KEY)) {
            escapeMusicSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_PREF_KEY);
        }
    }
    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != Constants.MAIN_MENU_SCENE_NAME) {
            if (isEscapeMenuActive) {
                DisableEscapeMenu();
            }
            else {
                EnableEscapeMenu();
            }
        }
    }

    public void EnableEscapeMenu() {
        if (PlayerSetup.localPlayerSetup) {
            PlayerSetup.localPlayerSetup.EnterEscapeMenu();
        }
        escapeMenuCanvas.enabled = true;
        isEscapeMenuActive = true;
    }

    public void DisableEscapeMenu() {
        if (PlayerSetup.localPlayerSetup) {
            PlayerSetup.localPlayerSetup.ExitEscapeMenu();
        }
        escapeMenuCanvas.enabled = false;
        isEscapeMenuActive = false;
    }

    public void QuitToMenu() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            manager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected) {
            manager.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active) {
            manager.StopServer();
        }
    }

    /*Functions for spawning monsters in single player*/
    public void SpawnJuggernaut() {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernaut);
            spawnFeedbackText.color = spawnJuggernautText.color;
            spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnJuggernautBoss() {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernautBoss);
            spawnFeedbackText.color = spawnJuggernautBossText.color;
            spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_BOSS_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassin() {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassin);
            spawnFeedbackText.color = spawnAssassinText.color;
            spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassinBoss() {
        if (PlayerSetup.localPlayerSetup && !PlayerSetup.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerSetup.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassinBoss);
            spawnFeedbackText.color = spawnAssassinBossText.color;
            spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_BOSS_FEEDBACK;
            animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    private void SpawnDisabledUI() {
        spawnFeedbackText.color = Color.white;
        spawnFeedbackText.text = Constants.SPAWN_DISABLED_FEEDBACK;
        animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, animator.GetLayerIndex("Base Layer"), 0);
    }

    //Function called in escape menu mouse sensitivity slider
    public void OnMouseSensitivityChange() {
        if (PlayerSetup.localPlayerSetup) {
            PlayerPrefs.SetFloat(Constants.SENSITIVITY_PREF_KEY, mouseSensitivitySlider.value);
            PlayerSetup.localPlayerSetup.mouseLook.mouseSensitivity = mouseSensitivitySlider.value;
        }
    }

    //Function that plays button being pressed sound
    public static void ButtonAudio() {
        AudioManager.instance.Play("ButtonPress");
    }

    //Function that plays a different button being pressed sound
    public static void ButtonAudio2() {
        AudioManager.instance.Play("ButtonPress2");
    }
    //Function that plays UI reverting to previous menu sound
    public static void BackAudio() {
        AudioManager.instance.Play("UIBack");
    }

    //Function called when changing volume slider in escape menu
    public void OnMasterVolumeChange() {
        PlayerPrefs.SetFloat(Constants.VOLUME_PREF_KEY, escapeVolumeSlider.value);
        AudioListener.volume = escapeVolumeSlider.value;
    }
    //Function called when changing music slider in escape menu
    public void OnMusicVolumeChange() {
        PlayerPrefs.SetFloat(Constants.MUSIC_PREF_KEY, escapeMusicSlider.value);
        AudioManager.instance.ChangeBackgroundVolume(escapeMusicSlider.value);
    }

}
