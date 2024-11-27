using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoryMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Maze1");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

}
