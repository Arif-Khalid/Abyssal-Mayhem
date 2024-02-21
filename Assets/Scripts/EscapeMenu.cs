using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 * Responsible for containing logic related to the escape menu
 */
public class EscapeMenu : MonoBehaviour
{
    private CustomNetworkManager _manager;
    private bool _isEscapeMenuActive = false;
    private Canvas _escapeMenuCanvas;

    //Variables for spawning special monsters
    [SerializeField] private TextMeshProUGUI _spawnJuggernautText;
    [SerializeField] private TextMeshProUGUI _spawnJuggernautBossText;
    [SerializeField] private TextMeshProUGUI _spawnAssassinText;
    [SerializeField] private TextMeshProUGUI _spawnAssassinBossText;
    [SerializeField] private TextMeshProUGUI _spawnFeedbackText;
    [SerializeField] private Slider _mouseSensitivitySlider;
    [SerializeField] private Slider _escapeVolumeSlider;
    [SerializeField] private Slider _escapeMusicSlider;
    [SerializeField] private Animator _animator;

    private void Start() {
        _manager = GameObject.FindFirstObjectByType<CustomNetworkManager>();
        _escapeMenuCanvas = GetComponent<Canvas>();
        if (!_manager) {
            Debug.LogWarning("Network manager cannot be found");
        }

        if (PlayerPrefs.HasKey(Constants.SENSITIVITY_PREF_KEY)) {
            _mouseSensitivitySlider.value = PlayerPrefs.GetFloat(Constants.SENSITIVITY_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.VOLUME_PREF_KEY)) {
            _escapeVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOLUME_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.MUSIC_PREF_KEY)) {
            _escapeMusicSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_PREF_KEY);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name != Constants.MAIN_MENU_SCENE_NAME) {
            if (_isEscapeMenuActive) {
                DisableEscapeMenu();
            }
            else {
                EnableEscapeMenu();
            }
        }
    }

    public void EnableEscapeMenu() {
        if (PlayerManager.localPlayerSetup) {
            PlayerManager.localPlayerSetup.EnterEscapeMenu();
        }
        _escapeMenuCanvas.enabled = true;
        _isEscapeMenuActive = true;
    }

    public void DisableEscapeMenu() {
        if (PlayerManager.localPlayerSetup) {
            PlayerManager.localPlayerSetup.ExitEscapeMenu();
        }
        _escapeMenuCanvas.enabled = false;
        _isEscapeMenuActive = false;
    }

    public void QuitToMenu() {
        if (NetworkServer.active && NetworkClient.isConnected) {
            _manager.StopHost();
        }
        // stop client if client-only
        else if (NetworkClient.isConnected) {
            _manager.StopClient();
        }
        // stop server if server-only
        else if (NetworkServer.active) {
            _manager.StopServer();
        }
    }

    /* Functions for spawning monsters in single player */
    public void SpawnJuggernaut() {
        if (PlayerManager.localPlayerSetup && !PlayerManager.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerManager.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernaut);
            _spawnFeedbackText.color = _spawnJuggernautText.color;
            _spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_FEEDBACK;
            _animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, _animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnJuggernautBoss() {
        if (PlayerManager.localPlayerSetup && !PlayerManager.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerManager.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernautBoss);
            _spawnFeedbackText.color = _spawnJuggernautBossText.color;
            _spawnFeedbackText.text = Constants.SPAWN_JUGGERNAUT_BOSS_FEEDBACK;
            _animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, _animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassin() {
        if (PlayerManager.localPlayerSetup && !PlayerManager.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerManager.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassin);
            _spawnFeedbackText.color = _spawnAssassinText.color;
            _spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_FEEDBACK;
            _animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, _animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    public void SpawnAssassinBoss() {
        if (PlayerManager.localPlayerSetup && !PlayerManager.localPlayerSetup.enemySpawner.awayPlayerReady) {
            PlayerManager.localPlayerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.assassinBoss);
            _spawnFeedbackText.color = _spawnAssassinBossText.color;
            _spawnFeedbackText.text = Constants.SPAWN_ASSASSIN_BOSS_FEEDBACK;
            _animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, _animator.GetLayerIndex("Base Layer"), 0);
        }
        else {
            SpawnDisabledUI();
        }
    }

    private void SpawnDisabledUI() {
        _spawnFeedbackText.color = Color.white;
        _spawnFeedbackText.text = Constants.SPAWN_DISABLED_FEEDBACK;
        _animator.Play(Constants.SPAWN_FEEDBACK_ANIM_NAME, _animator.GetLayerIndex("Base Layer"), 0);
    }

    /* Functions called by the escape menu */
    public static void ButtonAudio() {
        AudioManager.instance.Play("ButtonPress");
    }

    public static void ButtonAudio2() {
        AudioManager.instance.Play("ButtonPress2");
    }

    public static void BackAudio() {
        AudioManager.instance.Play("UIBack");
    }

    public void OnMasterVolumeChange() {
        PlayerPrefs.SetFloat(Constants.VOLUME_PREF_KEY, _escapeVolumeSlider.value);
        AudioListener.volume = _escapeVolumeSlider.value;
    }

    public void OnMusicVolumeChange() {
        PlayerPrefs.SetFloat(Constants.MUSIC_PREF_KEY, _escapeMusicSlider.value);
        AudioManager.instance.ChangeBackgroundVolume(_escapeMusicSlider.value);
    }

    public void OnMouseSensitivityChange() {
        if (PlayerManager.localPlayerSetup) {
            PlayerPrefs.SetFloat(Constants.SENSITIVITY_PREF_KEY, _mouseSensitivitySlider.value);
            PlayerManager.localPlayerSetup.mouseLook.mouseSensitivity = _mouseSensitivitySlider.value;
        }
    }
}
