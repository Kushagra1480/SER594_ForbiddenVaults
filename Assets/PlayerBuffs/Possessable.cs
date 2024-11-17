using UnityEngine;
using StarterAssets;

public class Possessable : MonoBehaviour
{
    [Header("Possession Settings")]
    public float moveSpeed = 4.0f;
    public float rotationSpeed = 0.5f;
    
    [Header("First Person Settings")]
    public Transform fpsCameraPoint;
    private float _cinemachineTargetPitch;
    
    [Header("Movement")]
    public float gravity = -15.0f;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.5f;
    public LayerMask groundLayers;
    
    private Rigidbody rb;
    private bool isGrounded;
    private float verticalVelocity;
    private CharacterController characterController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Add CharacterController if it doesn't exist
        characterController = gameObject.AddComponent<CharacterController>();
        characterController.height = 2f;
        characterController.radius = 0.5f;
        
        if (fpsCameraPoint == null)
        {
            GameObject cameraPointObj = new GameObject("FPSCameraPoint");
            cameraPointObj.transform.SetParent(transform);
            cameraPointObj.transform.localPosition = new Vector3(0, 0.8f, 0);
            fpsCameraPoint = cameraPointObj.transform;
        }

        // Disable Rigidbody and use CharacterController instead
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    public void HandleMovement(Vector2 moveInput, Vector2 lookInput)
    {
        GroundedCheck();

        // Camera rotation
        _cinemachineTargetPitch += lookInput.y * rotationSpeed;
        _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, -90f, 90f);
        fpsCameraPoint.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0, 0);
        
        // Object rotation
        transform.Rotate(Vector3.up * lookInput.x * rotationSpeed);

        // Calculate movement direction
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Handle gravity
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Apply movement using CharacterController
        Vector3 movement = moveDirection.normalized * moveSpeed;
        movement.y = verticalVelocity;
        
        characterController.Move(movement * Time.deltaTime);
    }

    void OnDisable()
    {
        // Clean up when possession ends
        if (characterController != null)
        {
            Destroy(characterController);
        }
    }
}