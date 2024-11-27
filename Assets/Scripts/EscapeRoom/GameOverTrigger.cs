using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using StarterAssets;

public class GameOverTrigger : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button endGameButton;
    [SerializeField] private float textScrollSpeed = 50f;  // Speed in characters per second
    
    [Header("Story Text")]
    [TextArea(5, 10)]
    [SerializeField] private string endingStory = @"As you step through the doorway, you feel a strange shift in reality...

The mechanical puzzles, the cryptic clues, they were all leading to this moment.

What lies beyond might forever remain a mystery, but you've proven yourself worthy of discovering its secrets.

Sometimes the greatest victories are not in the destination, but in the courage to step through the door...

THE END";

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;
    [SerializeField] private Camera playerCamera;

    private bool hasTriggered = false;

    private void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
            
        if (endGameButton != null)
            endGameButton.onClick.AddListener(OnEndGameClick);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            StartGameOver();
        }
    }

    private void StartGameOver()
    {
        // Disable player controls
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
            
        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Show game over panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            storyText.text = "";  // Clear text before scrolling
            StartCoroutine(ScrollText());
        }
    }

    private IEnumerator ScrollText()
    {
        if (storyText != null)
        {
            // Hide end game button until text is done
            if (endGameButton != null)
                endGameButton.gameObject.SetActive(false);

            // Scroll text character by character
            foreach (char c in endingStory)
            {
                storyText.text += c;
                yield return new WaitForSeconds(1f / textScrollSpeed);
            }

            // Show end game button after text is complete
            if (endGameButton != null)
                endGameButton.gameObject.SetActive(true);
        }
    }

    public void OnEndGameClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        if (endGameButton != null)
            endGameButton.onClick.RemoveListener(OnEndGameClick);
    }
}