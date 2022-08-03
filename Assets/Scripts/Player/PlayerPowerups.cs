using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    //References
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] PlayerWeapon playerWeapon;
    [SerializeField] PlayerSetup playerSetup;
    [SerializeField] PlayerUI playerUI;


    //Invincibility variables
    [SerializeField] float invincibileTime;
    [SerializeField] float invincibilityFireRate;

    //Juggernaut spawn variables
    [SerializeField] float timeTillSpawn;
    [SerializeField] string juggernautWarningMessage;
    [SerializeField] int numberOfJuggernauts;

    //Paranoia variables
    [SerializeField] float timeTillBlind;
    [SerializeField] string paranoiaWarningMessage;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ActivateExtraLife();
        }
    }
    //Invincibility
    public IEnumerator ActivateInvincibility()
    {
        AudioManager.instance.Play("Invincibility");
        playerHealth.invincible = true;
        playerWeapon.ChangeFireRate(invincibilityFireRate);
        playerSetup.enemySpawner.EnableOutline();
        yield return new WaitForSeconds(invincibileTime);
        playerHealth.invincible = false;
        playerWeapon.ChangeFireRate(1f);
        playerSetup.enemySpawner.DisableOutline();
        AudioManager.instance.Stop("Invincibility");
    }
    
    //Juggernaut Spawner
    public IEnumerator ActivateJuggernautSpawn()
    {
        playerUI.StartWarning(juggernautWarningMessage);
        yield return new WaitForSeconds(timeTillSpawn); //should be greater than 3 since warning animation plays for 3 seconds
        for(int i = 0; i < numberOfJuggernauts; i++) //spawns that number of juggernauts
        {
            playerSetup.enemySpawner.SpawnMonsterAtPoint(Vector3.zero, EnemySpawner.MonsterID.juggernaut);
        }       
    }

    //Paranoia
    public IEnumerator ActivateParanoia()
    {
        playerUI.StartWarning(paranoiaWarningMessage);
        yield return new WaitForSeconds(timeTillBlind); //should be greater than 3 since warning animation plays for 3 seconds
        playerUI.StartBlind();//UI for paranoia
    }

    //Extra life
    public void ActivateExtraLife()
    {
        playerHealth.AddLife();
    }

    public void ResetAppliedPowerups()
    {
        StopAllCoroutines();
        playerHealth.invincible = false;
        playerHealth.ResetLives();
        playerWeapon.ChangeFireRate(1f);
        playerSetup.enemySpawner.DisableOutline();
        AudioManager.instance.Stop("Invincibility");
        playerUI.StopAnimator();
    }
}
