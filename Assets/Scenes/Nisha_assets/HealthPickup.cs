using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1; // Amount of health the pickup restores

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log("Player picked up a health pack! Healed " + healAmount + " health.");
            Destroy(gameObject); // Destroy the health pickup after use
        }
    }
}
