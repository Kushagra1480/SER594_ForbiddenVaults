using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class LevelIntroHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private TextMeshProUGUI pressEText; // Optional "Press E to begin" text
    [SerializeField] private float textScrollSpeed = 50f;

    [Header("Story Text")]
    [TextArea(5, 10)]
    [SerializeField] private string introStory = @"The old mansion stands before you, its secrets locked behind mechanical puzzles and ancient mechanisms.

Rumors speak of a mysterious artifact hidden within, protected by intricate machinery that has stood the test of time.

Your research has led you here, to this moment. The fusebox, the gears, the cryptic clues...
They all hold the key to unlocking the truth.

But be warned: once you begin this journey, there may be no turning back...";

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isTextComplete = false;

    private void Start()
    {
        StartIntroSequence();
    }

    private void Update()
    {
        if (isTextComplete && Input.GetKeyDown(KeyCode.E))
        {
            StartGame();
        }
    }

    private void StartIntroSequence()
    {
        // Disable player controls initially
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Setup intro UI
        if (introPanel != null)
        {
            introPanel.SetActive(true);
            storyText.text = ""; // Clear text before scrolling
            if (pressEText != null)
                pressEText.gameObject.SetActive(false);
            StartCoroutine(ScrollText());
        }
    }

    private IEnumerator ScrollText()
    {
        if (storyText != null)
        {
            foreach (char c in introStory)
            {
                storyText.text += c;
                yield return new WaitForSeconds(1f / textScrollSpeed);
            }

            isTextComplete = true;
            
            // Show "Press E to begin" text
            if (pressEText != null)
                pressEText.gameObject.SetActive(true);
        }
    }

    private void StartGame()
    {
        // Hide intro panel
        if (introPanel != null)
            introPanel.SetActive(false);

        // Enable player controls
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;

        // Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Destroy the intro handler
        Destroy(this.gameObject);
    }
}