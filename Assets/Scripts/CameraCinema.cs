using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCinema : MonoBehaviour
{
    Animator animator;
    private bool rotating;
    public AssassinCinematic assassinCinematic;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (rotating)
            {
                animator.SetBool("Rotate", false);
                rotating = false;
            }
            else
            {
                animator.SetBool("Rotate", true);
                rotating = true;
            }   
        }
    }

    public void ToggleADS()
    {
        assassinCinematic.ToggleAimAnim();
    }
}
