using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class MainMenuTest
{


    [UnityTest]
    public IEnumerator MainMenuScene_InitialState_HasAllCanvases() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Canvas optionsMenuCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        Canvas difficultySelectCanvas = GameObject.Find("DifficultySelect").GetComponent<Canvas>();
        Canvas guideDisplayCanvas = GameObject.Find("GuideDisplay").GetComponent<Canvas>();

        Assert.True(mainMenuCanvas);
        Assert.True(optionsMenuCanvas);
        Assert.True(difficultySelectCanvas);
        Assert.True(guideDisplayCanvas);

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);
        yield return null;
    }

    [UnityTest]
    public IEnumerator MainMenu_InitialState_HasTitle() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Transform titleTransform = mainMenuCanvas.transform.Find("Title");
        Assert.AreEqual("Abyssal Mayhem", titleTransform.GetComponentInChildren<TextMeshProUGUI>().text);
        yield return null;
    }

    [UnityTest]
    public IEnumerator MainMenu_InitialState_HasAllButtons() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Button[] mainMenuButtons = mainMenuCanvas.GetComponentsInChildren<Button>();
        Assert.AreEqual(5, mainMenuButtons.Length);

        Assert.True(mainMenuButtons.Any(button => button.GetComponentInChildren<TextMeshProUGUI>().text == "Play Infinite Spawn"));
        Assert.True(mainMenuButtons.Any(button => button.GetComponentInChildren<TextMeshProUGUI>().text == "Play Snipe To Win"));
        Assert.True(mainMenuButtons.Any(button => button.GetComponentInChildren<TextMeshProUGUI>().text == "Guide"));
        Assert.True(mainMenuButtons.Any(button => button.GetComponentInChildren<TextMeshProUGUI>().text == "Options"));
        Assert.True(mainMenuButtons.Any(button => button.GetComponentInChildren<TextMeshProUGUI>().text == "Quit"));
        yield return null;
    }

    [UnityTest]
    public IEnumerator MainMenu_EnableAndDisableOptions_ShouldToggleOptionsMenu() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Canvas optionsMenuCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        Canvas difficultySelectCanvas = GameObject.Find("DifficultySelect").GetComponent<Canvas>();
        Canvas guideDisplayCanvas = GameObject.Find("GuideDisplay").GetComponent<Canvas>();
        MenuGameManager menuGameManager = Object.FindObjectOfType<MenuGameManager>();

        menuGameManager.EnableOptions();
        yield return null;

        Assert.True(optionsMenuCanvas.enabled);
        Assert.False(mainMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);

        menuGameManager.DisableOptions();
        yield return null;

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);
    }

    [UnityTest]
    public IEnumerator MainMenu_EnableAndDisableDifficultySelect_ShouldToggleDifficultySelect() 
    {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Canvas optionsMenuCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        Canvas difficultySelectCanvas = GameObject.Find("DifficultySelect").GetComponent<Canvas>();
        Canvas guideDisplayCanvas = GameObject.Find("GuideDisplay").GetComponent<Canvas>();
        MenuGameManager menuGameManager = Object.FindObjectOfType<MenuGameManager>();

        menuGameManager.EnableDifficultySelect();
        yield return null;

        Assert.True(difficultySelectCanvas.enabled);
        Assert.False(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);

        menuGameManager.DisableDifficultySelect();
        yield return null;

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);
    }

    [UnityTest]
    public IEnumerator MainMenu_EnableGuide_ShouldEnableGuideCanvas() 
    {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Canvas optionsMenuCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        Canvas difficultySelectCanvas = GameObject.Find("DifficultySelect").GetComponent<Canvas>();
        Canvas guideDisplayCanvas = GameObject.Find("GuideDisplay").GetComponent<Canvas>();
        MenuGameManager menuGameManager = Object.FindObjectOfType<MenuGameManager>();

        menuGameManager.EnableGuide();
        yield return null;

        Assert.True(guideDisplayCanvas.enabled);
        Assert.False(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
    }

    [UnityTest]
    public IEnumerator MainMenu_BackToMainMenuCalled_ShouldResetToOnlyMainMenuEnabled() 
    {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        Canvas mainMenuCanvas = GameObject.Find("MainMenu").GetComponent<Canvas>();
        Canvas optionsMenuCanvas = GameObject.Find("OptionsMenu").GetComponent<Canvas>();
        Canvas difficultySelectCanvas = GameObject.Find("DifficultySelect").GetComponent<Canvas>();
        Canvas guideDisplayCanvas = GameObject.Find("GuideDisplay").GetComponent<Canvas>();
        MenuGameManager menuGameManager = Object.FindObjectOfType<MenuGameManager>();

        optionsMenuCanvas.enabled = true;
        menuGameManager.BackToMainMenu();
        yield return null;

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);

        difficultySelectCanvas.enabled = true;
        menuGameManager.BackToMainMenu();
        yield return null;

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);

        guideDisplayCanvas.enabled = true;
        menuGameManager.BackToMainMenu();
        yield return null;

        Assert.True(mainMenuCanvas.enabled);
        Assert.False(optionsMenuCanvas.enabled);
        Assert.False(difficultySelectCanvas.enabled);
        Assert.False(guideDisplayCanvas.enabled);
    }

    [UnityTest]
    public IEnumerator DifficultySelect_SelectEasyDifficulty_ShouldSetDifficultyAndLoadInfiniteSpawn()
    {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        SteamLobby steamLobby = Object.FindObjectOfType<SteamLobby>();
        steamLobby.SetEasyInfSpawn();
        yield return null;

        Assert.AreEqual(PlayerPrefs.GetInt("difficulty"), steamLobby.easyID);
        Assert.AreEqual(steamLobby.InfiniteSpawnScene, steamLobby.manager.onlineScene);
    }

    [UnityTest]
    public IEnumerator DifficultySelect_SelectMediumDifficulty_ShouldSetDifficultyAndLoadInfiniteSpawn() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        SteamLobby steamLobby = Object.FindObjectOfType<SteamLobby>();
        steamLobby.SetMediumInfSpawn();
        yield return null;

        Assert.AreEqual(PlayerPrefs.GetInt("difficulty"), steamLobby.mediumID);
        Assert.AreEqual(steamLobby.InfiniteSpawnScene, steamLobby.manager.onlineScene);
    }

    [UnityTest]
    public IEnumerator DifficultySelect_SelectHardDifficulty_ShouldSetDifficultyAndLoadInfiniteSpawn() {
        SceneManager.LoadScene("MainMenuSteam");
        yield return null;

        SteamLobby steamLobby = Object.FindObjectOfType<SteamLobby>();
        steamLobby.SetHardInfSpawn();
        yield return null;

        Assert.AreEqual(PlayerPrefs.GetInt("difficulty"), steamLobby.hardID);
        Assert.AreEqual(steamLobby.InfiniteSpawnScene, steamLobby.manager.onlineScene);
    }
}