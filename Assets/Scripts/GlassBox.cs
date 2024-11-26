using UnityEngine;

public class GlassBox : InteractableObject
{
    [Header("Glass Box Settings")]
    [SerializeField] private GameObject glassObject;
    [SerializeField] private GameObject keyObject;
    [SerializeField] private Transform bars;
    [SerializeField] private float requiredBarHeight = 1f; // Height at which bars are considered lowered
    [SerializeField] private ParticleSystem glassBreakEffect;
    [SerializeField] private AudioSource breakSound;

    private bool isBroken = false;

    protected override void OnInteract()
    {
        if (!isBroken && CanBreakGlass())
        {
            BreakGlass();
        }
        else if (!isBroken)
        {
            if (!AreBarsBelowThreshold())
            {
                DisplayMessage("The protective bars need to be lowered first.");
            }
            else if (!InventoryManager.Instance.HasItem("Hammer"))
            {
                DisplayMessage("I need something to break this glass.");
            }
        }
    }

    private bool CanBreakGlass()
    {
        return AreBarsBelowThreshold() && InventoryManager.Instance.HasItem("Hammer");
    }

    private bool AreBarsBelowThreshold()
    {
        return bars.position.y <= requiredBarHeight;
    }

    private void BreakGlass()
    {
        isBroken = true;

        if (glassObject != null)
        {
            glassObject.SetActive(false);
        }

        if (glassBreakEffect != null)
        {
            glassBreakEffect.Play();
        }

        if (breakSound != null)
        {
            breakSound.Play();
        }

        if (keyObject != null)
        {
            keyObject.GetComponent<CollectibleGear>().enabled = true;
        }
    }

    private void DisplayMessage(string message)
    {
        // Implement your message display system here
        Debug.Log(message);
    }
}