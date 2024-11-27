using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu2 : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the Pause Menu UI GameObject

    private bool isPaused = false; // Tracks if the game is paused

    void Update()
    {
        // Toggle pause when pressing the Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // Resume game if it's paused
            }
            else
            {
                PauseGame(); // Pause game if it's not paused
            }
        }
    }

    // Pauses the game and activates the pause menu
    public void PauseGame()
    {
        pauseMenuUI.SetActive(true); // Show the Pause Menu UI
        Time.timeScale = 0f; // Freeze the game
        isPaused = true;
    }

    // Resumes the game and deactivates the pause menu
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the Pause Menu UI
        Time.timeScale = 1f; // Unfreeze the game
        isPaused = false;

    }

    // Restarts the current level
    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure the game is unpaused
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reload the current scene
    }

    // Returns to the Main Menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f; // Ensure the game is unpaused
        SceneManager.LoadScene("MainMenuScene"); // Load the Main Menu scene
    }
}
