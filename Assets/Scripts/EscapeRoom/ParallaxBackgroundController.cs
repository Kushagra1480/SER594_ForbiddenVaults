using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ParallaxBackgroundController : MonoBehaviour
{
    [Header("Effect Settings")]
    [SerializeField, Range(0, 1)] private float depthEffect = 0.1f;
    [SerializeField, Range(0, 1)] private float distortionStrength = 0.1f;
    [SerializeField, Range(0, 1)] private float fogStrength = 0.5f;
    [SerializeField] private Color fogColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private float moveSpeed = 0.1f;
    
    private Material material;
    private Vector3 initialPosition;
    private Transform playerCamera;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
        initialPosition = transform.position;
        playerCamera = Camera.main.transform;
        
        UpdateMaterialProperties();
    }

    private void Update()
    {
        if (playerCamera != null)
        {
            // Add subtle movement based on camera position
            Vector3 cameraOffset = playerCamera.position - initialPosition;
            Vector3 newPosition = initialPosition + new Vector3(
                cameraOffset.x * moveSpeed,
                cameraOffset.y * moveSpeed * 0.5f,
                0
            );
            
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
        }
    }

    private void UpdateMaterialProperties()
    {
        if (material != null)
        {
            material.SetFloat("_Depth", depthEffect);
            material.SetFloat("_DistortionStrength", distortionStrength);
            material.SetFloat("_FogStrength", fogStrength);
            material.SetColor("_FogColor", fogColor);
        }
    }

    private void OnValidate()
    {
        UpdateMaterialProperties();
    }
}   