using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuGameManager : MonoBehaviour
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas difficultyMenu;
    [SerializeField] Canvas guide;
    [SerializeField] GameObject[] slides;
    [SerializeField] Button nextButton;
    [SerializeField] Button prevButton;
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    int currentSlide = 0;


    public void Start()
    {
        currentSlide = 0;
        SetCurrentSlide();
        if (PlayerPrefs.HasKey(Constants.VOLUME_PREF_KEY)) {
            masterVolumeSlider.value = PlayerPrefs.GetFloat(Constants.VOLUME_PREF_KEY);
        }
        if (PlayerPrefs.HasKey(Constants.MUSIC_PREF_KEY)) {
            musicVolumeSlider.value = PlayerPrefs.GetFloat(Constants.MUSIC_PREF_KEY);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !mainMenu.enabled)
        {
            BackToMainMenu();
        }
    }

    public void EnableOptions()
    {
        AudioManager.instance.Play("ButtonPress");
        mainMenu.enabled = false;
        optionsMenu.enabled = true;
    }

    public void DisableOptions()
    {
        AudioManager.instance.Play("ButtonPress");
        mainMenu.enabled = true;
        optionsMenu.enabled = false;
    }

    public void EnableDifficultySelect()
    {
        AudioManager.instance.Play("ButtonPress");
        mainMenu.enabled = false;
        difficultyMenu.enabled = true;
    }

    public void DisableDifficultySelect()
    {
        AudioManager.instance.Play("ButtonPress");
        mainMenu.enabled = true;
        difficultyMenu.enabled = false; 
    }

    public void EnableGuide()
    {
        AudioManager.instance.Play("ButtonPress");
        mainMenu.enabled = false;
        guide.enabled = true;
    }
    public void QuitGame()
    {
        AudioManager.instance.Play("ButtonPress");
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        AudioManager.instance.Play("UIBack");
        mainMenu.enabled = true;
        guide.enabled = false;
        difficultyMenu.enabled = false;
        optionsMenu.enabled = false;
        currentSlide = 0;
        SetCurrentSlide();
    }

    public void SetCurrentSlide()
    {
        foreach(GameObject slide in slides)
        {
            slide.SetActive(false);
        }
        slides[currentSlide].SetActive(true);
        if(currentSlide == 0)
        {
            prevButton.interactable = false;
            nextButton.interactable = true;
        }
        else if(currentSlide == slides.Length - 1)
        {
            nextButton.interactable = false;
            prevButton.interactable = true;
        }
        else
        {
            nextButton.interactable = true;
            prevButton.interactable = true;
        }
    }

    public void MoveSlide(int moveNumber)
    {
        AudioManager.instance.Play("ButtonPress");
        currentSlide += moveNumber;
        SetCurrentSlide();
    }

    public void OnMasterVolumeChange() {
        PlayerPrefs.SetFloat(Constants.VOLUME_PREF_KEY, masterVolumeSlider.value);
        AudioListener.volume = masterVolumeSlider.value;
    }

    public void OnMusicVolumeChange() {
        PlayerPrefs.SetFloat(Constants.MUSIC_PREF_KEY, musicVolumeSlider.value);
        AudioManager.instance.ChangeBackgroundVolume(musicVolumeSlider.value);
    }
}
