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
    private Collider originalCollider;
    private Vector3 originalCenter;
    private float originalHeight;
    private float originalRadius;

    private void Awake()
    {
        SetupCameraPoint();
        SetupCharacterController();
    }

    private void SetupCameraPoint()
    {
        if (fpsCameraPoint == null)
        {
            GameObject cameraPointObj = new GameObject("FPSCameraPoint");
            cameraPointObj.transform.SetParent(transform);
            cameraPointObj.transform.localPosition = new Vector3(0, 1.8f, 0);
            fpsCameraPoint = cameraPointObj.transform;
        }
    }

    private void SetupCharacterController()
    {
        // Get the original collider (could be any type)
        originalCollider = GetComponent<Collider>();
        /*if (originalCollider != null)
        {
            // Store original collider properties
            Bounds bounds = originalCollider.bounds;
            originalCenter = originalCollider.bounds.center - transform.position;
            originalHeight = bounds.size.y;
            originalRadius = Mathf.Max(bounds.extents.x, bounds.extents.z);

            // Add CharacterController with dimensions based on the original collider
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = originalHeight;
            characterController.radius = originalRadius;
            characterController.center = originalCenter;
            
            // Disable the original collider while possessed
            //originalCollider.enabled = false;
        }*/
        //else
        {
            // If no collider exists, create a CharacterController with default dimensions
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.radius = 0.5f;
            characterController.center = Vector3.up;
            Debug.LogWarning($"No collider found on {gameObject.name}. Using default CharacterController dimensions.");
        }

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
        if (characterController == null) return;

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

        // Calculate movement direction relative to camera orientation
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x);
        
        // Apply movement and gravity
        Vector3 movement = moveDirection.normalized * moveSpeed;
        movement.y = verticalVelocity;
        
        characterController.Move(movement * Time.deltaTime);
    }

    void OnDisable()
    {
        // Restore original collider when possession ends
        if(originalCollider != null)
        {
            originalCollider.enabled = true;
        }
        
        if (characterController != null)
        {
            Destroy(characterController);
        }
    }
}