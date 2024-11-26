using UnityEngine;
using TMPro;
using StarterAssets;

public class KeyCollectible : InteractableObject
{
    [Header("Item Settings")]
    [SerializeField] private string itemID = "Key";
    [SerializeField] private string itemName = "Key";
    [SerializeField] private Sprite itemIcon;
    
    [Header("Dependencies")]
    [SerializeField] private GlassPanel glassPanel;
    
    [Header("UI")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isConfirmationShowing = false;

    private void Start()
    {
        base.Start();
        
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    protected override void Update()
    {
        if (!isConfirmationShowing)
        {
            // Only allow interaction if glass is broken
            if (glassPanel != null && glassPanel.IsBroken)
            {
                base.Update();
            }
        }
        else
        {
            HandleConfirmation();
        }
    }

    protected override void OnInteract()
    {
        ShowConfirmation();
    }

    private void ShowConfirmation() {
        interactionPrompt.SetActive(false);
        isConfirmationShowing = true;
        confirmationPanel.SetActive(true);
        confirmationText.text = $"Pick up {itemName}?";
        
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void HandleConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            CollectItem();
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            CancelCollection();
        }
    }

    private void CollectItem() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        InventoryManager.Instance.AddItem(itemID, itemName, itemIcon);
        CloseConfirmation();
        gameObject.SetActive(false);
    }

    private void CancelCollection()
    {
        CloseConfirmation();
    }

    private void CloseConfirmation()
    {
        isConfirmationShowing = false;
        confirmationPanel.SetActive(false);
        
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }
    private void OnDisable() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (isConfirmationShowing)
        {
            CloseConfirmation();
        }
    }
    private void OnDestroy() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}