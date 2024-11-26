using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;
using System.Collections.Generic;

public class FuseBoxInteraction : InteractableObject {
    [Header("Fusebox Components")]
    [SerializeField] private GameObject electricalComponent;
    [SerializeField] private GameObject[] highlighterPens;  // Array of 4 highlighter pen objects
    [SerializeField] private Material activatedMaterial;
    [SerializeField] private Material deactivatedMaterial;
    [SerializeField] private Light[] indicatorLights;      // Array of 4 lights above switches
    [SerializeField] private float rotationAmount = 45f;   // Amount to rotate on X axis
    [SerializeField] private float rotationSpeed = 5f;

     [Header("Light Colors")]
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private Color onColor = Color.green;
    
    [Header("Camera Settings")]
    [SerializeField] private Camera puzzleCamera;
    [SerializeField] private Transform cameraPosition;
    
    [Header("Feedback")]
    [SerializeField] private GameObject feedbackPanel;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    [Header("Audio")]
    [SerializeField] private AudioSource switchSound;
    [SerializeField] private AudioSource successSound;
    [SerializeField] private AudioSource failSound;

    [Header("Door References")]
    [SerializeField] private GameObject doorToDisable;  // Reference to the door object
    [SerializeField] private ParticleSystem doorDisappearEffect;  // Optional particle effect
    [SerializeField] private float doorDisableDelay = 1.5f;  // Delay before door disappears
    
    private bool isComponentInstalled = false;
    private bool isPuzzleActive = false;
    private bool isShowingMessage = false;
    private List<int> correctSequence = new List<int> { 0, 2, 3, 1 };  // Red, Blue, Yellow, Green
    private List<int> playerSequence = new List<int>();
    private MeshRenderer[] highlighterRenderers;
    private bool[] switchStates;  // Track if switches are on/off
    private Quaternion[] originalRotations;
    private Dictionary<KeyCode, int> switchKeys = new Dictionary<KeyCode, int>()
    {
        { KeyCode.Alpha1, 0 }, // Red
        { KeyCode.Alpha2, 1 }, // Green
        { KeyCode.Alpha3, 2 }, // Blue
        { KeyCode.Alpha4, 3 }  // Yellow
    };

    protected override void Start() {
        base.Start();
        
        // Get references if not assigned
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
            
        // Initial setup
        if (electricalComponent != null)
            electricalComponent.SetActive(false);
        if (puzzleCamera != null)
            puzzleCamera.gameObject.SetActive(false);
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);

        switchStates = new bool[highlighterPens.Length];
        originalRotations = new Quaternion[highlighterPens.Length];
        
        // Store original rotations and setup lights
        for (int i = 0; i < highlighterPens.Length; i++) {
            originalRotations[i] = highlighterPens[i].transform.rotation;
            if (indicatorLights != null && i < indicatorLights.Length) {
                indicatorLights[i].color = offColor;
            }
        }
            
        // Setup highlighter renderers array
        SetupHighlighterRenderers();
            
