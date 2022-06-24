using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuGameManager : MonoBehaviour
{
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas optionsMenu;
    [SerializeField] Canvas difficultyMenu;
    public void StartInfiniteSpawn()
    {
        SceneManager.LoadScene(1);
    }

    public void EnableOptions()
    {
        //Enable options menu and disable main menu
        mainMenu.enabled = false;
        optionsMenu.enabled = true;
    }

    public void DisableOptions()
    {
        //Disable options menu and enable main menu
        mainMenu.enabled = true;
        optionsMenu.enabled = false;
    }

    public void EnableDifficultySelect()
    {
        //Disable main menu and enabled difficulty select
        mainMenu.enabled = false;
        difficultyMenu.enabled = true;
    }

    public void DisableDifficultySelect()
    {
        //Disable difficulty select and enabled main menu
        mainMenu.enabled = true;
        difficultyMenu.enabled = false; 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
