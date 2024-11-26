using UnityEngine;
using TMPro;
using System.Collections;

public class NumpadInteraction : InteractableObject
{
    [Header("Numpad UI References")]
    [SerializeField] private GameObject numpadPanel;
    [SerializeField] private TextMeshProUGUI instructionsDisplay;
    [SerializeField] private TMP_InputField inputField;

    [Header("Numpad Settings")]
    [SerializeField] private string correctCode = "845";
    [SerializeField] private Cabinet cabinetToOpen;
    [SerializeField] private string defaultInstructions = "Enter Code";
    
    [SerializeField] private CharacterController playerController;
    
    private string currentInput = "";
    private bool isNumpadActive = false;
    private bool isShowingMessage = false;

    private void Awake()
    {
        numpadPanel.SetActive(false);
        instructionsDisplay.text = defaultInstructions;
        
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(OnInputFieldChanged);
            inputField.characterLimit = 3;
        }
        
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player")
                .GetComponent<CharacterController>();
        }
    }

    protected override void Update()
    {
        // Only check for initial interaction if not showing numpad
        if (!isNumpadActive)
        {
            base.Update();
        }
        // Handle numpad input when active
        else if (isNumpadActive)
        {
            HandleNumpadInput();
        }
    }

    protected override void OnInteract()
    {
        ShowNumpad();
    }

    private void ShowNumpad()
    {
        isNumpadActive = true;
        numpadPanel.SetActive(true);
        ResetNumpad();
        
        // Disable interaction prompt and player controller
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
            
        if (playerController != null)
            playerController.enabled = false;
    }

    private void HandleNumpadInput()
    {
        // Handle number inputs (both number row and numpad)
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i) || Input.GetKeyDown(KeyCode.Keypad0 + i))
            {
                AddNumber(i);
            }
        }

        // Handle backspace
        if (Input.GetKeyDown(KeyCode.Backspace) && currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
            UpdateDisplay();
        }

        // Handle enter/return
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CheckCode();
        }

        // Handle escape to close numpad
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseNumpad();
        }
    }

    private void AddNumber(int number)
    {
        if (currentInput.Length < 3)
        {
            currentInput += number.ToString();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (inputField != null)
        {
            inputField.text = currentInput;
        }
    }

    private void CheckCode() {
        if (currentInput == correctCode) {
            OnCorrectCode();
        }
        else {
            StartCoroutine(ShowErrorMessage());
        }
    }

    private void OnCorrectCode() {
        cabinetToOpen.Open();
        instructionsDisplay.text = "Correct!";
        StartCoroutine(DisableNumpadDelayed(1f));
    }

    private IEnumerator ShowErrorMessage() {
        instructionsDisplay.text = "ERROR";
        currentInput = "";
        
        yield return new WaitForSeconds(1f);
        
        instructionsDisplay.text = defaultInstructions;
        UpdateDisplay();
    }

     private IEnumerator DisableNumpadDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        CloseNumpad();
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        this.enabled = false;
    }

    private void OnDisable() {
        if (numpadPanel != null)
            numpadPanel.SetActive(false);
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (playerController != null && !playerController.enabled)
            playerController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void CloseNumpad()
    {
        isNumpadActive = false;
        numpadPanel.SetActive(false);
        
        // Re-enable player controller when closing
        if (playerController != null)
            playerController.enabled = true;
            
        ResetNumpad();
    }


    private void ResetNumpad()
    {
        currentInput = "";
        UpdateDisplay();
        instructionsDisplay.text = defaultInstructions;
        if (inputField != null)
        {
            inputField.text = "";
        }
    }

    private void OnInputFieldChanged(string value)
    {
        string numbersOnly = System.Text.RegularExpressions.Regex.Replace(value, "[^0-9]", "");
        
        if (numbersOnly.Length <= 3)
        {
            currentInput = numbersOnly;
            UpdateDisplay();
        }
        else
        {
            currentInput = numbersOnly.Substring(0, 3);
            UpdateDisplay();
        }
    }
}