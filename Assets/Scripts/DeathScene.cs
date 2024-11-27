using UnityEngine;

public class DeathScene : MonoBehaviour
{
    public PlayerHealth playerHealth; // Reference to the PlayerHealth component

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object has the PlayerHealth component
        PlayerHealth collidedPlayerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (collidedPlayerHealth != null)
        {
            collidedPlayerHealth.TakeDamage(collidedPlayerHealth.currentHealth); // Reduce health to zero
            Debug.Log("Player's health set to 0 due to collision.");
        }
    }

    // Optional: Use this if you want the box collider to be a trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger has the PlayerHealth component
        PlayerHealth triggeredPlayerHealth = other.gameObject.GetComponent<PlayerHealth>();

        if (triggeredPlayerHealth != null)
        {
            triggeredPlayerHealth.TakeDamage(triggeredPlayerHealth.currentHealth); // Reduce health to zero
            Debug.Log("Player's health set to 0 due to trigger.");
        }
    }
}
