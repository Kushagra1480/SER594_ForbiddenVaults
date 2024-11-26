using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollectibleItem : InteractableObject
{
    [Header("Item Settings")]
    [SerializeField] private string itemID;
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    
    [Header("UI References")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private TextMeshProUGUI confirmationText;
    [SerializeField] private CharacterController playerController;
    private bool isConfirmationShowing = false;

    private void Start()
    {
        base.Start();
        
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
        if (playerController == null) {
            playerController = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<CharacterController>();
        }
    }

    protected override void Update()
    {
        if (!isConfirmationShowing)
        {
            base.Update();
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

    private void ShowConfirmation()
    {
        isConfirmationShowing = true;
        confirmationPanel.SetActive(true);
        confirmationText.text = $"Pick up {itemName}?";
        
        if (playerController != null)
            playerController.enabled = false;
            
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HandleConfirmation()
    {
        if (Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.Return))
        {
            CollectItem();
        }
        else if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Escape))
        {
            CancelCollection();
        }
    }

    private void CollectItem() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        InventoryManager.Instance.AddItem(itemID, itemName, itemIcon);
        Debug.Log($"Adding item - ID: {itemID}, Name: {itemName}, Icon: {itemIcon}");
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
            
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (isConfirmationShowing)
        {
            CloseConfirmation();
        }
    }
}