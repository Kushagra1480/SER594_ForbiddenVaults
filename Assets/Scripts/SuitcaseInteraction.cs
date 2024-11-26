using UnityEngine;
using TMPro;
using StarterAssets;
using System.Collections;

public class SuitcaseInteraction : InteractableObject
{
    [Header("Settings")]
    [SerializeField] private string requiredItemID = "Key";
    [SerializeField] private AudioSource unlockSound;
    [SerializeField] private Animator suitcaseAnimator;

    [SerializeField] private Sprite electronicComponentSprite;

    [Header("UI - Unlock Message")]
    [SerializeField] private GameObject unlockMessagePanel;
    [SerializeField] private TextMeshProUGUI unlockMessageText;

    [Header("UI - Item Message")]
    [SerializeField] private GameObject itemMessagePanel;
    [SerializeField] private TextMeshProUGUI itemMessageText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isUnlocked = false;
    private bool isShowingUnlockMessage = false;
    private bool isShowingItemMessage = false;

    private void Start()
    {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
            
        // Hide message panels at start
        if (unlockMessagePanel != null)
            unlockMessagePanel.SetActive(false);
        if (itemMessagePanel != null)
            itemMessagePanel.SetActive(false);
    }

    protected override void Update()
    {
        // Only check for base interactions if no messages are showing
        if (!isShowingUnlockMessage && !isShowingItemMessage) {
            base.Update();
        }
        // Check for message dismissal
        if (Input.GetKeyDown(KeyCode.E)) {
            if (isShowingUnlockMessage) {
                HideUnlockMessage();
            }
            else if (isShowingItemMessage) {
                HideItemMessage();
            }
        }
    }

    protected override void OnInteract()
    {
        if (!isUnlocked) {
            if (InventoryManager.Instance.HasItem(requiredItemID))
            {
                UnlockSuitcase();
            }
            else
            {
                ShowUnlockMessage("The key fits perfectly. The lock's tumblers click into place...");
            }
        }
    }

    private void UnlockSuitcase()
    {
        isUnlocked = true;
        
        if (unlockSound != null)
            unlockSound.Play();
            
        if (suitcaseAnimator != null)
            suitcaseAnimator.SetTrigger("Open");

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        StartCoroutine(UnlockSequence());
    }

    private IEnumerator UnlockSequence()
    {
        // Ensure interaction prompt is hidden
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // First message
        ShowUnlockMessage("The key fits perfectly. The lock's tumblers click into place...");
        
        // Wait for player to dismiss unlock message
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideUnlockMessage();
        
        // Small delay between messages
        yield return new WaitForSeconds(2f);
        
        // Second message
        ShowItemMessage("Beneath layers of protective padding, you discover an electronic component");
        InventoryManager.Instance.RemoveItem(requiredItemID);
        InventoryManager.Instance.AddItem("5", "ElectricalComponent", electronicComponentSprite);
        
        // Wait for player to dismiss item message
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideItemMessage();
        
        // Disable further interaction
        this.enabled = false;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void ShowUnlockMessage(string message)
    {
        isShowingUnlockMessage = true;
        DisablePlayerControls();
        
        // Hide interaction prompt when showing any message
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (unlockMessagePanel != null && unlockMessageText != null)
        {
            unlockMessageText.text = message;
            unlockMessagePanel.SetActive(true);
        }
    }

    private void ShowItemMessage(string message)
    {
        isShowingItemMessage = true;
        DisablePlayerControls();
        
        // Hide interaction prompt when showing any message
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (itemMessagePanel != null && itemMessageText != null)
        {
            itemMessageText.text = message;
            itemMessagePanel.SetActive(true);
        }
    }

    private void HideUnlockMessage()
    {
        isShowingUnlockMessage = false;
        if (!isShowingItemMessage) 
        {
            EnablePlayerControls();
            // Only show interaction prompt if we're not unlocked yet
            if (!isUnlocked && interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }

        if (unlockMessagePanel != null)
            unlockMessagePanel.SetActive(false);
    }

    private void HideItemMessage()
    {
        isShowingItemMessage = false;
        if (!isShowingUnlockMessage) 
        {
            EnablePlayerControls();
            // Don't re-enable interaction prompt after item message
        }

        if (itemMessagePanel != null)
            itemMessagePanel.SetActive(false);
    }

    private void DisablePlayerControls()
    {
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void EnablePlayerControls()
    {
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }

    private void OnDisable()
    {
        // Ensure everything is cleaned up when disabled
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (unlockMessagePanel != null)
            unlockMessagePanel.SetActive(false);
        if (itemMessagePanel != null)
            itemMessagePanel.SetActive(false);
            
        EnablePlayerControls();
    }
}