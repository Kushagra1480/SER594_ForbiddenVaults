using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KeyControl : MonoBehaviour
{
    [SerializeField] GameObject lowC;
    [SerializeField] GameObject lowD;
    [SerializeField] GameObject lowE;
    [SerializeField] GameObject lowF;
    [SerializeField] GameObject lowG;
    [SerializeField] GameObject lowA;
    [SerializeField] GameObject lowB;
    [SerializeField] GameObject lowCs;
    [SerializeField] GameObject lowDs;
    [SerializeField] GameObject lowFs;
    [SerializeField] GameObject lowGs;
    [SerializeField] GameObject lowAs;
    [SerializeField] GameObject CDEF;
    [SerializeField] GameObject FEDC;
    [SerializeField] GameObject FCED;
    [SerializeField] GameObject correctAudioText;
    [SerializeField] GameObject wrongAudioText;
    [SerializeField] GameObject prizeClaimText;
    [SerializeField] GameObject PianoUI;
    [SerializeField] GameObject PlayPianoText;
    int randomIndex;
    int i;
    
    // 2D list to store multiple correct sequences
    private List<List<string>> allSequences = new List<List<string>>
    {
        new List<string> { "C", "D", "E", "F" },
        new List<string> { "F", "E", "D", "C" },
        new List<string> { "C", "E", "D", "F" },
        new List<string> { "F", "C", "E", "D" }
    };

    private List<string> correctSequence; // The selected correct sequence
    private List<string> playerInput = new List<string>(); // Stores player input
    
    void Start()
    {
        SelectRandomSequence();
    }
    public void LowPressC()
    {
        RegisterInput("C");
        lowC.SetActive(false);
        lowC.SetActive(true);
    }
    public void LowPressD()
    {
        RegisterInput("D");
        lowD.SetActive(false);
        lowD.SetActive(true);
    }
    public void LowPressE()
    {
        RegisterInput("E");
        lowE.SetActive(false);
        lowE.SetActive(true);
    }
    public void LowPressF()
    {
        RegisterInput("F");
        lowF.SetActive(false);
        lowF.SetActive(true);
    }
    public void LowPressG()
    {
        RegisterInput("G");
        lowG.SetActive(false);
        lowG.SetActive(true);
    }
    public void LowPressA()
    {
        RegisterInput("A");
        lowA.SetActive(false);
        lowA.SetActive(true);
    }
    public void LowPressB()
    {
        RegisterInput("B");
        lowB.SetActive(false);
        lowB.SetActive(true);
    }

    public void LowPressCs()
    {
        RegisterInput("Cs");
        lowCs.SetActive(false);
        lowCs.SetActive(true);
    }

    public void LowPressDs()
    {
        RegisterInput("Ds");
        lowDs.SetActive(false);
        lowDs.SetActive(true);
    }

    public void LowPressFs()
    {
        RegisterInput("Fs");
        lowFs.SetActive(false);
        lowFs.SetActive(true);
    }

    public void LowPressGs()
    {
        RegisterInput("Gs");
        lowGs.SetActive(false);
        lowGs.SetActive(true);
    }

    public void LowPressAs()
    {
        RegisterInput("As");
        lowAs.SetActive(false);
        lowAs.SetActive(true);
    }

    public void ShowPiano()
    {
        PlayPianoText.SetActive(false);
        PianoUI.SetActive(true);
        PlayCorrectAudio();
    }

    public void HidePiano()
    {
        PianoUI.SetActive(false);
    }

    IEnumerator ShowWrongMessage()
    {
        wrongAudioText.SetActive(true);
        Debug.Log("I am reaching here sequence");
        yield return new WaitForSeconds(2); // Pause for 2 seconds
        wrongAudioText.SetActive(false);
    }

        IEnumerator ShowCorrectMessage()
    {
        correctAudioText.SetActive(true);
        Debug.Log("I am reaching correct msg sequence");
        yield return new WaitForSeconds(2); // Pause for 2 seconds
        correctAudioText.SetActive(false);
        OnPuzzleSolved();
    }

    // Registers the player's input and checks the sequence
    private void RegisterInput(string key)
    {
        playerInput.Add(key);

        // Check if the player's input matches the correct sequence so far
        for (int i = 0; i < playerInput.Count; i++)
        {
            if (playerInput[i] != correctSequence[i])
            {
                Debug.Log("Incorrect sequence! Resetting...");
                StartCoroutine(ShowWrongMessage());
                ResetPuzzle();
                return;
            }
        }

        // If the sequence matches and the player completes it
        if (playerInput.Count == correctSequence.Count)
        {
            Debug.Log("Correct sequence! Puzzle solved!");
            StartCoroutine(ShowCorrectMessage());
        }
    }

    // Selects a random sequence from the 2D list
    private void SelectRandomSequence()
    {
        randomIndex = Random.Range(0, allSequences.Count);
        correctSequence = allSequences[randomIndex];
        Debug.Log("Selected Sequence: " + string.Join(", ", correctSequence));
    }

    // Resets the player's input if they get the sequence wrong
    private void ResetPuzzle()
    {
        playerInput.Clear();
    }
    
// Handles what happens when the puzzle is solved
    private void OnPuzzleSolved()
    {
        // Add behavior for when the puzzle is solved, e.g., unlocking a door
        correctAudioText.SetActive(false);
        prizeClaimText.SetActive(true);
        Debug.Log("The piano opens a secret compartment!");
        // Example: Unlock a reward object or play a success animation
    }

    public void PlayCorrectAudio()
    {
        Debug.Log(randomIndex);
        if (randomIndex == 0)
        {
            ActivateCDEF();
        }
        else if (randomIndex == 1)
        {
            ActivateFEDC();
        }
        else if (randomIndex == 3)
        {
            ActivateFCED();
        }
    }

    public void ActivateCDEF()
    {
        CDEF.SetActive(false);
        CDEF.SetActive(true);
    }

    public void ActivateFEDC()
    {
        FEDC.SetActive(false);
        FEDC.SetActive(true);
    }

    public void ActivateFCED()
    {
        FCED.SetActive(false);
        FCED.SetActive(true);
    }
}
