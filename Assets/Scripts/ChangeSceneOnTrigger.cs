using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnTrigger : MonoBehaviour
{
    // Name of the scene to load
    public string sceneName = "TargetSceneName";

    // This method is called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player or a specific object
        if (other.CompareTag("Player")) // Ensure your player has the "Player" tag
        {
            // Load the specified scene
            SceneManager.LoadScene(sceneName);
        }
    }
}
