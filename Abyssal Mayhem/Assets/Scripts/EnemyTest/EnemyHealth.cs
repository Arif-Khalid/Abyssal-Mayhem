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
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        SetMaxHealth(100);
    }

    private void LateUpdate()
    {
        if (cameraTransform)
        {
            canvas.transform.LookAt(canvas.transform.position + cameraTransform.forward);
        }
        //Testing take damage function
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }*/
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
    }
}
