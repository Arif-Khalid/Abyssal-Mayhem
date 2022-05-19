using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private Transform cameraTransform;

    float xRotation = 0f; //Store up/down rotation
    // Start is called before the first frame update
    void Start()
    {
        //Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Get inputs from mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Clamp looking up and down to 90 degrees
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Look up/down
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //Look right/left
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
