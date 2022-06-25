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
        PlayerPowerups playerPowerups = PlayerSetup.localPlayerSetup.GetComponent<PlayerPowerups>();
        if (pickupName == powerupName.invincibility)
        {
            playerPowerups.StartCoroutine(nameof(playerPowerups.ActivateInvincibility));
        }
        else if (pickupName == powerupName.juggernautSpawn)
        {
            PlayerSetup.localPlayerSetup.CmdJuggernautSpawn();
        }
        else if (pickupName == powerupName.paranoia)
        {
            PlayerSetup.localPlayerSetup.CmdParanoia();
        }
        else if (pickupName == powerupName.extraLife)
        {
            playerPowerups.ActivateExtraLife();
        }
        chestContent.MakeAvailable();
        Destroy(transform.parent.gameObject);
    }
}
