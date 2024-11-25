using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    private bool isPaused = false;

    // Function to resume the level
    public void ResumeLevel()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            Debug.Log("Level is already running.");
        }
    }

    // Function to restart the level (game over)
    public void RestartLevel()
    {
        Time.timeScale = 1; // Resume time before restarting
        SceneManager.LoadScene("LevelZeroScene"); // Restart the level
    }

    // Function to return to the main menu
    public void ReturnToMenu()
    {
        Time.timeScale = 1; // Ensure time is resumed before returning to the menu
        SceneManager.LoadScene("MainMenuScene");
    }

    // Function to quit the game
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    // Function to pause the game
    public void PauseGame()
    {
        Time.timeScale = 0; // Freeze time when the game is paused
        isPaused = true;
        Debug.Log("Game Paused.");
    }

    // Function to resume the game
    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game time
        isPaused = false;
        Debug.Log("Game Resumed.");
    }


}
