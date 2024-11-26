using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class WindowInteraction : InteractableObject
{
    [Header("Window Settings")]
    [SerializeField] private GameObject textDisplayPanel;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private Camera windowCam;
    [SerializeField] private Transform[] cameraWaypoints;

    [Header("Camera Movement")]
    [SerializeField] private float panDuration = 1.5f;
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private AnimationCurve panCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Player References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private FirstPersonController fpsController;

    private Camera playerCamera;
    private bool isShowingMessage = false;
    private bool isPanning = false;
    private bool hasSeenWindow = false;

    private void Awake()
    {
        textDisplayPanel.SetActive(false);
        if (playerController == null)
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
        if (fpsController == null)
            fpsController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        
        playerCamera = Camera.main;
        if (windowCam != null)
            windowCam.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (!isShowingMessage)
        {
            base.Update();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !isPanning)
        {
            HideMessage();
        }
    }

    protected override void OnInteract()
    {
        StartCoroutine(WindowInspectionSequence());
    }

    private IEnumerator WindowInspectionSequence()
    {
        isShowingMessage = true;
        DisablePlayerControl();
        SwitchToWindowCamera();

        // First observation
        if (!hasSeenWindow)
        {
            ShowMessage("A stained glass window... The colored panels seem significant.");
            yield return new WaitForSeconds(3f);
        }

        // Start camera pan
        isPanning = true;
        yield return StartCoroutine(PanThroughWaypoints());
        isPanning = false;

        // Show detailed observation based on inspection
        if (!hasSeenWindow)
        {
            ShowMessage("The dawn light reveals a pattern...\nFirst burns the crimson sunrise,\nThen ocean's depths darken the sky,\nDesert sand brings the heat of day,\nForest shadows mark the way.");
            hasSeenWindow = true;
        }
        else
        {
            ShowMessage("The colors still hold their secret:\nRed dawn,\nBlue depths,\nYellow sands,\nGreen shadows.");
        }

        // Wait for player to dismiss
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        HideMessage();
    }

    private IEnumerator PanThroughWaypoints()
    {
        for (int i = 0; i < cameraWaypoints.Length - 1; i++)
        {
            yield return StartCoroutine(PanBetweenPoints(
                cameraWaypoints[i].position,
                cameraWaypoints[i].rotation,
                cameraWaypoints[i + 1].position,
                cameraWaypoints[i + 1].rotation
            ));
            
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    private IEnumerator PanBetweenPoints(Vector3 startPos, Quaternion startRot, Vector3 endPos, Quaternion endRot)
    {
        float elapsed = 0f;
        
        while (elapsed < panDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / panDuration;
            float curveValue = panCurve.Evaluate(t);
            
            windowCam.transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            windowCam.transform.rotation = Quaternion.Slerp(startRot, endRot, curveValue);
            
            yield return null;
        }
        
        windowCam.transform.position = endPos;
        windowCam.transform.rotation = endRot;
    }

    private void ShowMessage(string message)
    {
        if (textDisplayPanel != null && displayText != null)
        {
            displayText.text = message;
            textDisplayPanel.SetActive(true);
        }
    }

    private void HideMessage()
    {
        isShowingMessage = false;
        textDisplayPanel.SetActive(false);
        SwitchToPlayerCamera();
        EnablePlayerControl();
    }

    private void DisablePlayerControl()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (playerController != null)
            playerController.enabled = false;
        if (fpsController != null)
            fpsController.enabled = false;
    }

    private void EnablePlayerControl()
    {
        if (playerController != null)
            playerController.enabled = true;
        if (fpsController != null)
            fpsController.enabled = true;
    }

    private void SwitchToWindowCamera()
    {
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(false);
        if (windowCam != null)
            windowCam.gameObject.SetActive(true);
    }

    private void SwitchToPlayerCamera()
    {
        if (windowCam != null)
            windowCam.gameObject.SetActive(false);
        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (isShowingMessage)
        {
            HideMessage();
        }
    }
}