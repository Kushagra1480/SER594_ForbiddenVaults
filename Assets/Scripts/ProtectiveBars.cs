using UnityEngine;
using TMPro;
using System.Collections;
using StarterAssets;

public class ProtectiveBars : MonoBehaviour
{
    [SerializeField] private float lowerDistance = 2f;
    [SerializeField] private float lowerSpeed = 1f;
    [SerializeField] private AudioSource barMovementSound;

    private Vector3 startPosition;
    private bool isLowering = false;
    private bool isLowered = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isLowering)
        {
            float step = lowerSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(
                transform.position, 
                startPosition + Vector3.down * lowerDistance, 
                step
            );

            if (Vector3.Distance(transform.position, startPosition + Vector3.down * lowerDistance) < 0.001f)
            {
                isLowering = false;
                isLowered = true;
            }
        }
    }

    public void LowerBars()
    {
        if (!isLowered && !isLowering)
        {
            isLowering = true;
            if (barMovementSound != null)
            {
                barMovementSound.Play();
            }
        }
    }

    public bool IsLowered => isLowered;
}