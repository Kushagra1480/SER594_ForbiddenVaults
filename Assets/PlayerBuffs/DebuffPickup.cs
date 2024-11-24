using UnityEngine;
using StarterAssets;

public class DebuffPickup : MonoBehaviour
{
    public enum DebuffType
    {
        Slow,
        Shrink
    }

    public DebuffType debuffType;
    public float rotationSpeed = 50f;
    public bool debugMode = true;

    [Header("Visual Settings")]
    public Color debuffColor = Color.red;

    private void Start()
    {
        // Set the color of the pickup
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = debuffColor;
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugMode) Debug.Log($"Trigger entered by: {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            if (debugMode) Debug.Log("Player entered trigger");

            var debuffSystem = other.GetComponent<PlayerDebuffSystem>();
            if (debuffSystem != null)
            {
                switch (debuffType)
                {
                    case DebuffType.Slow:
                        debuffSystem.ActivateSlowDebuff();
                        if (debugMode) Debug.Log("Activated slow debuff");
                        break;

                    case DebuffType.Shrink:
                        debuffSystem.ActivateShrinkDebuff();
                        if (debugMode) Debug.Log("Activated shrink debuff");
                        break;
                }
                
                Destroy(gameObject);
            }
        }
    }
}