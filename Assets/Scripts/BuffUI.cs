using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class BuffUI : MonoBehaviour
{
    [Header("Buff UI GameObjects")]
    public GameObject speedBuffIcon;
    public GameObject possessionBuffIcon; // Icon for possession buff
    public GameObject strengthBuffIcon;

    [Header("External References")]
    public PlayerBuffSystem playerBuffSystem; // Handles speed and strength buffs
    public PossessionSystem possessionSystem;
    public Timer possessionTimer; // Timer for the possession buff

    void Update()
    {
        // Handle buff icons visibility
        HandleBuffIcons();
    }

    private void HandleBuffIcons()
    {
        // Speed Buff
        if (playerBuffSystem._speedJumpBuffTimer > 0)
        {
            if (speedBuffIcon.activeSelf == false)
            {
                StartPossessionBuff((int)playerBuffSystem._speedJumpBuffTimer);
                speedBuffIcon.SetActive(true);
                Debug.Log("Speed Buff activated. Timer: " + playerBuffSystem._speedJumpBuffTimer);
            }
            else
            {
                Debug.Log("Speed Buff already active.");
            }
        }
        

        // Strength Buff
        if (playerBuffSystem._strengthBuffTimer > 0)
        {
            if (strengthBuffIcon.activeSelf == false)
            {
                StartPossessionBuff((int)playerBuffSystem._strengthBuffTimer);
                strengthBuffIcon.SetActive(true);
                Debug.Log("Strength Buff activated. Timer: " + playerBuffSystem._strengthBuffTimer);
            }
            else
            {
                Debug.Log("Strength Buff already active.");
            }
        }
        


        // Possession Buff
        if (possessionSystem._possessionTimer > 0)
        {
            if (possessionBuffIcon.activeSelf == false)
            {
                StartPossessionBuff((int)possessionSystem._possessionTimer);
                possessionBuffIcon.SetActive(true);
                Debug.Log("Possession Buff activated. Timer: " + possessionSystem._possessionTimer);
            }
            else
            {
                Debug.Log("Possession Buff already active.");
            }
        }
        if (playerBuffSystem._speedJumpBuffTimer < 1)
        {
            speedBuffIcon.SetActive(false);
        }
        if (playerBuffSystem._strengthBuffTimer < 1)
        {
            strengthBuffIcon.SetActive(false);

        }
        if (possessionSystem._possessionTimer < 1)
        {
            possessionBuffIcon.SetActive(false);
        }

    }
    public void ZeroChecker()
    {
        if (playerBuffSystem._speedJumpBuffTimer == 0)
        {
            speedBuffIcon.SetActive(false);
        }
        if (playerBuffSystem._strengthBuffTimer == 0)
        {
            strengthBuffIcon.SetActive(false);

        }
        if (possessionSystem._possessionTimer == 0)
        {
            possessionBuffIcon.SetActive(false);
        }
    }

    public void StartPossessionBuff(int durationInSeconds)
    {
        // Configure the timer for possession
        possessionTimer.hours = 0;
        possessionTimer.minutes = durationInSeconds / 60;
        possessionTimer.seconds = durationInSeconds % 60;

        possessionTimer.countMethod = Timer.CountMethod.CountDown;
        possessionTimer.StartTimer();
        Debug.Log("Possession Buff timer started for " + durationInSeconds + " seconds.");
    }
}
