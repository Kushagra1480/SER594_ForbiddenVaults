using UnityEngine;
using TMPro;
using System.Collections;

public class GearMechanism : InteractableObject
{
    [Header("Gear References")]
    [SerializeField] private Transform[] gears;
    [SerializeField] private Transform gearSlot;
    [SerializeField] private Transform bars;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float barLowerDistance = 2f;
    [SerializeField] private float barLowerSpeed = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource gearTurningSound;
    [SerializeField] private AudioSource barLoweringSound;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackTextPanel;  // UI panel for feedback
    [SerializeField] private TextMeshProUGUI feedbackText;  // Text component
    [SerializeField] private float feedbackDuration = 2f;   // How long to show feedback

    private bool isGearPlaced = false;
    private bool isActivated = false;
    private Vector3 barsStartPosition;
    private bool isFeedbackShowing = false;

    private void Start()
    {
        base.Start();
        if (bars != null)
        {
            barsStartPosition = bars.position;
        }

        // Make sure feedback UI is hidden at start
        if (feedbackTextPanel != null)
            feedbackTextPanel.SetActive(false);
    }

    protected override void OnInteract()
    {
        if (!isGearPlaced && InventoryManager.Instance.HasItem("Gear")) {
            PlaceGear();
        }
        else if (isGearPlaced && !isActivated) {
            ActivateMechanism();
        }
        else if (!isGearPlaced) {
            ShowFeedback("Nothing happened");
        }
    }

    private void PlaceGear()
    {
        isGearPlaced = true;
        InventoryManager.Instance.RemoveItem("Gear");
        
        // Create and place gear visual
        GameObject placedGear = Instantiate(Resources.Load<GameObject>("Gear"), gearSlot.position, gearSlot.rotation);
        placedGear.transform.SetParent(gearSlot);
    }

    private void ActivateMechanism()
    {
        isActivated = true;
        ShowFeedback("The aged machinery groans to life, shifting hidden mechanisms.");
        
        if (gearTurningSound != null)
            gearTurningSound.Play();
        
        StartCoroutine(RotateGearsCoroutine());
        StartCoroutine(LowerBarsCoroutine());
    }

    private void ShowFeedback(string message)
    {
        if (isFeedbackShowing)
            return;

        StartCoroutine(ShowFeedbackCoroutine(message));
    }

    private IEnumerator ShowFeedbackCoroutine(string message)
    {
        isFeedbackShowing = true;

        // Show feedback UI
        if (feedbackTextPanel != null && feedbackText != null)
        {
            feedbackText.text = message;
            feedbackTextPanel.SetActive(true);

            yield return new WaitForSeconds(feedbackDuration);

            feedbackTextPanel.SetActive(false);
        }

        isFeedbackShowing = false;
    }

    private IEnumerator RotateGearsCoroutine()
    {
        while (isActivated)
        {
            foreach (Transform gear in gears)
            {
                gear.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
            }
            yield return null;
        }
    }

    private IEnumerator LowerBarsCoroutine()
    {
        if (barLoweringSound != null)
        {
            barLoweringSound.Play();
        }

        Vector3 targetPosition = barsStartPosition + Vector3.down * barLowerDistance;
        float elapsedTime = 0f;

        while (elapsedTime < barLowerSpeed)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / barLowerSpeed;
            
            bars.position = Vector3.Lerp(barsStartPosition, targetPosition, percentageComplete);
            yield return null;
        }

        // Ensure bars are exactly at target position
        bars.position = targetPosition;
    }
}