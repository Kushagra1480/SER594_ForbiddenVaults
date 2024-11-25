using UnityEngine;

public class Hazard : MonoBehaviour
{
    public int damageAmount = 1; // Damage the hazard deals to the player

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
            Debug.Log("Player hit a hazard! Took " + damageAmount + " damage.");
        }
    }
}
