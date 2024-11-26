using UnityEngine;
using TMPro;
using StarterAssets;

public class GlassPanel : InteractableObject
{
    [Header("Dependencies")]
    [SerializeField] private string requiredItemID = "Hammer";
    [SerializeField] private GameObject glassObject;
    [SerializeField] private AudioSource glassBreakSound;
    [SerializeField] private ProtectiveBars protectiveBars; // Add reference to bars

    [Header("UI")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isBroken = false;
    private bool isShowingMessage = false;
    private bool isShowingConfirmation = false;

    private void Start()
    {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    protected override void Update() {
        bool barsAreLowered = protectiveBars != null && protectiveBars.IsLowered;
        
        if (!isShowingMessage && !isShowingConfirmation && barsAreLowered) {
            base.Update();
        }
        else if (isShowingConfirmation) {
            HandleConfirmation();
        }
        else if (isShowingMessage && Input.GetKeyDown(KeyCode.E)) {
            HideMessage();
        }

        // Override the interaction prompt visibility if bars aren't lowered
        if (interactionPrompt != null && !barsAreLowered)
        {
            interactionPrompt.SetActive(false);
        }
    }

    protected override void OnInteract()
    {
        // Double check bars are lowered before allowing interaction
        if (!protectiveBars.IsLowered)
            return;

        if (!isBroken)
        {
            if (InventoryManager.Instance.HasItem(requiredItemID))
            {
                ShowConfirmation("Break this glass?");
            }
            else
            {
                ShowMessage("This glass looks like it could be broken");
            }
        }
    }

    private void HandleConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            BreakGlass();
            HideConfirmation();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            HideConfirmation();
        }
    }

    private void BreakGlass()
    {
        isBroken = true;
        
        if (glassBreakSound != null)
            glassBreakSound.Play();

        if (glassObject != null)
            glassObject.SetActive(false);

        // Disable further interaction
        this.enabled = false;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }

    private void ShowMessage(string message)
    {
        isShowingMessage = true;
        DisablePlayerControls();

        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackText.text = message;
            feedbackPanel.SetActive(true);
        }
    }

    private void ShowConfirmation(string message)
    {
        isShowingConfirmation = true;
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        DisablePlayerControls();

        if (confirmationPanel != null && confirmationText != null)
        {
            confirmationText.text = message;
            confirmationPanel.SetActive(true);
        }
    }

    private void HideMessage()
    {
        isShowingMessage = false;
        EnablePlayerControls();

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    private void HideConfirmation()
    {
        isShowingConfirmation = false;
        EnablePlayerControls();

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
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

    public bool IsBroken => isBroken;
}