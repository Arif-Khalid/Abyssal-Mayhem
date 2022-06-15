using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider slider; //Slider controlling UI healthbar
    public Gradient gradient; //color gradient of health bar
    [SerializeField] Image fill; //image of frontfill of healthbar
    public bool dead = false;
    public bool invincible = false;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        
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
        dead = false;
    }
    public void TakeDamage(int damage, float duration, float magnitude)
    {
        if (invincible)
        {
            return;
        }
        if (dead)
        {
            return;
        }
        CameraShake.cameraShake.StartCoroutine(CameraShake.cameraShake.Shake(duration, magnitude));
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(currentHealth <= 0)
        {
            Death();
        }
    }

    public void RestoreHealth(int value)
    {
        currentHealth += value;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void Death()
    {
        GetComponent<PlayerSetup>().LocalDeath();
        dead = true;
    }

    private void OnEnable()
    {
        SetMaxHealth(maxHealth);
    }
}
