using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class InteractableObject : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] protected float interactionDistance = 2f;
    [SerializeField] protected string promptText = "Press E to interact";
    
    [Header("UI References")]
    [SerializeField] protected GameObject interactionPrompt;
    [SerializeField] protected TextMeshProUGUI promptTextUI;
    
    protected bool playerInRange = false;
    protected Transform playerTransform;
    protected Camera playerCamera;

    protected virtual void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        playerCamera = Camera.main;
        if (promptTextUI) promptTextUI.text = promptText;
        if (interactionPrompt) interactionPrompt.SetActive(false);
    }

    protected virtual void Update()
    {
        CheckInteraction();
        
        if (playerInRange && IsLookingAtObject() && Input.GetKeyDown(KeyCode.E))
        {
            OnInteract();
        }
    }

    protected void CheckInteraction()
    {
        // First check if player is in range
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= interactionDistance;

        // Then check if player is looking at the object
        bool isLookingAt = IsLookingAtObject();

        // Update prompt visibility
        if (interactionPrompt)
        {
            interactionPrompt.SetActive(playerInRange && isLookingAt);
        }
    }

    protected bool IsLookingAtObject()
    {
        if (!playerCamera || !playerInRange) return false;

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            // Check if the ray hit this object
            return hit.collider.gameObject == this.gameObject;
        }

        return false;
    }

    protected void CheckPlayerDistance()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionDistance;

        if (wasInRange != playerInRange)
        {
            if (interactionPrompt) interactionPrompt.SetActive(playerInRange);
        }
    }
      protected virtual void OnDrawGizmosSelected()
    {
        // Draw interaction radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }

    protected virtual void OnInteract()
    {
        // Override in derived classes
    }
}