using UnityEngine;
using MagicPigGames; // Include the namespace for HorizontalProgressBar

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Maximum health the player can have
    public int currentHealth; // Current health of the player

    public HorizontalProgressBar healthProgressBar; // Reference to the HorizontalProgressBar

    public CharacterController characterController; // Reference to the player's Character Controller

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;

        // Check and set the health progress bar's value
        if (healthProgressBar != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthProgressBar.SetProgress(healthPercentage);
            healthProgressBar.UpdateHealthColor(currentHealth); // Set the initial color
        }
        else
        {
            Debug.LogError("HealthProgressBar is not assigned in the Inspector!");
        }

        // Assign CharacterController automatically if it's not set in the Inspector
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (characterController == null)
        {
            Debug.LogError("CharacterController component not found on the player.");
        }
    }

    // Method to handle damage
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't go below 0

        // Update the health progress bar
        if (healthProgressBar != null)
        {
            float healthPercentage = (float)currentHealth / maxHealth;
            healthProgressBar.SetProgress(healthPercentage);
            healthProgressBar.UpdateHealthColor(currentHealth); // Update color
        }

        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed max health

            // Update the health progress bar
            if (healthProgressBar != null)
            {
                float healthPercentage = (float)currentHealth / maxHealth;
                healthProgressBar.SetProgress(healthPercentage);
                healthProgressBar.UpdateHealthColor(currentHealth); // Update color
            }

            Debug.Log("Player healed. Current health: " + currentHealth);
        }
        else
        {
            Debug.Log("Health is already full.");
        }
    }

    // Method to handle player death
    void Die()
    {
        Debug.Log("Player has died.");

        // Disable the Character Controller to prevent movement
        if (characterController != null)
        {
            characterController.enabled = false;
        }
    }

    // Method to check if the player is dead
    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
