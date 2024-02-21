using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPickup : Interactable
{
    public ChestContent chestContent;
    public enum powerupName { invincibility, juggernautSpawn, paranoia, extraLife}
    public PowerupPickup.powerupName pickupName;
    protected override void Interact()
    {
        AudioManager.instance.Play("PowerupPickup");
        PlayerPowerups playerPowerups = PlayerManager.localPlayerSetup.GetComponent<PlayerPowerups>();
        if (pickupName == powerupName.invincibility)
        {
            playerPowerups.StartCoroutine(nameof(playerPowerups.ActivateInvincibility));
        }
        else if (pickupName == powerupName.juggernautSpawn)
        {
            PlayerManager.localPlayerSetup.CmdJuggernautSpawn();
        }
        else if (pickupName == powerupName.paranoia)
        {
            PlayerManager.localPlayerSetup.CmdParanoia();
        }
        else if (pickupName == powerupName.extraLife)
        {
            playerPowerups.ActivateExtraLife();
        }
        chestContent.MakeAvailable();
        gameObject.SetActive(false);
    }
}
