using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [SerializeField] float speed = 12f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] Animator weaponSlotAnimator;
    [SerializeField] float prohibitionRadius;
    [SerializeField] Transform playerFeet;
    [SerializeField] LayerMask whatIsProhibited;
    [SerializeField] PlayerUI playerUI;

    bool prohibitionStarted = false;
    Vector3 velocity;
    Vector3 move;
    bool isGrounded;
    bool isMovementDisabled = false;
    [SerializeField]float mass = 3.0f;
    Vector3 impact = Vector3.zero;


    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        VerticalMovement();
        if (impact.magnitude > 0.2) move += impact; //apply the impact force
        controller.Move(move * Time.deltaTime);
        weaponSlotAnimator.SetFloat("WalkSpeed", controller.velocity.magnitude / speed ); //Set speed of walking animation
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime); //consume the impact energy each cycle
        CheckProhibition();
    }

    //Movement horizontally locally
    private void HorizontalMovement()
    {
        isGrounded = controller.isGrounded;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (isMovementDisabled) { move = Vector3.zero; }
        else
        {
            move = transform.right * x + transform.forward * z; //move locally
            move *= speed;
        }      
    }
    //Movement vertically with gravity and jumping
    //Animator for FPS weapon movements
    private void VerticalMovement()
    {
        if (isGrounded)
        {
            weaponSlotAnimator.SetBool("isGrounded", true);
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump") && !isMovementDisabled)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                float currentAnimVelocity = weaponSlotAnimator.GetFloat("VelocityY");
                //Prevent snapping by not allowing an increase of more than 2 in animator velocity
                if (velocity.y - currentAnimVelocity > 2)
                {
                    weaponSlotAnimator.SetFloat("VelocityY", currentAnimVelocity + 2); //Set velocity for animations
                }
                else
                {
                    weaponSlotAnimator.SetFloat("VelocityY", velocity.y); //Set velocity for animation
                }              
                weaponSlotAnimator.SetBool("isGrounded", false); //Start rising and falling blendtree
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            float currentAnimVelocity = weaponSlotAnimator.GetFloat("VelocityY");
            //Prevent snapping
            if (velocity.y - currentAnimVelocity > 2)
            {
                weaponSlotAnimator.SetFloat("VelocityY", currentAnimVelocity + 2);
            }
            else
            {
                weaponSlotAnimator.SetFloat("VelocityY", velocity.y); //Set velocity for animations
            }
            weaponSlotAnimator.SetBool("isGrounded", false); //Return to walking animations
        }

        move += velocity;
    }

    public void EnableMovement()
    {
        isMovementDisabled = false;
    }
    public void DisableMovement()
    {
        isMovementDisabled = true;
    }

    //Call this function to add impact force
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; //reflect down force on ground
        impact += dir.normalized * force / mass;
    }

    private void CheckProhibition()
    {
        if(Physics.CheckSphere(playerFeet.position, prohibitionRadius, whatIsProhibited))
        {
            if (!prohibitionStarted)
            {
                prohibitionStarted = true;
                playerUI.StartProhibitionTimer();
            }
        }
        else
        {
            if (prohibitionStarted && controller.isGrounded)
            {
                prohibitionStarted = false;
                playerUI.StopProhibitionTimer();
            }
        }
    }

    private void OnDisable()
    {
        move = Vector3.zero;
        impact = Vector3.zero;    
    }
}
