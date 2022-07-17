using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider slider; //Slider controlling UI healthbar
    public Gradient gradient; //color gradient of health bar
    [SerializeField] Image fill; //image of frontfill of healthbar
    [SerializeField] Image medkitImage; //Image of regen medkit consumable
    [SerializeField] TextMeshProUGUI countText; //Count of cooldown of health restore
    [SerializeField] TextMeshProUGUI keyText; //Text to show what key to press to use health restore
    [SerializeField] Animator animator; //Animator for health regen animation
    [SerializeField] int healthRestored; //Amount of health restored by regeneration
    [SerializeField] float timePerTick; //Amount of time in between ticks of regeneration that restore 1 health per tick;
    [SerializeField] int cooldown; //Time in seconds before health regen is available again;
    public bool dead = false;
    public bool invincible = false;
    public int lives = 0;
    private float damageMultiplier = 1f;
    [Header("Difficulty Multipliers")]
    [SerializeField] float easyMultiplier;
    [SerializeField] float normalMultiplier;
    [SerializeField] float hardMultiplier;

    [SerializeField] PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && keyText.enabled)
        {
            StartRegen();
        }
    }
    public void SetMaxHealth(int value) //Set max health to a new value and restore current health
    {
        if (value == 0)
        {
            value = maxHealth;
        }
        maxHealth = value;
        currentHealth = maxHealth;
        slider.maxValue = value;
        slider.value = value;
        fill.color = gradient.Evaluate(1f);
        //Stop health regen and health regen countdown
        animator.SetBool("isRegen", false);
        StopAllCoroutines();
        AudioManager.instance.Stop("Regen");
        countText.text = string.Empty;
        keyText.enabled = true;
        medkitImage.color = Color.white;
        dead = false;
    }
    public void TakeDamage(int damage, float duration, float magnitude, string deathByName)
    {
        if (invincible)
        {
            return;
        }
        if (dead)
        {
            return;
        }
        damage = (int)(damage * damageMultiplier);
        CameraShake.cameraShake.StartCoroutine(CameraShake.cameraShake.Shake(duration, magnitude));
        playerUI.HurtUI(); //Getting hurt UI
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(currentHealth <= 0)
        {
            Death(deathByName);           
        }
    }

    public void RestoreHealth(int value)
    {
        currentHealth += value;
        if(currentHealth >= maxHealth) { currentHealth = maxHealth; }
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void Death(string deathByName)
    {
        dead = true;
        lives -= 1;
        if(lives < 0)
        {
            GetComponent<PlayerSetup>().LocalDeath(deathByName);
            dead = true;           
        }
        else
        {
            PlayerSetup.localPlayerSetup.enemySpawner.Respawn();
            playerUI.UpdateExtraLives(lives);
            playerUI.StartWarning("Extra Life Used");
            AudioManager.instance.Play("ExtraLife");
        }
        
    }

    private void OnEnable()
    {
        SetMaxHealth(maxHealth);
    }

    /*Extra life powerup code*/
    public void AddLife() //Called by extra life powerup
    {
        lives += 1;
        playerUI.UpdateExtraLives(lives);
    }

    public void ResetLives()
    {
        lives = 0;
        playerUI.UpdateExtraLives(lives);
    }

    public void StartRegen()
    {       
        StartCoroutine(nameof(StartRegenCountdown));
        StartCoroutine(nameof(Regen));
    }

    IEnumerator Regen()
    {
        AudioManager.instance.Play("Regen");
        animator.SetBool("isRegen", true);
        int totalTicks = healthRestored;
        while(totalTicks > 0)
        {
            totalTicks--;
            RestoreHealth(1);
            yield return new WaitForSeconds(timePerTick);
        }
        animator.SetBool("isRegen", false);
        AudioManager.instance.Stop("Regen");
    }
    IEnumerator StartRegenCountdown()
    {
        keyText.enabled = false;
        medkitImage.color = new Color32(67, 67, 67, 255);
        int i = cooldown;
        while(i >= 0)
        {
            countText.text = i.ToString();
            i--;
            yield return new WaitForSeconds(1f);
        }
        countText.text = string.Empty;
        keyText.enabled = true;
        medkitImage.color = Color.white;
    }

    public void SetDamageMultiplier(int difficultyID)
    {
        if(difficultyID == 0)
        {
            damageMultiplier = easyMultiplier;
        }
        else if(difficultyID == 1)
        {
            damageMultiplier = normalMultiplier;
        }
        else
        {
            damageMultiplier = hardMultiplier;
        }
    }
}
