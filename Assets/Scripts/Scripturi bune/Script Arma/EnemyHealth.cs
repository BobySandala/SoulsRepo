using UnityEngine;


public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the enemy
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount; // Reduce health

        Debug.Log($"Enemy took {amount} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        // Add any additional logic for when the enemy dies (e.g., play animation, destroy object, etc.)
        Destroy(gameObject);
    }
}
