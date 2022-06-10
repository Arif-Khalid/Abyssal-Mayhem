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
    [SerializeField] GameObject localUI;
    [SerializeField] GameObject deathUI;
    [SerializeField] GameObject winUI;



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

    public void EnableDeathUI()
    {
        deathUI.SetActive(true);
    }

    public void DisableDeathUI()
    {
        deathUI.SetActive(false);
    }

    public void EnableWinUI()
    {
        winUI.SetActive(true);
    }

    public void DisableWinUI()
    {
        winUI.SetActive(false);
    }

    public void UpdateWaitingPrompt()
    {
        waitingPrompt.text = "monsters now spawning";
    }

    public void ResetWaitingPrompt()
    {
        waitingPrompt.text = "Press X to spawn monsters while you wait";
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
}
