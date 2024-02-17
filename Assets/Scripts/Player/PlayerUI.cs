using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    //References to text UI
    [SerializeField] TextMeshProUGUI localScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    [SerializeField] TextMeshProUGUI waitingText;
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI quotaText;
    [SerializeField] TextMeshProUGUI waitingPrompt;
    [SerializeField] TextMeshProUGUI interactPrompt;
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Image ammoImage;
    [SerializeField] RectTransform ammoImageTransform;
    [SerializeField] TextMeshProUGUI roundStartCount;
    private int roundStartCounter = 3;
    [SerializeField] TextMeshProUGUI warningText;
    [SerializeField] TextMeshProUGUI extraLifeText;
    [SerializeField] Image crossHairLines;
    [SerializeField] Image crossHairDot;
    [SerializeField] RectTransform crosshairTransform;
    
    [SerializeField] GameObject localUI;
    [SerializeField] GameObject deathUI;
    [SerializeField] TextMeshProUGUI deathByText;
    [SerializeField] GameObject winUI;
    [SerializeField] TextMeshProUGUI winUIDeathByText;
    [SerializeField] GameObject lossUI;

    [SerializeField] Animator animator;

    [Header("Prohibition")]
    private int count = 6;
    [SerializeField] GameObject prohibitionText;
    [SerializeField] TextMeshProUGUI prohibitionCount;
    [SerializeField] int prohibitionDamage;
    [SerializeField] int prohibitionForce;
    [SerializeField] float prohibitionShakeDuration = 0.15f;
    [SerializeField] float prohibitionShakeMagnitude = 0.4f;
    [SerializeField] string prohibitionDeathByName = "their own curiosity";

    [Header("GunUI")]
    [SerializeField] WeaponUI[] weaponUIs;

    /*Updates local and away score UI on clients*/
    public void UpdateLocalScore(int newLocalScore)
    {
        localScoreText.text = "Your Score: " + newLocalScore.ToString();
    }

    public void UpdateAwayScore(int newAwayScore)
    {
        awayScoreText.text = "Enemy Score: " + newAwayScore.ToString();
    }

    /*UI display for waiting for players*/
    public void BothPlayersReady()
    {
        waitingText.enabled = false;
        waitingPrompt.enabled = false;
    }

    public void BothPlayersNotReady()
    {
        waitingText.enabled = true;
        waitingPrompt.enabled = true;
    }

    /*UI Display for rounds and quotas*/

    public void UpdateRoundText(int newRound, int newQuota)
    {
        roundText.text = "Round: " + newRound.ToString();
        quotaText.text = "Quota: " + newQuota.ToString();
    }

    public void ResetRounds()
    {
        roundText.text = null;
        quotaText.text = null;
    }

    public void EnableDeathUI(string deathByName)
    {
        deathUI.SetActive(true);
        animator.Play("PlayerDie", animator.GetLayerIndex("EOG Layer"));
        deathByText.text = "You were killed by " + deathByName;
    }

    public void DisableDeathUI()
    {
        deathUI.SetActive(false);
    }

    public void EnableWinBySpeedUI()
    {
        winUI.SetActive(true);
        animator.Play("PlayerWin", animator.GetLayerIndex("EOG Layer"));
        winUIDeathByText.text = "You won by reaching the final quota first";
        
    }
    public void EnableWinUI(string deathByName)
    {
        winUI.SetActive(true);
        animator.Play("PlayerWin", animator.GetLayerIndex("EOG Layer"));
        winUIDeathByText.text = "Your opponent was killed by " + deathByName; 
    }


    public void DisableWinUI()
    {
        winUI.SetActive(false);
    }

    public void EnableLossUI()
    {
        lossUI.SetActive(true);
        animator.Play("PlayerLose", animator.GetLayerIndex("EOG Layer"));
    }

    public void DisableLossUI()
    {
        lossUI.SetActive(false);
    }

    public void UpdateWaitingPrompt()
    {
        waitingPrompt.text = "monsters and pickups now spawning";
    }

    public void ResetWaitingPrompt()
    {
        waitingPrompt.text = "Spawn monsters and pickups \n [X]";
    }

    public void UpdateInteractPrompt(string promptMessage)
    {
        interactPrompt.text = promptMessage;
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        if(ammoText == null)
        {
            return;
        }
        if(maxAmmo == -1)
        {
            ammoText.text = currentAmmo.ToString() + "/∞";
        }
        else
        {
            ammoText.text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
        }
    }

    public void UpdateAmmoSprite(AmmoImage ammo)
    {
        ammoImage.sprite = ammo.ammoSprite;
        ammoImageTransform.localScale = new Vector3(ammo.imageScale, ammo.imageScale, ammo.imageScale);
    }

    public void StartNewRoundCount()
    {
        roundStartCounter = 3;
        roundStartCount.text = roundStartCounter.ToString();
        animator.Play("RoundStartCount");
        AudioManager.instance.Play("Countdown");
        //Play the animation
    }

    public void UpdateRoundStartNumber()
    {
        roundStartCounter -= 1;
        roundStartCount.text = roundStartCounter.ToString();
    }

    public void StartNextRound()
    {
        PlayerSetup.localPlayerSetup.enemySpawner.StartNextRound();
    }
    //Function to call by player movement when player is standing on prohibited area
    public void StartProhibitionTimer()
    {
        AudioManager.instance.Play("Warning");
        count = 6;
        prohibitionText.SetActive(true);//enable the text
        StartCoroutine(Prohibition());
    }

    public void StopProhibitionTimer()
    {
        AudioManager.instance.Stop("Warning");
        //disable the text and stop coroutines
        StopAllCoroutines();
        prohibitionText.SetActive(false);
    }

    IEnumerator Prohibition()
    {
        //Count decrease until becomes 0
        while(count > 0)
        {
            count -= 1;
            prohibitionCount.text = count.ToString();
            yield return new WaitForSeconds(1f);
        }

        //Damage and push player up and off prohibited area
        AudioManager.instance.Play("Prohibition");
        GetComponent<PlayerHealth>().TakeDamage(prohibitionDamage, prohibitionShakeDuration, prohibitionShakeMagnitude, prohibitionDeathByName);
        if (GetComponent<PlayerHealth>().dead)
        {
            yield break;
        }
        GetComponent<PlayerMovement>().AddImpact(-transform.forward, prohibitionForce);
        
        //Restart coroutine if still on prohibited area
        StartProhibitionTimer();
    }

    public void HurtUI()
    {
        animator.Play("PlayerHurt", animator.GetLayerIndex("Hurt Layer"), 0f);
    }
    /*Code for powerups UI*/
    public void StartWarning(string message)
    {
        if (message != "Extra Life Used") { AudioManager.instance.Play("Warning"); }
        warningText.text = "!" + message + "!";
        animator.Play("Warning", animator.GetLayerIndex("Warning Layer"), 0f);
    }

    public void StopWarning()
    {
        AudioManager.instance.Stop("Warning");
    }
    public void StartBlind()
    {
        //Play blind animation
        AudioManager.instance.Play("Paranoia");
        animator.Play("Blind", animator.GetLayerIndex("Blind Layer"), 0f);
    }

    public void StopBlind()
    {
        AudioManager.instance.Stop("Paranoia");
    }
    public void StopAnimator()
    {
        //Stop warning animation
        animator.Play("Empty", animator.GetLayerIndex("Warning Layer"));
        //Stop blind animation as well
        animator.Play("Empty", animator.GetLayerIndex("Blind Layer"));
        //Stop death animation
        animator.Play("Empty", animator.GetLayerIndex("EOG Layer"));
        StopWarning();
    }

    public void UpdateExtraLives(int lives)
    {
        if(lives > 0)
        {
            extraLifeText.text = "+" + lives.ToString();
        }
        else
        {
            extraLifeText.text = string.Empty;
        }
    }

    public void UpdateCrosshair()
    {
        if (CrosshairSettings.instance.crosshairEnabled)
        {
            crossHairLines.enabled = CrosshairSettings.instance.outerLinesEnabled;
            crossHairDot.enabled = CrosshairSettings.instance.dotEnabled;
            crossHairLines.color = CrosshairSettings.instance.color;
            crossHairDot.color = CrosshairSettings.instance.color;
            crosshairTransform.localScale = new Vector3(CrosshairSettings.instance.crosshairSize,
                CrosshairSettings.instance.crosshairSize, CrosshairSettings.instance.crosshairSize);
        }
        else
        {
            crossHairLines.enabled = false;
            crossHairDot.enabled = false;
        }
    }

    public void SetActiveWeaponUI(int ID)
    {
        for(int i = 0; i < weaponUIs.Length; i++)
        {
            if(i != ID) { weaponUIs[i].DeactivateWeaponUI(); }
            else { weaponUIs[i].ActivateWeaponUI(); }
        }
    }

    public void EnableWeaponUI(int ID)
    {
        weaponUIs[ID].gameObject.SetActive(true);
    }

    public void DisableWeaponUI(int ID)
    {
        weaponUIs[ID].gameObject.SetActive(false);
    }
}
