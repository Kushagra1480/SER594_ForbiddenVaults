using UnityEngine;
using StarterAssets;

public class BoxController : MonoBehaviour 
{
    [Header("Spawn Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float potionSpawnChance = 0.6f;
    [SerializeField] private float spawnHeight = 1f;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject[] potionPrefabs;
    
    private bool isDestroyed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;
        
        if (other.CompareTag("Player"))
        {
            isDestroyed = true;
            
            // Either spawn a potion or apply debuff
            if (Random.value < potionSpawnChance)
            {
                SpawnRandomPotion();
            }
            else
            {
                ApplyRandomDebuff(other.gameObject);
            }
            
            // Disable the box
            gameObject.SetActive(false);
        }
    }

    private void SpawnRandomPotion()
    {
        if (potionPrefabs == null || potionPrefabs.Length == 0) return;

        // Pick a random potion
        int randomIndex = Random.Range(0, potionPrefabs.Length);
        GameObject potionPrefab = potionPrefabs[randomIndex];

        // Spawn position above the box
        Vector3 spawnPosition = transform.position + Vector3.up * spawnHeight;
        
        // Add small random offset
        Vector3 randomOffset = Random.insideUnitSphere * 0.5f;
        randomOffset.y = 0;
        
        Instantiate(potionPrefab, spawnPosition + randomOffset, Quaternion.identity);
    }

    private void ApplyRandomDebuff(GameObject player)
    {
        var debuffSystem = player.GetComponent<PlayerDebuffSystem>();
        if (debuffSystem == null) return;

        // Randomly choose between slow and shrink debuff
        if (Random.value < 0.5f)
        {
            debuffSystem.ActivateSlowDebuff();
        }
        else
        {
            debuffSystem.ActivateShrinkDebuff();
        }
    }
}