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
    }

    public void BothPlayersNotReady()
    {
        waitingText.enabled = true;
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
}