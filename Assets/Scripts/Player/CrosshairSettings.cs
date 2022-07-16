using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairSettings : MonoBehaviour
{
    [Header("Color Presets")]
    [SerializeField] Color greenColor;
    [SerializeField] Image greenSelectImage;
    [SerializeField] Color blueColor;
    [SerializeField] Image blueSelectImage;
    [SerializeField] Color whiteColor;
    [SerializeField] Image whiteSelectImage;
    [SerializeField] Color pinkColor;
    [SerializeField] Image pinkSelectImage;
    [SerializeField] Color yellowColor;
    [SerializeField] Image yellowSelectImage;
    [SerializeField] Color orangeColor;
    [SerializeField] Image orangeSelectImage;

    [Header("Other references")]
    [SerializeField] Image crosshairEnabledTickImage;
    [SerializeField] Image dotEnabledTickImage;
    [SerializeField] Image outerLinesEnabledTickImage;
    [SerializeField] Slider crosshairSizeSlider;
    //0 is green, 1 is blue, 2 is white, 3 is pink, 4 is yellow, 5 is orange
    List<Color> colors = new List<Color>();
    List<Image> selectImages = new List<Image>();

    public static CrosshairSettings instance;
    [HideInInspector]
    public Color color;
    [HideInInspector]
    public bool crosshairEnabled = true;
    [HideInInspector]
    public bool dotEnabled = true;
    [HideInInspector]
    public bool outerLinesEnabled = true;
    [HideInInspector]
    public float crosshairSize = 0.35f;
    private void Awake()
    {       
        if (instance != null) { Destroy(this.gameObject); return; }
        instance = this;
        colors.Add(greenColor);
        colors.Add(blueColor);
        colors.Add(whiteColor);
        colors.Add(pinkColor);
        colors.Add(yellowColor);
        colors.Add(orangeColor);
        selectImages.Add(greenSelectImage);
        selectImages.Add(blueSelectImage);
        selectImages.Add(whiteSelectImage);
        selectImages.Add(pinkSelectImage);
        selectImages.Add(yellowSelectImage);
        selectImages.Add(orangeSelectImage);
        if (PlayerPrefs.HasKey("CrosshairColors"))
        {
            ChangeCrosshairColor(PlayerPrefs.GetInt("CrosshairColors"));
        }
        else
        {
            ChangeCrosshairColor(2); //Set default color to white
        }
        if (PlayerPrefs.HasKey("CrosshairEnable"))
        {
            if (PlayerPrefs.GetInt("CrosshairEnable") == 0) //0 is false thus crosshair is disabled
            {
                ToggleCrosshair();
            }
        }
        if (PlayerPrefs.HasKey("DotEnable"))
        {
            if (PlayerPrefs.GetInt("DotEnable") == 0) //0 is false thus dot is disabled
            {
                ToggleDot();
            }
        }
        if (PlayerPrefs.HasKey("OuterLinesEnable"))
        {
            if (PlayerPrefs.GetInt("OuterLinesEnable") == 0) //0 is false thus outer lines is disabled
            {
                ToggleOuterLines();
            }
        }
        if (PlayerPrefs.HasKey("CrosshairSize"))
        {
            crosshairSizeSlider.value = PlayerPrefs.GetFloat("CrosshairSize");
            ChangeCrosshairSize();
        }
    }

    public void ChangeCrosshairColor(int colorID)
    {
        //Change the escape menu UI to reflect color selected
        foreach(Image i in selectImages) { i.enabled = false; }
        selectImages[colorID].enabled = true;
        //Save the crosshair color
        PlayerPrefs.SetInt("CrosshairColors", colorID);
        color = colors[PlayerPrefs.GetInt("CrosshairColors")];
        //Set the crosshair color
        if (PlayerSetup.localPlayerSetup)
        {
            SteamLobby.ButtonAudio2();
            PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
        }
    }

    public void ToggleCrosshair()
    {
        if (crosshairEnabledTickImage.enabled)
        {
            //Disable crosshair
            crosshairEnabled = false;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("CrosshairEnable", 0);
            crosshairEnabledTickImage.enabled = false;
        }
        else
        {
            //Enable crosshair
            crosshairEnabled = true;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("CrosshairEnable", 1);
            crosshairEnabledTickImage.enabled = true;
        }
    }

    public void ToggleDot()
    {
        if (dotEnabledTickImage.enabled)
        {
            //Disable crosshair            
            dotEnabled = false;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("DotEnable", 0);
            dotEnabledTickImage.enabled = false;
        }
        else
        {
            //Enable crosshair            
            dotEnabled = true;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("DotEnable", 1);
            dotEnabledTickImage.enabled = true;
        }
    }

    public void ToggleOuterLines()
    {
        if (outerLinesEnabledTickImage.enabled)
        {
            //Disable crosshair            
            outerLinesEnabled = false;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("OuterLinesEnable", 0);
            outerLinesEnabledTickImage.enabled = false;
        }
        else
        {
            //Enable crosshair            
            outerLinesEnabled = true;
            if (PlayerSetup.localPlayerSetup)
            {
                SteamLobby.ButtonAudio2();
                PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
            }
            PlayerPrefs.SetInt("OuterLinesEnable", 1);
            outerLinesEnabledTickImage.enabled = true;
        }
    }

    public void ChangeCrosshairSize()
    {
        crosshairSize = crosshairSizeSlider.value;
        if (PlayerSetup.localPlayerSetup)
        {
            PlayerSetup.localPlayerSetup.playerUI.UpdateCrosshair();
        }
        PlayerPrefs.SetFloat("CrosshairSize", crosshairSize);
    }
}