        // Reset all highlighters to deactivated state
        ResetHighlighters();
    }
    

    private void SetupHighlighterRenderers() {
        highlighterRenderers = new MeshRenderer[highlighterPens.Length];
        
        for (int i = 0; i < highlighterPens.Length; i++) {
            // Try to get MeshRenderer from the object itself
            highlighterRenderers[i] = highlighterPens[i].GetComponent<MeshRenderer>();
            
            // If not found, look for it in children
            if (highlighterRenderers[i] == null) {
                highlighterRenderers[i] = highlighterPens[i].GetComponentInChildren<MeshRenderer>();
            }
            
            if (highlighterRenderers[i] == null) {
                Debug.LogError($"No MeshRenderer found for highlighter pen {i} or its children!");
            }

            // Add collider if missing
            if (highlighterPens[i].GetComponent<BoxCollider>() == null && 
                highlighterPens[i].GetComponentInChildren<BoxCollider>() == null) {
                
                MeshRenderer meshRenderer = highlighterPens[i].GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null) {
                    BoxCollider boxCollider = highlighterPens[i].AddComponent<BoxCollider>();
                    boxCollider.size = meshRenderer.bounds.size;
                    boxCollider.center = meshRenderer.bounds.center - highlighterPens[i].transform.position;
                }
            }
        }
    }

    protected void ResetHighlighters() {
        for (int i = 0; i < highlighterPens.Length; i++) {
            // Reset rotation
            highlighterPens[i].transform.rotation = originalRotations[i];
            
            // Reset switch states
            switchStates[i] = false;
            
            // Reset lights
            if (indicatorLights != null && i < indicatorLights.Length) {
                indicatorLights[i].color = offColor;
            }
        }
    }

    protected override void Update() {
        if (!isShowingMessage && !isPuzzleActive) {
            base.Update();
        }
        else if (isPuzzleActive) {
            HandlePuzzleInput();
            HandleMouseInput();
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            HideMessage();
        }

        // Allow escape at any time to return to player view
        if ((isPuzzleActive || isShowingMessage) && Input.GetKeyDown(KeyCode.Escape)) {
            ReturnToPlayerView();
        }
    }

    private void HandleMouseInput() {
        if (Input.GetMouseButtonDown(0)) { // Left mouse click
            Ray ray = puzzleCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                // Find which highlighter was clicked
                for (int i = 0; i < highlighterPens.Length; i++) {
                    if (hit.collider.gameObject == highlighterPens[i] || 
                        hit.collider.transform.IsChildOf(highlighterPens[i].transform)) {
                        ActivateHighlighter(i);
                        break;
                    }
                }
            }
        }
    }

    protected override void OnInteract() {
        // Hide interaction prompt immediately when interacting
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (!isComponentInstalled) {
            if (InventoryManager.Instance.HasItem("5")) { // Electrical component ID
                InstallComponent();
            } else {
                ShowMessage("The fusebox panel is missing its primary switch. A diagram shows it requires a specific component");
            }
        } else if (!isPuzzleActive) {
            StartPuzzle();
        }
    }

    private void InstallComponent() {
        isComponentInstalled = true;
        InventoryManager.Instance.RemoveItem("5");
        
        if (electricalComponent != null)
            electricalComponent.SetActive(true);
            
        ShowMessage("You installed the electrical component in the fusebox");
    }

     private void StartPuzzle() {
        isPuzzleActive = true;
        
        // Ensure interaction prompt is hidden
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Disable player controls
        DisablePlayerControls();
        
        // Switch to puzzle camera
        playerCamera.gameObject.SetActive(false);
        puzzleCamera.gameObject.SetActive(true);
        
        // Enable mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Reset puzzle state
        playerSequence.Clear();
        ResetHighlighters();
    }


    private void HandlePuzzleInput() {
        foreach (var switchKey in switchKeys) {
            if (Input.GetKeyDown(switchKey.Key)) {
                ActivateHighlighter(switchKey.Value);
            }
        }
    }

    private void ActivateHighlighter(int index) {
        if (switchSound != null)
            switchSound.Play();
            
        // Toggle switch state
        switchStates[index] = !switchStates[index];
        
        // Update light color
        if (indicatorLights != null && index < indicatorLights.Length) {
            indicatorLights[index].color = switchStates[index] ? onColor : offColor;
        }
        
        // Rotate the highlighter
        StartCoroutine(RotateHighlighter(index));
        
        playerSequence.Add(index);
        
        // Check if sequence is complete
        if (playerSequence.Count == correctSequence.Count) {
            StartCoroutine(CheckSequence());
        }
    }
    private IEnumerator RotateHighlighter(int index) {
        Quaternion startRotation = highlighterPens[index].transform.rotation;
        Quaternion targetRotation;
        
        if (switchStates[index]) {
            // Rotate forward
            targetRotation = originalRotations[index] * Quaternion.Euler(rotationAmount, 0, 0);
        } else {
            // Rotate back
            targetRotation = originalRotations[index];
        }
        
        float elapsed = 0f;
        
        while (elapsed < 1f) {
            elapsed += Time.deltaTime * rotationSpeed;
            highlighterPens[index].transform.rotation = 
                Quaternion.Lerp(startRotation, targetRotation, elapsed);
            yield return null;
        }
        
        // Ensure final rotation is exact
        highlighterPens[index].transform.rotation = targetRotation;
    }

    private IEnumerator CheckSequence() {
        yield return new WaitForSeconds(0.5f);
        
        bool isCorrect = true;
        for (int i = 0; i < correctSequence.Count; i++) {
            if (playerSequence[i] != correctSequence[i]) {
                isCorrect = false;
                break;
            }
        }
        
        if (isCorrect) {
            if (successSound != null)
                successSound.Play();
                
            ShowMessage("You hear the door creak");
            
            // Start door disappearance sequence
            StartCoroutine(DisableDoorSequence());
            
            yield return new WaitForSeconds(1f);
            ReturnToPlayerView();
            this.enabled = false; // Disable further interaction
        } else {
            if (failSound != null)
                failSound.Play();
                
            // Reset everything
            ResetHighlighters();
            playerSequence.Clear();
        }
    }

    private IEnumerator DisableDoorSequence() {
        if (doorToDisable != null) {
            // Wait for specified delay
            yield return new WaitForSeconds(doorDisableDelay);

            // Play particle effect if assigned
            if (doorDisappearEffect != null) {
                // Position effect at door's position
                doorDisappearEffect.transform.position = doorToDisable.transform.position;
                doorDisappearEffect.Play();
            }

            // Optional: Fade out door material before disabling
            MeshRenderer doorRenderer = doorToDisable.GetComponent<MeshRenderer>();
            if (doorRenderer != null) {
                Material doorMat = doorRenderer.material;
                Color startColor = doorMat.color;
                float elapsed = 0f;
                float fadeDuration = 1f;

                while (elapsed < fadeDuration) {
                    elapsed += Time.deltaTime;
                    float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                    doorMat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                    yield return null;
                }
            }

            // Disable the door
            doorToDisable.SetActive(false);
        }
    }

    private void ReturnToPlayerView() {
        isPuzzleActive = false;
        
        // Switch cameras
        puzzleCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);
        
        // Enable player controls
        EnablePlayerControls();
        
        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Ensure interaction prompt stays hidden after puzzle
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }


    private void ShowMessage(string message) {
        isShowingMessage = true;
        DisablePlayerControls();

        // Ensure interaction prompt is hidden when showing message
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (feedbackPanel != null && feedbackText != null) {
            feedbackText.text = message;
            feedbackPanel.SetActive(true);
        }
    }

    private void HideMessage() {
        isShowingMessage = false;
        
        // Only re-enable player controls if we're not in the puzzle
        if (!isPuzzleActive) {
            EnablePlayerControls();
            
            // Only show interaction prompt if not installed or puzzle not started
            if (!isComponentInstalled && interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }

        if (feedbackPanel != null)
            feedbackPanel.SetActive(false);
    }

    private void DisablePlayerControls() {
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void EnablePlayerControls() {
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }

    private void OnDisable() {
        if (isPuzzleActive || isShowingMessage) {
            ReturnToPlayerView();
        }
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
}
    