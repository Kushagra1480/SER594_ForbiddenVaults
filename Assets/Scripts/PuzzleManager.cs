using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;  // Required for UI text updates

public class PuzzleManager : MonoBehaviour
{
    public TextMeshPro resultText;  // Reference to the UI Text component to display the result
    public List<PuzzleData> puzzleDataObjects;  // List of PuzzleData objects to check

    // Start is called before the first frame update
    void Start()
    {
        // Optionally, initialize puzzleDataObjects if not set in the Inspector
        if (puzzleDataObjects == null || puzzleDataObjects.Count == 0)
        {
            puzzleDataObjects = new List<PuzzleData>(FindObjectsOfType<PuzzleData>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPuzzleAlignment();  // Continuously check puzzle alignment in real-time
    }

    // Check if all puzzle data objects are aligned correctly
    void CheckPuzzleAlignment()
    {
        bool allCorrect = true;

        // Loop through all PuzzleData objects and check if they are correct
        foreach (var puzzle in puzzleDataObjects)
        {
            if (!puzzle.isCorrectAngle)  // Check if the current piece is correctly aligned
            {
                allCorrect = false;
                break;  // No need to check further if one object is incorrect
            }
        }

        // Update the text based on the puzzle alignment status
        if (allCorrect)
        {
            resultText.text = "5";  // Display 5 if all puzzle pieces are correct
        }
        else
        {
            resultText.text = "";  // Clear the text if the puzzle is incorrect
        }
    }
}
