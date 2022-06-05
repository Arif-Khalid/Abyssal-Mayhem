using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    [SerializeField] float speed = 12f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 2f;

    Vector3 velocity;
    Vector3 move;
    bool isGrounded;
    bool isMovementDisabled = false;

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
        VerticalMovement();
        if (!isMovementDisabled)
        {
            controller.Move(move * Time.deltaTime);
        }    
    }

    //Movement horizontally locally
    private void HorizontalMovement()
    {
        isGrounded = controller.isGrounded;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        move = transform.right * x + transform.forward * z; //move locally
        move *= speed;
    }
    //Movement vertically with gravity and jumping
    private void VerticalMovement()
    {
        if (isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
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
