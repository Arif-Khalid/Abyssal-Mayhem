using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI localScoreText;
    [SerializeField] TextMeshProUGUI awayScoreText;
    [SerializeField] TextMeshProUGUI waitingText;
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI quotaText;

    public void UpdateLocalScore(int newLocalScore)
    {
        localScoreText.text = "Your Score: " + newLocalScore.ToString();
    }

    public void UpdateAwayScore(int newAwayScore)
    {
        awayScoreText.text = "Enemy Score: " + newAwayScore.ToString();
    }

    public void BothPlayersReady()
    {
        waitingText.enabled = false;
    }

    public void UpdateRoundText(int newRound, int newQuota)
    {
        roundText.text = "Round: " + newRound.ToString();
        quotaText.text = "Quota: " + newQuota.ToString();
    }
}
