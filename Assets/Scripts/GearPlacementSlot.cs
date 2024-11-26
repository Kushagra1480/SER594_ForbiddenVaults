using UnityEngine;
using TMPro;
using StarterAssets;

public class GearPlacementSpot : InteractableObject
{
    [Header("Gear Settings")]
    [SerializeField] private GameObject gearVisualPrefab;
    [SerializeField] private GameObject gearModel;
    [SerializeField] private string requiredGearID = "2";
    [SerializeField] private float rotationSpeed = 30f;

    [Header("UI References")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;
    
    private GameObject placedGear;
    public bool IsGearPlaced => isGearPlaced;
    private bool isGearPlaced = false;
    private bool isShowingMessage = false;

    private GameObject placedGearContainer;  
    private Transform gearToRotate;          

    
    // This will be accessed by the lever to check if gear is placed

    private void Start() {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
        if (gearModel != null)
            gearModel.SetActive(false);
    }

    protected override void Update() {
        if (!isShowingMessage) {
            base.Update();
            if (placedGearContainer != null)
            {
                placedGearContainer.transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
            }
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            HideMessage();
        }
    }

    protected override void OnInteract() {
        if (!isGearPlaced) {
            if (InventoryManager.Instance.HasItem(requiredGearID)) {
                PlaceGear();
            }
            else {
                ShowMessage("A gear could fit in here");
            }
        }
    }

     private void PlaceGear() {
        InventoryManager.Instance.RemoveItem(requiredGearID);
        
        // Show the pre-placed gear
        if (gearModel != null)
            gearModel.SetActive(true);
        
        isGearPlaced = true;
        ShowMessage("You slot the gear in the mechanism");
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    private void ShowMessage(string message)
    {
        isShowingMessage = true;

        // Disable player controls
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;

        // Hide interaction prompt while showing message
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

        // Only show interaction prompt again if gear isn't placed
        if (!isGearPlaced && interactionPrompt != null)
            interactionPrompt.SetActive(playerInRange);
        
        // If gear was placed, disable further interaction
        if (isGearPlaced)
            this.enabled = false;
    }
    private void OnDisable()
    {
        if (isShowingMessage)
        {
            HideMessage();
        }
    }

}