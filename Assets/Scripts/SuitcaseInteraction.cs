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

    [Header("UI")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isUnlocked = false;
    private bool isShowingMessage = false;

    private void Start()
    {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
            
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    protected override void Update()
    {
        if (!isShowingMessage) {
            base.Update();
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            HideMessage();
        }
    }

    protected override void OnInteract()
    {
        if (!isUnlocked) {
            if (InventoryManager.Instance.HasItem(requiredItemID)) {
                UnlockSuitcase();
            }
            else {
                ShowMessage("A sturdy metal suitcase with a complex locking mechanism");
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

        StartCoroutine(UnlockSequence());
    }

    private IEnumerator UnlockSequence()
    {
        // First message
        ShowMessage("The key fits perfectly. The lock's tumblers click into place...");
        
        // Wait for player to dismiss
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideMessage();
        
        yield return new WaitForSeconds(1f);
        
        // Second message
        ShowMessage("Beneath layers of protective padding, you discover an electronic component");
        InventoryManager.Instance.RemoveItem(requiredItemID);
        InventoryManager.Instance.AddItem("5", "ElectricalComponent", electronicComponentSprite);
        
        // Wait for player to dismiss
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideMessage();
        
        // Disable further interaction
        this.enabled = false;
    }

    private void ShowMessage(string message)
    {
        isShowingMessage = true;
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;

        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);
        }
    }

    private void HideMessage()
    {
        isShowingMessage = false;
        
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;

        if (messagePanel != null)
            messagePanel.SetActive(false);
            
        // Only show interaction prompt if not unlocked
        if (!isUnlocked && interactionPrompt != null)
            interactionPrompt.SetActive(playerInRange);
    }

    private void OnDisable()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (messagePanel != null)
            messagePanel.SetActive(false);
            
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }
}