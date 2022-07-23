using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] Image backgroundImage;
    [SerializeField] Color activatedColor;
    [SerializeField] Color deactivatedColor;

    public void ActivateWeaponUI()
    {
        backgroundImage.color = activatedColor;
    }

    public void DeactivateWeaponUI()
    {
        backgroundImage.color = deactivatedColor;
    }
}
