using UnityEngine;

public class CollectibleGear : InteractableObject
{
    [SerializeField] private string itemID = "Gear";
    [SerializeField] private AudioSource pickupSound;
    [SerializeField] private Sprite gearIcon;

    protected override void OnInteract()
    {
        if (playerInRange)
        {
            if (pickupSound != null)
            {
                pickupSound.Play();
            }
            InventoryManager.Instance.AddItem(itemID, "Gear", gearIcon);
            gameObject.SetActive(false);
        }
    }
}