using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 0, 100); // Rotation speed in degrees per second

    void Update()
    {
        // Rotate the object based on the rotation speed and deltaTime
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
