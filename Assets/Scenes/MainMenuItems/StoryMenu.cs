using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LevelZeroScene");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }


}
