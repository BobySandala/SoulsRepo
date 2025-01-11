using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int baseDamage = 10; // Base damage amount for light attack
    public string enemyTag = "Enemy"; // Tag to identify enemy objects

    private player_Movement playerMovement; // Reference to the player's movement script

    void Start()
    {
        // Find the player_Movement script on the parent object
        playerMovement = GetComponentInParent<player_Movement>();
        if (playerMovement == null)
        {
            Debug.LogError("player_Movement script not found on the parent object!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is attacking
        if (playerMovement != null && playerMovement.isAttacking)
        {
            Debug.Log("Player is attacking, checking for enemy contact...");

            // Check if the object has the enemy tag
            if (other.CompareTag(enemyTag))
            {
                Debug.Log($"Weapon hit an enemy: {other.name}");

                // Try to find a Health component on the enemy
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // Determine the damage based on the attack type
                    int damageAmount = CalculateDamage();

                    // Apply damage to the enemy
                    Debug.Log($"Dealing {damageAmount} damage to {other.name}");
                    enemyHealth.TakeDamage(damageAmount);
                }
                else
                {
                    Debug.Log($"Enemy {other.name} does not have a health component!");
                }
            }
            else
            {
                Debug.Log($"Contact occurred with {other.name}, but it is not tagged as an enemy.");
            }
        }
        else
        {
            Debug.Log("Player is not attacking. No damage applied.");
        }
    }

    private int CalculateDamage()
    {
        // Calculate damage based on the type of attack
        int damage = baseDamage; // Default to light attack damage

        if (playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("Heavy"))
        {
            damage *= 2; // Heavy attack does double damage
        }
        else if (playerMovement.animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
        {
            damage *= 3; // Jump attack does triple damage
        }

        return damage;
    }
}
