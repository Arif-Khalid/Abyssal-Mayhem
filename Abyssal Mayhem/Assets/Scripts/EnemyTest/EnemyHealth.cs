using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider slider; //Slider controlling UI healthbar
    public Gradient gradient; //color gradient of healthbar
    public Canvas canvas; //canvas housing healthbar in worldspace
    [SerializeField] Image fill; //image of frontfill of healthbar
    public Transform cameraTransform; //Reference to player camera
    PlayerSetup playerSetup; //Reference to player setup script
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(100);
        playerSetup = GetComponent<EnemyAI>().player.GetComponent<PlayerSetup>();
    }

    private void LateUpdate()
    {
        //Make health bar canvas face player camera
        if (cameraTransform)
        {
            canvas.transform.LookAt(canvas.transform.position + cameraTransform.forward); 
        }
    }
    public void SetMaxHealth(int value) //Set max health to a new value and restore current health
    {
        maxHealth = value;
        currentHealth = maxHealth;
        slider.maxValue = value;
        slider.value = value;
        fill.color = gradient.Evaluate(1f);
    }
    public void TakeDamage(int damage) //Take Damage
    {
        currentHealth -= damage;
        slider.value = currentHealth;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if(currentHealth <= 0)
        {
            DeathByPlayer();
        }
    }

    public void DeathByPlayer() //called when an enemy dies by a player
    {
        playerSetup.killedAnEnemy(transform.position);
        Destroy(this.gameObject);
    }

    public void Death() //called when an enemy dies
    {
        Destroy(this.gameObject);
    }
}
