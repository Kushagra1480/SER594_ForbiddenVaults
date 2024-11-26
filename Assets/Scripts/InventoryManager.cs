using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using StarterAssets;


// Updated Inventory Manager
public class InventoryManager : MonoBehaviour {
    public static InventoryManager Instance { get; private set; }
    
    [Header("UI Elements")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform itemsContainer; // Parent for item slots
    [SerializeField] private GameObject itemSlotPrefab;

    [Header("Settings")]
    [SerializeField] private int maxInventorySlots = 10;
    [SerializeField] private FirstPersonController fpsController;
    
    
    private Dictionary<string, InventoryItem> inventory = new Dictionary<string, InventoryItem>();
    private bool isInventoryOpen = false;
    private CharacterController playerController;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (inventoryPanel == null)
            Debug.LogError("Inventory Panel not assigned to InventoryManager!");
        if (itemsContainer == null)
            Debug.LogError("Items Container not assigned to InventoryManager!");
        if (itemSlotPrefab == null)
            Debug.LogError("Item Slot Prefab not assigned to InventoryManager!");

        inventoryPanel.SetActive(false);
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);
        
        if (playerController != null) {
            playerController.enabled = !isInventoryOpen;
        }
        if (fpsController != null) {
            fpsController.enabled = !isInventoryOpen;
            fpsController.SetRotation(isInventoryOpen);
        }
        
        if (isInventoryOpen)
        {
            RefreshInventoryUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void AddItem(string itemID, string itemName, Sprite itemIcon)
    {
        if (inventory.Count >= maxInventorySlots) return;

        if (!inventory.ContainsKey(itemID))
        {
            inventory.Add(itemID, new InventoryItem(itemName, itemIcon));
            RefreshInventoryUI();
        }
    }

    public bool HasItem(string itemID)
    {
        return inventory.ContainsKey(itemID);
    }

    public void RemoveItem(string itemID)
    {
        if (inventory.ContainsKey(itemID))
        {
            inventory.Remove(itemID);
            RefreshInventoryUI();
        }
    }

    private void RefreshInventoryUI()
{
    if (itemsContainer == null || itemSlotPrefab == null) 
    {
        Debug.LogError("Missing required references in InventoryManager!");
        return;
    }

    // Clear existing items
    foreach (Transform child in itemsContainer)
    {
        Destroy(child.gameObject);
    }

    // Add current items
    foreach (var item in inventory)
    {
        GameObject slot = Instantiate(itemSlotPrefab, itemsContainer);
            
        // Get the components - add null checks and debug logs
        Image iconImage = slot.transform.Find("ItemIcon")?.GetComponent<Image>();
        TextMeshProUGUI nameText = slot.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();

        if (iconImage == null)
            Debug.LogError("ItemIcon Image component not found on prefab!");
        if (nameText == null)
            Debug.LogError("ItemName TextMeshProUGUI component not found on prefab!");

        // Check if the item has valid data
        if (item.Value.icon == null)
            Debug.LogError($"No icon assigned for item: {item.Value.name}");

        // Set the values if components exist
        if (iconImage != null && item.Value.icon != null)
            iconImage.sprite = item.Value.icon;
        if (nameText != null)
            nameText.text = item.Value.name;

        // Optional: Add debug log to verify item data
        Debug.Log($"Adding item to inventory - Name: {item.Value.name}, Has Icon: {item.Value.icon != null}");
    }
}
}

// Inventory Item class
[System.Serializable]
public class InventoryItem
{
    public string name;
    public Sprite icon;

    public InventoryItem(string name, Sprite icon)
    {
        this.name = name;
        this.icon = icon;
    }
}