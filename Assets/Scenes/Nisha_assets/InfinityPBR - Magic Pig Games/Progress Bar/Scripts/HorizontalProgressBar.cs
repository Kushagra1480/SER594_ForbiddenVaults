using System;
using UnityEngine;
using UnityEngine.UI;

namespace MagicPigGames
{
    [Serializable]
    public class HorizontalProgressBar : ProgressBar
    {
        public RawImage fillImage; // Reference to the FillImage RawImage
        public RawImage overlayImage; // Reference to the OverlayImage RawImage
        public Color greenColor = Color.green; // Color for health > 1
        public Color redColor = Color.red;     // Color for health == 1

        // Method to update the health color for both fill and overlay images
        public void UpdateHealthColor(int currentHealth)
        {
            // Update FillImage color
            if (fillImage != null)
            {
                Debug.Log($"Updating FillImage color. Current Health: {currentHealth}");

                if (currentHealth <= 1)
                {
                    fillImage.color = redColor;
                    Debug.Log("FillImage: Health is low. Changing color to red.");
                }
                else
                {
                    fillImage.color = greenColor;
                    Debug.Log("FillImage: Health is sufficient. Changing color to green.");
                }
            }
            else
            {
                Debug.LogWarning("FillImage is not assigned in the Progress Bar.");
            }

            // Update OverlayImage color
            if (overlayImage != null)
            {
                Debug.Log($"Updating OverlayImage color. Current Health: {currentHealth}");

                if (currentHealth <= 1)
                {
                    overlayImage.color = redColor;
                    Debug.Log("OverlayImage: Health is low. Changing color to red.");
                }
                else
                {
                    overlayImage.color = greenColor;
                    Debug.Log("OverlayImage: Health is sufficient. Changing color to green.");
                }
            }
            else
            {
                Debug.LogWarning("OverlayImage is not assigned in the Progress Bar.");
            }
        }
    }
}
