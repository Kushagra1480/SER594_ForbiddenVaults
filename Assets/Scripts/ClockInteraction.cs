using UnityEngine;
using UnityEngine.UIElements;
using TMPro;
using System.Collections;
using StarterAssets;

public class ClockInteraction : InteractableObject {
    [Header("Clock Settings")]
    [SerializeField] private GameObject timeDisplayPanel;
    [SerializeField] private TextMeshProUGUI timeDisplayText;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    [Header("Camera Settings")]
    [SerializeField] private Camera clockCam;
    [SerializeField] private Transform[] cameraWaypoints;  // Array of positions for camera movement
    [SerializeField] private float panDuration = 2f;      // How long to move between each position
    [SerializeField] private float pauseDuration = 1f;    // How long to pause at each position
    [SerializeField] private AnimationCurve panCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Camera playerCamera;
    private bool isShowingMessage = false;
    private bool isPanning = false;

    private void Awake() {
        timeDisplayPanel.SetActive(false);
        if (playerController == null) {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        }
        if (clockCam != null)
            clockCam.gameObject.SetActive(false);
            
        playerCamera = Camera.main;
    }

    protected override void Update() {
        if (!isShowingMessage) {
            base.Update();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isPanning) {
            HideMessage();
        }
    }

    protected override void OnInteract() {
        StartCoroutine(ShowMessageSequence());
    }

    private IEnumerator ShowMessageSequence() {
        isShowingMessage = true;
        timeDisplayText.text = "The clock has stopped... Frozen at 8:45. Its hands refuse to move.";
        timeDisplayPanel.SetActive(true);
        
        DisablePlayerControl();
        SwitchToClockCamera();
        
        // Start the camera pan sequence
        isPanning = true;
        yield return StartCoroutine(PanThroughWaypoints());
        isPanning = false;
        
        // Wait for player input to exit
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideMessage();
    }

    private IEnumerator PanThroughWaypoints() {
        for (int i = 0; i < cameraWaypoints.Length - 1; i++) {
            yield return StartCoroutine(PanBetweenPoints(
                cameraWaypoints[i].position,
                cameraWaypoints[i].rotation,
                cameraWaypoints[i + 1].position,
                cameraWaypoints[i + 1].rotation
            ));
            
            // Pause at waypoint
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    private IEnumerator PanBetweenPoints(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot) {
        float elapsed = 0f;
        
        while (elapsed < panDuration) {
            elapsed += Time.deltaTime;
            float t = elapsed / panDuration;
            
            // Use animation curve for smooth movement
            float curveValue = panCurve.Evaluate(t);
            
            clockCam.transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            clockCam.transform.rotation = Quaternion.Slerp(startRot, endRot, curveValue);
            
            yield return null;
        }
        
        // Ensure we end exactly at the target
        clockCam.transform.position = endPos;
        clockCam.transform.rotation = endRot;
    }

    private void DisablePlayerControl() {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void EnablePlayerControl() {
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }

    private void SwitchToClockCamera() {
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);
        if (clockCam != null)
            clockCam.gameObject.SetActive(true);
    }

    private void SwitchToPlayerCamera() {
        if (clockCam != null)
            clockCam.gameObject.SetActive(false);
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);
    }

    private void HideMessage() {
        isShowingMessage = false;
        timeDisplayPanel.SetActive(false);
        SwitchToPlayerCamera();
        EnablePlayerControl();
    }

    private void OnDisable() {
        if (isShowingMessage) {
            HideMessage();
        }
    }
}