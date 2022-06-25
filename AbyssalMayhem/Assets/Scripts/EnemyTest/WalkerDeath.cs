using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkerDeath : MonoBehaviour
{
    Animator animator;
    [SerializeField] Behaviour[] behavioursToDisable;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DeathAnim()
    {
        foreach(Behaviour behaviour in behavioursToDisable)
        {
            behaviour.enabled = false;
        }
        animator.Play("MushroomDie");
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }

}
