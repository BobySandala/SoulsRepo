using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player; // Assign the player GameObject in the Inspector
    public Collider swordCollider; // Reference to the sword's collider
    public float activationRange = 15.0f; // Range within which the enemy becomes active
    public float attackRange = 2.0f; // Range within which the enemy attacks
    public float chaseRange = 10.0f; // Range within which the enemy chases the player
    public float attackCooldown = 2.0f; // Cooldown time between attacks
    public float turnSpeed = 5.0f; // Speed at which the enemy turns to face the player
    public float rightOffsetAngle = 15.0f; // Angle to offset the aim to the right

    private Animator animator;
    private float lastAttackTime;
    private bool isActive = false;
    private string currentAttack; // Track which attack is being used

    private float initialYPosition; // To lock the enemy's Y position

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator.runtimeAnimatorController.name != "EnemyAnimator")
        {
            Debug.LogError("Assigned Animator is not 'EnemyAnimator'. Please check the Animator Controller.");
        }

        // Ensure root motion is applied
        animator.applyRootMotion = true;

        // Disable sword collider initially
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }

        // Save the initial Y position to lock it during attacks
        initialYPosition = transform.position.y;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isActive && distanceToPlayer <= activationRange)
        {
            isActive = true;
            animator.SetFloat("Speed", 0); // Start in idle state
        }

        if (isActive)
        {
            if (distanceToPlayer <= attackRange)
            {
                // Attack the player if in range
                animator.SetFloat("Speed", 0); // Stop movement
                FacePlayerWithOffset();       // Ensure the enemy faces the player
                AttackPlayer();
            }
            else if (distanceToPlayer <= chaseRange)
            {
                // Chase the player using root motion
                FacePlayerWithOffset();       // Rotate toward the player
                animator.SetFloat("Speed", 1); // Trigger movement animation
            }
            else
            {
                // Idle if the player is out of range
                animator.SetFloat("Speed", 0);
            }
        }

        // Lock the enemy's Y position to prevent climbing
        LockYPosition();
    }

    void FacePlayerWithOffset()
    {
        // Calculate direction to the player
        Vector3 direction = (player.position - transform.position).normalized;

        // Offset the direction slightly to the right
        Vector3 rightOffset = Quaternion.Euler(0, rightOffsetAngle, 0) * direction;

        // Smoothly rotate the enemy to face the offset direction
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(rightOffset.x, 0, rightOffset.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    void LockYPosition()
    {
        // Keep the enemy's Y position consistent while allowing horizontal movement
        transform.position = new Vector3(transform.position.x, initialYPosition, transform.position.z);
    }

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;

            // Trigger attack animations
            int attackType = Random.Range(0, 2); // Randomly choose between Slash and Slash1

            if (attackType == 0)
            {
                currentAttack = "Slash"; // Set current attack
                animator.SetTrigger("Slash");
            }
            else
            {
                currentAttack = "Slash1"; // Set current attack
                animator.SetTrigger("Slash1");
            }
        }
    }

    // Called from animation event to enable sword collider
    public void EnableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = true;
        }
    }

    // Called from animation event to disable sword collider
    public void DisableSwordCollider()
    {
        if (swordCollider != null)
        {
            swordCollider.enabled = false;
        }
    }

    // Called from the sword's collision event to deal damage
    public void DealDamage(PlayerData playerData)
    {
        if (playerData != null)
        {
            int damageAmount = 0;

            // Determine damage based on the current attack
            if (currentAttack == "Slash")
            {
                damageAmount = 20; // Damage for Slash
            }
            else if (currentAttack == "Slash1")
            {
                damageAmount = 40; // Damage for Slash1
            }

            playerData.TakeDamage(damageAmount);
            Debug.Log($"Player hit by {currentAttack}. Damage: {damageAmount}");
        }
    }
}