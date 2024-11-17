using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    public float targetAngleX1; // First target X-axis angle for success
    public float targetAngleX2; // Second target X-axis angle for success
    public bool isCorrectAngle = false; // Tracks if the puzzle piece is correctly oriented
    private Quaternion targetRotation1; // The first target rotation as a quaternion
    private Quaternion targetRotation2; // The second target rotation as a quaternion

    private void Start()
    {
        // Calculate the target rotations based on the target angles
        targetRotation1 = Quaternion.Euler(targetAngleX1, transform.eulerAngles.y, transform.eulerAngles.z);
        targetRotation2 = Quaternion.Euler(targetAngleX2, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void Update()
    {
        // Continuously check the angle in the Update loop
        CheckAngle();

        // Check for mouse clicks and handle rotation
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        // Cast a ray from the camera to detect the clicked object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            RotateObject();
        }
    }

    private void RotateObject()
    {
        // Get the current rotation
        Quaternion currentRotation = transform.rotation;

        // Add 90 degrees to the Y-axis (if rotating along the Y-axis)
        Quaternion newRotation = currentRotation * Quaternion.Euler(0, 90, 0);

        // Apply the new rotation
        transform.rotation = newRotation;

        // After the rotation, check if the piece is correctly aligned
        CheckAngle();
    }

    private void CheckAngle()
    {
        // Check if the current rotation matches either of the target rotations
        bool isAlignedWithTarget1 = Quaternion.Angle(transform.rotation, targetRotation1) < 1f; // 1 degree tolerance
        bool isAlignedWithTarget2 = Quaternion.Angle(transform.rotation, targetRotation2) < 1f; // 1 degree tolerance

        // If aligned with either target, set isCorrectAngle to true
        isCorrectAngle = isAlignedWithTarget1 || isAlignedWithTarget2;

        if (isCorrectAngle)
        {
            Debug.Log($"{gameObject.name} is correctly aligned!");
        }
    }
}
