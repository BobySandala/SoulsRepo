using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 600; // Maximum health of the enemy
    public int currentHealth;

    private Animator animator;
    private Collider enemyCollider; // Reference to the main collider
    private bool isDead = false;    // Track if the enemy is dead

    void Start()
    {
        currentHealth = maxHealth; // Initialize health

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            //Debug.LogError("Animator component missing on Enemy!");
        }

        enemyCollider = GetComponent<Collider>();
        if (enemyCollider == null)
        {
            //Debug.LogError("Collider component missing on Enemy!");
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Ignore damage if the enemy is already dead

        currentHealth -= amount; // Reduce health

        //Debug.Log($"Enemy took {amount} damage! Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return; // Prevent multiple triggers of the death logic

        isDead = true; // Mark the enemy as dead
        //Debug.Log("Enemy died!");

        // Trigger the die animation
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Disable functionality of the enemy after dying
        StartCoroutine(DisableEnemy());
    }

    private IEnumerator DisableEnemy()
    {
        // Wait for the animation to complete before disabling functionality
        yield return new WaitForSeconds(2f); // Adjust time based on animation length

        // Disable the collider
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Disable this script to stop processing further logic
        this.enabled = false;

        // Optionally disable other components like movement
        EnemyController enemyController = GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        //Debug.Log("Enemy functionality disabled after death.");
    }
}