using UnityEngine;
using StarterAssets;

public class BuffPickup : MonoBehaviour
{
    public enum BuffType
    {
        SpeedAndJump,
        Strength,
        Possession
    }

    public BuffType buffType;
    public float rotationSpeed = 50f;
    public bool debugMode = true;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugMode) Debug.Log($"Trigger entered by: {other.gameObject.name}");

        if (other.CompareTag("Player"))
        {
            if (debugMode) Debug.Log("Player entered trigger");

            switch (buffType)
            {
                case BuffType.Possession:
                    var possessionSystem = other.GetComponent<PossessionSystem>();
                    if (possessionSystem != null)
                    {
                        possessionSystem.ActivatePossessionBuff();
                        if (debugMode) Debug.Log("Activated possession buff");
                    }
                    break;

                default:
                    var buffSystem = other.GetComponent<PlayerBuffSystem>();
                    if (buffSystem != null)
                    {
                        if (buffType == BuffType.SpeedAndJump)
                            buffSystem.ActivateSpeedAndJumpBuff();
                        else if (buffType == BuffType.Strength)
                            buffSystem.ActivateStrengthBuff();
                    }
                    break;
            }
            
            Destroy(gameObject);
        }
    }
}