using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinCinematic : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] LineRenderer laserSight;
    public Transform laserOrigin;
    public float animSpeed;
    public bool ads = false;

    private void Update()
    {
        laserSight.SetPosition(0, laserOrigin.position);
        laserSight.SetPosition(1, laserOrigin.position + laserOrigin.forward * 100);
    }
    public void ToggleAimAnim()
    {
        if (ads)
        {
            BackToIdle();
        }
        else
        {
            ADS();
        }
    }
    public void ADS()
    {
        ads = true;
        animator.SetFloat("Speed", animSpeed);
    }

    public void BackToIdle()
    {
        ads = false;
        animator.SetFloat("Speed", -animSpeed);
    }
 }
