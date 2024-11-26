using UnityEngine;

public class Cabinet : MonoBehaviour
{
    [Header("Cabinet Settings")]
    [SerializeField] private float openDistance = 1f;
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private CollectibleItem hammerObject;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    public bool isOpening = false;
    private bool isOpen = false;

    private void Start() {
        startPosition = transform.position;
        targetPosition = startPosition + new Vector3(openDistance, 0, 0);
        hammerObject.enabled = false;
    }

    private void Update()
    {
        if (isOpening)
        {
            float step = openSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
            
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isOpening = false;
                isOpen = true;
            }
        }
    }

    public void Open()
    {
        if (!isOpen) {
            isOpening = true;
        }
        hammerObject.enabled = true;
    }
    public bool IsOpen()
    {
        return isOpen;
    }
}