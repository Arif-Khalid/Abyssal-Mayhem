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
    [SerializeField] TextMeshProUGUI roundStartCount;
    private int roundStartCounter = 3;
    [SerializeField] TextMeshProUGUI warningText;
    
    [SerializeField] GameObject localUI;
    [SerializeField] GameObject deathUI;
    [SerializeField] GameObject winUI;

    [SerializeField] Animator animator;
    
    [Header("Prohibition")]
    private int count = 6;
    [SerializeField] GameObject prohibitionText;
    [SerializeField] TextMeshProUGUI prohibitionCount;
    [SerializeField] int prohibitionDamage;
    [SerializeField] int prohibitionForce;
    [SerializeField] float prohibitionShakeDuration = 0.15f;
    [SerializeField] float prohibitionShakeMagnitude = 0.4f;



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

    public void StartNewRoundCount()
    {
        roundStartCounter = 3;
        roundStartCount.text = roundStartCounter.ToString();
        animator.Play("RoundStartCount");
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
        count = 6;
        prohibitionText.SetActive(true);//enable the text
        StartCoroutine(Prohibition());
    }

    public void StopProhibitionTimer()
    {
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
        GetComponent<PlayerHealth>().TakeDamage(prohibitionDamage, prohibitionShakeDuration, prohibitionShakeMagnitude);
        if (GetComponent<PlayerHealth>().dead)
        {
            yield break;
        }
        GetComponent<PlayerMovement>().AddImpact(-transform.forward, prohibitionForce);
        
        //Restart coroutine if still on prohibited area
        StartProhibitionTimer();
    }

    public void StartWarning(string message)
    {
        warningText.text = "!Incoming " + message + "!";
        animator.Play("Warning", animator.GetLayerIndex("Warning Layer"), 0f);
    }
}
