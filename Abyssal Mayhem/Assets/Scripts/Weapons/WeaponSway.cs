using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
   [Header("Sway Settings")]
   [SerializeField] private float smooth;
   [SerializeField] private float swayMultiplier;

   private void Update()
   {
       // Obtain mouse inputs
       float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
       float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

       // Calculating Target Rotation
       Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
       Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

       Quaternion targetRotation = rotationX * rotationY;

       // Implement the rotation
       transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
   }
}
