using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class Keypad : MonoBehaviour
{
    public TextMeshProUGUI displayText;  // Reference to TextMeshPro text field
    public GameObject[] numberButtons;  // Array of number buttons (0-9)
    public GameObject clearButton;      // Reference to the red clear button

    private string currentInput = "";    // Variable to track the current input

    // Start is called before the first frame update
    void Start()
    {
        // Add listeners for number buttons
        for (int i = 0; i < numberButtons.Length; i++)
        {
            int index = i;  // Local copy of i for the lambda expression
            numberButtons[i].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnNumberButtonPressed(index.ToString()));
        }

        // Add listener for the clear button
        clearButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(ClearText);
    }

    // Method to handle number button presses
    void OnNumberButtonPressed(string number)
    {
        if (currentInput.Length < 4)  // Limit input to 4 digits
        {
            currentInput += number;  // Append the number to current input
            displayText.text = currentInput;  // Update the display text
        }
    }

    // Method to clear the text
    void ClearText()
    {
        currentInput = "";  // Reset the current input string
        displayText.text = "";  // Clear the display
    }
}
