using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI resumeText;
    [SerializeField] private TextMeshProUGUI returnText;
    [SerializeField] private TextMeshProUGUI quitText;

    [Header("UI Text Settings")]
    [SerializeField] private string titleString = "PAUSED";
    [SerializeField] private string resumeString = "Resume";
    [SerializeField] private string returnString = "Return to Menu";
    [SerializeField] private string quitString = "Quit Game";

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    [Header("Scene Reference")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void Start()
    {
        // Initialize UI
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Set up button listeners
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        if (returnButton != null)
            returnButton.onClick.AddListener(ReturnToMainMenu);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        // Set UI text
        if (titleText != null) titleText.text = titleString;
        if (resumeText != null) resumeText.text = resumeString;
        if (returnText != null) returnText.text = returnString;
        if (quitText != null) quitText.text = quitString;

        // Get references if not assigned
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();

        // Ensure game starts unpaused
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        
        // Disable player controls
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Pause time
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        
        // Enable player controls
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Resume time
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        // Resume time scale before loading new scene
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDisable()
    {
        // Remove button listeners
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);
        if (returnButton != null)
            returnButton.onClick.RemoveListener(ReturnToMainMenu);
        if (quitButton != null)
            quitButton.onClick.RemoveListener(QuitGame);

        // Ensure time scale is reset if script is disabled
        Time.timeScale = 1f;
    }
}