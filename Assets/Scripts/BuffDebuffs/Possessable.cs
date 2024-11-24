using UnityEngine;
using StarterAssets;

public class Possessable : MonoBehaviour
{
    [Header("Possession Settings")]
    public float moveSpeed = 4.0f;
    public float rotationSpeed = 1.0f;
    
    [Header("First Person Settings")]
    public Transform fpsCameraPoint;
    private float _cinemachineTargetPitch;
    
    [Header("Movement")]
    public float gravity = -15.0f;
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.5f;
    public LayerMask groundLayers;
    
    private CharacterController characterController;
    private bool isGrounded;
    private float verticalVelocity;

    private void Awake()
    {
        if (fpsCameraPoint == null)
        {
            GameObject cameraPointObj = new GameObject("FPSCameraPoint");
            cameraPointObj.transform.SetParent(transform);
            cameraPointObj.transform.localPosition = new Vector3(0, 1.8f, 0);
            fpsCameraPoint = cameraPointObj.transform;
        }

        characterController = gameObject.AddComponent<CharacterController>();
        characterController.height = 2f;
        characterController.radius = 0.5f;

        // Disable any existing Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
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
        // Handle looking up/down (pitch) - this only affects the camera point
        _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch - (lookInput.y * rotationSpeed), -89f, 89f);
        fpsCameraPoint.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0, 0);
        
        // Handle looking left/right (yaw) - this rotates the entire object
        transform.Rotate(Vector3.up * (lookInput.x * rotationSpeed));

        // Ground check and gravity
        GroundedCheck();
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Get camera's forward and right vectors, but ignore vertical component
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        // Project vectors onto the horizontal plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to camera orientation
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);
        
        // Apply movement and gravity
        Vector3 movement = moveDirection.normalized * moveSpeed;
        movement.y = verticalVelocity;
        
        characterController.Move(movement * Time.deltaTime);
    }

    void OnDisable()
    {
        if (characterController != null)
        {
            Destroy(characterController);
        }
    }
}