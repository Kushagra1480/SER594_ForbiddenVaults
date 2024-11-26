using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System.Collections;
using StarterAssets;

public class BookInteraction : InteractableObject
{
    [Header("Book Settings")]
    [SerializeField] private GameObject textDisplayPanel;
    [SerializeField] private TextMeshProUGUI bookTextDisplay;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private bool isShowingMessage = false;

    private void Awake() {
        textDisplayPanel.SetActive(false);
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void Update() {
        if (!isShowingMessage) {
            base.Update();
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            HideMessage();
        }
    }

    protected override void OnInteract() {
        ShowMessage();
    }

    private void ShowMessage() {
        isShowingMessage = true;
        bookTextDisplay.text = "Time tells all, but death keeps no schedule";
        textDisplayPanel.SetActive(true);
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void HideMessage() {
        isShowingMessage = false;
        textDisplayPanel.SetActive(false);
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }
}
