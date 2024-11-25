using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MagicPigGames; // Import namespace for ProgressBar

public class HUDManager : MonoBehaviour
{
    public PlayerHealth playerHealth; // Reference to the PlayerHealth script
    public GameObject deathPanel; // Panel that appears when the player dies
    public HorizontalProgressBar healthProgressBar; // Reference to the horizontal progress bar for health
    public Timer gameTimer; // Reference to the Timer prefab

    private bool isGameActive = true; // Track whether the game is active

    void Start()
    {
        // Ensure PlayerHealth reference is set
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth reference is missing in HUDManager!");
        }

        // Ensure DeathPanel reference is set
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DeathPanel reference is missing in HUDManager!");
        }

        // Ensure Health Progress Bar is set
        if (healthProgressBar == null)
        {
            Debug.LogError("HealthProgressBar reference is missing in HUDManager!");
        }

        // Ensure Game Timer reference is set and subscribe to its event
        if (gameTimer == null)
        {
            Debug.LogError("GameTimer reference is missing in HUDManager!");
        }
        else
        {
            gameTimer.onTimerEnd.AddListener(HandleGameOver); // Trigger game-over when timer ends
        }

        UpdateHealthProgressBar(); // Display the initial health value
    }

    void Update()
    {
        if (isGameActive)
        {
            // Check health and update progress bar
            if (playerHealth != null)
            {
                UpdateHealthProgressBar(); // Update health progress bar

                if (playerHealth.currentHealth <= 0)
                {
                    HandleGameOver(); // Trigger game-over behavior when health is 0
                }
            }
        }

        // Restart game on pressing the "P" key
        if (Input.GetKeyDown(KeyCode.P))
        {
            RestartGame();
        }
    }

    // Update the health progress bar
    private void UpdateHealthProgressBar()
    {
        if (healthProgressBar != null && playerHealth != null)
        {
            float healthPercentage = (float)playerHealth.currentHealth / playerHealth.maxHealth; // Calculate health percentage
            healthProgressBar.SetProgress(healthPercentage); // Update the progress bar

            // Update the color based on current health
            healthProgressBar.UpdateHealthColor(playerHealth.currentHealth);
        }
    }

    // Handle game-over behavior
    public void HandleGameOver()
    {
        if (!isGameActive) return;

        Debug.Log("Game Over!");
        isGameActive = false; // Stop the game

        if (deathPanel != null)
        {
            deathPanel.SetActive(true); // Show the death panel
        }
    }

    // Method to restart the game
    public void RestartGame()
    {
        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
