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

    Vector3 velocity;
    Vector3 move;
    bool isGrounded;
    bool isMovementDisabled = false;

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        VerticalMovement();
        controller.Move(move * Time.deltaTime);
        weaponSlotAnimator.SetFloat("WalkSpeed", controller.velocity.magnitude / speed ); //Set speed of walking animation
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
}
