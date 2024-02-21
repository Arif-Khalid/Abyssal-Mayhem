using UnityEngine;
using UnityEngine.UI;

public class MenuGameManager : MonoBehaviour
{
    [SerializeField] private Canvas _mainMenu;
    [SerializeField] private Canvas _optionsMenu;
    [SerializeField] private Canvas _difficultyMenu;
    [SerializeField] private Canvas _guide;

    [Header("Under guide display")]
    [SerializeField] private GameObject[] _slides;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _prevButton;
    private int _currentSlide = 0;                          // The current slide of the guide display

    [Header("Under options menu")]
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;


    public void Start() {
        _currentSlide = 0;
        SetCurrentSlide();
        if (PlayerPrefs.HasKey(Constants.VOLUME_PREF_KEY)) {
            _masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOLUME_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.MUSIC_PREF_KEY)) {
            _musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_PREF_KEY);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !_mainMenu.enabled) {
            BackToMainMenu();
        }
    }

    public void EnableOptions() {
        AudioManager.instance.Play("ButtonPress");
        _mainMenu.enabled = false;
        _optionsMenu.enabled = true;
    }

    public void DisableOptions() {
        AudioManager.instance.Play("ButtonPress");
        _mainMenu.enabled = true;
        _optionsMenu.enabled = false;
    }

    public void EnableDifficultySelect() {
        AudioManager.instance.Play("ButtonPress");
        _mainMenu.enabled = false;
        _difficultyMenu.enabled = true;
    }

    public void DisableDifficultySelect() {
        AudioManager.instance.Play("ButtonPress");
        _mainMenu.enabled = true;
        _difficultyMenu.enabled = false;
    }

    public void EnableGuide() {
        AudioManager.instance.Play("ButtonPress");
        _mainMenu.enabled = false;
        _guide.enabled = true;
    }
    public void QuitGame() {
        AudioManager.instance.Play("ButtonPress");
        Application.Quit();
    }

    public void BackToMainMenu() {
        AudioManager.instance.Play("UIBack");
        _mainMenu.enabled = true;
        _guide.enabled = false;
        _difficultyMenu.enabled = false;
        _optionsMenu.enabled = false;
        _currentSlide = 0;
        SetCurrentSlide();
    }

    public void SetCurrentSlide() {
        foreach (GameObject slide in _slides) {
            slide.SetActive(false);
        }
        _slides[_currentSlide].SetActive(true);
        if (_currentSlide == 0) {
            _prevButton.interactable = false;
            _nextButton.interactable = true;
        }
        else if (_currentSlide == _slides.Length - 1) {
            _nextButton.interactable = false;
            _prevButton.interactable = true;
        }
        else {
            _nextButton.interactable = true;
            _prevButton.interactable = true;
        }
    }

    public void MoveSlide(int moveNumber) {
        AudioManager.instance.Play("ButtonPress");
        _currentSlide += moveNumber;
        SetCurrentSlide();
    }

    public void OnMasterVolumeChange() {
        PlayerPrefs.SetFloat(Constants.VOLUME_PREF_KEY, _masterVolumeSlider.value);
        AudioListener.volume = _masterVolumeSlider.value;
    }

    public void OnMusicVolumeChange() {
        PlayerPrefs.SetFloat(Constants.MUSIC_PREF_KEY, _musicVolumeSlider.value);
        AudioManager.instance.ChangeBackgroundVolume(_musicVolumeSlider.value);
    }
}
