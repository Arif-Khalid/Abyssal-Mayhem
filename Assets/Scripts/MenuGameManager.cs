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
    int currentSlide = 0;


    private void OnEnable()
    {
        currentSlide = 0;
        SetCurrentSlide();
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
        SteamLobby.ButtonAudio();
        //Enable options menu and disable main menu
        mainMenu.enabled = false;
        optionsMenu.enabled = true;
    }

    public void DisableOptions()
    {
        SteamLobby.ButtonAudio();
        //Disable options menu and enable main menu
        mainMenu.enabled = true;
        optionsMenu.enabled = false;
    }

    public void EnableDifficultySelect()
    {
        SteamLobby.ButtonAudio();
        //Disable main menu and enabled difficulty select
        mainMenu.enabled = false;
        difficultyMenu.enabled = true;
    }

    public void DisableDifficultySelect()
    {
        SteamLobby.ButtonAudio();
        //Disable difficulty select and enabled main menu
        mainMenu.enabled = true;
        difficultyMenu.enabled = false; 
    }

    public void EnableGuide()
    {
        SteamLobby.ButtonAudio();
        //Disable main menu and enabled guide
        mainMenu.enabled = false;
        guide.enabled = true;
    }
    public void QuitGame()
    {
        SteamLobby.ButtonAudio();
        Application.Quit();
    }

    private void BackToMainMenu()
    {
        SteamLobby.BackAudio();
        mainMenu.enabled = true;
        guide.enabled = false;
        currentSlide = 0;
        SetCurrentSlide();
        difficultyMenu.enabled = false;
        optionsMenu.enabled = false;
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
        }else if(currentSlide == slides.Length - 1)
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
        SteamLobby.ButtonAudio();
        currentSlide += moveNumber;
        SetCurrentSlide();
    }
}
