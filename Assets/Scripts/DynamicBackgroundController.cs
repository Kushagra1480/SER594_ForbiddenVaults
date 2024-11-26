using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class DynamicBackgroundController : MonoBehaviour
{
    [Header("Distortion Settings")]
    [SerializeField] private float distortionStrength = 0.02f;
    [SerializeField] private float distortionSpeed = 0.5f;
    [SerializeField] [Range(0, 1)] private float parallaxStrength = 0.02f;
    
    [Header("Fog Settings")]
    [SerializeField] private Color fogColor = new Color(0.1f, 0.1f, 0.15f, 1f);
    [SerializeField] [Range(0, 1)] private float fogStrength = 0.5f;
    [SerializeField] private float depthOffset = 0.1f;

    [Header("Dynamic Settings")]
    [SerializeField] private float playerDistanceInfluence = 1f;
    [SerializeField] private float breathingEffect = 0.2f;
    [SerializeField] private float breathingSpeed = 1f;

    private Material material;
    private Transform playerTransform;
    private Vector3 initialPosition;
    private float initialDistortion;

    private void Start()
    {
        // Get or create material instance
        material = GetComponent<Renderer>().material;
        initialDistortion = distortionStrength;
        initialPosition = transform.position;
        
        // Find player
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Set initial shader properties
        UpdateShaderProperties();
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            // Calculate distance-based effects
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            float distanceFactor = Mathf.Clamp01(distanceToPlayer / playerDistanceInfluence);

            // Add subtle breathing motion
            float breathing = Mathf.Sin(Time.time * breathingSpeed) * breathingEffect;
            
            // Update distortion based on distance and breathing
            float currentDistortion = initialDistortion * (1 + distanceFactor + breathing);
            material.SetFloat("_DistortStrength", currentDistortion);

            // Add subtle position offset based on player movement
            Vector3 offset = (playerTransform.position - initialPosition) * parallaxStrength * 0.1f;
            transform.position = initialPosition + new Vector3(offset.x, offset.y, 0);

            // Update fog based on distance
            material.SetFloat("_FogStrength", fogStrength * (1 + distanceFactor * 0.5f));
        }
    }

    private void UpdateShaderProperties()
    {
        if (material != null)
        {
            material.SetFloat("_DistortStrength", distortionStrength);
            material.SetFloat("_DistortSpeed", distortionSpeed);
            material.SetFloat("_ParallaxStrength", parallaxStrength);
            material.SetColor("_FogColor", fogColor);
            material.SetFloat("_FogStrength", fogStrength);
            material.SetFloat("_DepthOffset", depthOffset);
        }
    }

    private void OnValidate()
    {
        UpdateShaderProperties();
    }
}