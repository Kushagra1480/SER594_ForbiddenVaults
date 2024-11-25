using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller; // Reference to the CharacterController component
    public float speed = 12f; // Movement speed
    public float jumpHeight = 3f; // Jump height
    public float gravity = -9.81f; // Gravity force

    private Vector3 velocity; // Stores the player's current velocity
    private bool isGrounded; // Checks if the player is grounded

    public Transform groundCheck; // Reference to the ground check position
    public float groundDistance = 0.4f; // Radius for ground check
    public LayerMask groundMask; // Layer mask to define what counts as ground

    void Update()
    {
        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset velocity when grounded
        }

        // Get input for movement
        float x = Input.GetAxis("Horizontal"); // Left/right movement
        float z = Input.GetAxis("Vertical"); // Forward/backward movement

        // Calculate movement direction based on camera's rotation
        Vector3 move = transform.right * x + transform.forward * z;

        // Apply movement
        controller.Move(move * speed * Time.deltaTime);

        // Handle jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
