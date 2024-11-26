using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class LeverInteraction : InteractableObject
{
    [Header("Dependencies")]
    [SerializeField] private GearPlacementSpot gearSpot;
    [SerializeField] private Transform[] gearsToSpin;
    [SerializeField] private ProtectiveBars protectiveBars;  // Add this reference
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    [Header("Audio")]
    [SerializeField] private AudioSource gearSpinningSound;

    private bool isActivated = false;
    private bool isShowingMessage = false;

    private void Start()
    {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    protected override void Update()
    {
        if (!isShowingMessage)
        {
            base.Update();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            HideMessage();
        }
    }

    protected override void OnInteract()
    {
        if (!isActivated && !isShowingMessage)
        {
            if (gearSpot != null && gearSpot.IsGearPlaced)
            {
                ActivateLever();
            }
            else
            {
                ShowMessage("Nothing happened");
            }
        }
    }

    private void ActivateLever()
    {
        isActivated = true;
        ShowMessage("You hear metal bars move");
        
        // Start gear spinning
        if (gearSpinningSound != null)
        {
            gearSpinningSound.Play();
        }

        StartCoroutine(SpinGearsCoroutine());

        // Lower the bars
        if (protectiveBars != null)
        {
            protectiveBars.LowerBars();
        }
        
        // Disable further interaction with the lever
        this.enabled = false;
    }

    private void ShowMessage(string message)
    {
        isShowingMessage = true;

        // Disable player controls
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;

        // Hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        // Show message
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackText.text = message;
            feedbackPanel.SetActive(true);
        }
    }

    private void HideMessage()
    {
        isShowingMessage = false;

        // Re-enable player controls
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;

        // Hide message panel
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        // If not activated, show interaction prompt again
        if (!isActivated && interactionPrompt != null)
            interactionPrompt.SetActive(playerInRange);
    }

    private IEnumerator SpinGearsCoroutine() {
        while (isActivated)
{
            for (int i = 0; i < gearsToSpin.Length; i++)
            {
                Transform gear = gearsToSpin[i];
                if (gear != null)
                {
                    // Reverse rotation for the last gear
                    float directionMultiplier = (i == gearsToSpin.Length - 1) ? -1f : 1f;
                    gear.Rotate(0f, rotationSpeed * Time.deltaTime * directionMultiplier, 0f);
                }
            }
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (isShowingMessage)
        {
            HideMessage();
        }
    }
}