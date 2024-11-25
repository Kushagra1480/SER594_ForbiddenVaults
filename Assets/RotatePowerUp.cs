using UnityEngine;

public class RotatePowerUp : MonoBehaviour
{
    public float rotationSpeed = 100f; // Speed of rotation

    void Update()
    {
        // Rotate the object along the Y-axis
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
