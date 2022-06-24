using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;

    float xRotation = 0f; //Store up/down rotation

    private bool mouseLocked = true;
    private bool stopRotation = false;
    // Start is called before the first frame update
    void Start()
    {
        LockMouse();
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseLocked && !stopRotation)
        {
            XRotate();
            YRotate();
        }
    }

    //Look up/down by rotating along local x axis
    private void XRotate()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Clamp looking up and down to 90 degrees
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Look up/down
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    //Look left/right by rotating along y axis
    private void YRotate()
    {
        //Get inputs from mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        XRotate();
        //Look right/left
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mouseLocked = true;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        mouseLocked = false;
    }

    //Stops and enables rotation of player on mouse movement
    //Called when player dies and when play again is pressed
    public void StopRotation()
    {
        stopRotation = true;
    }

    public void EnableRotation()
    {
        stopRotation = false;
    }
}   
