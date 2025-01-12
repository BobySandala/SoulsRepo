using System.Collections; // Required for IEnumerator and coroutines
using System.Collections.Generic;
using UnityEngine;


public class player_Movement : MonoBehaviour
{
    // Variables
    public Animator animator;             // Reference to the Animator component
    public Transform cameraTransform;     // Reference to the camera transform
    public float rotationSpeed = 10.0f;   // Smooth rotation speed

    private bool isRolling = false;       // State to check if the character is rolling
    private bool isFacingBackward = false; // State to check if moving backward
    public bool isAttacking = false;     // State to check if the character is attacking
    private bool isBlocking = false;      // State to check if the character is blocking
    private bool isJumping = false; // Tracks if the player is currently jumping

    public GameObject estusFlaskPrefab; // Reference to the Estus Flask Variant prefab
    private bool isDrinking = false;    // State to track if the player is drinking
    public GameObject hipEstus;

    public AudioClip nomnomnom;

    void Start()
    {
        // Assign the Animator component if not set
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Default to the main camera if no camera transform is assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Add Rigidbody component if not already attached
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // Freeze rotation to prevent physics-based rotation
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Ensure gravity is enabled and Rigidbody behaves as dynamic
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    void Update()
    {
        HandleMovement();  // Handle movement input
        HandleRoll();      // Handle rolling input
        HandleAttack();    // Handle attack input
        HandleBlock();     // Handle blocking input
        HandleJump();      // Handle jump input
        HandleDrink();     // Handle drinking input
    }

    void HandleJump()
    {
        PlayerData playerData = GetComponent<PlayerData>(); // Reference to PlayerData
        int jumpStaminaCost = playerData.jumpStaminaCost;

        if (isRolling || isAttacking || isBlocking || isJumping) return;

        if (Input.GetKeyDown(KeyCode.X) && playerData.stamina >= jumpStaminaCost) // Standard Jump
        {
            playerData.ConsumeStamina(jumpStaminaCost); // Consume stamina
            Debug.Log("Jump started.");
            isJumping = true;
            animator.SetTrigger("Jump");
            Invoke("EndJump", 1.0f); // Adjust duration if needed
        }
    }

    // Resets the jumping state
    void EndJump()
    {
        isJumping = false; // Reset jumping state
        Debug.Log("Jump ended.");
        animator.ResetTrigger("Jump"); // Clear jump trigger
    }

    void HandleMovement()
    {
        PlayerData playerData = GetComponent<PlayerData>();
        if (playerData != null && playerData.state == 1) // If the player is sitting
        {
            // Allow camera to rotate but disable character movement and rotation
            Debug.Log("Player is sitting. Character movement and rotation disabled.");
            return;
        }

        // Existing movement and rotation logic
        if (isRolling || isAttacking || isBlocking) return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection = forward;
            isFacingBackward = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveDirection = isFacingBackward ? right : -right;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveDirection = isFacingBackward ? -right : right;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection = -forward;
            isFacingBackward = true;
            Quaternion targetRotation = Quaternion.LookRotation(-forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) && moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        animator.SetFloat("Front", vertical);
        animator.SetFloat("Right", horizontal * (isFacingBackward ? -1 : 1));
        animator.SetFloat("Speed", moveDirection.magnitude);
    }



    void HandleRoll()
    {
        PlayerData playerData = GetComponent<PlayerData>(); // Reference to PlayerData

        int rollStaminaCost = playerData.rollStaminaCost; // Retrieve stamina cost

        if (Input.GetKeyDown(KeyCode.Space) && !isRolling) // Press Space to roll
        {
            if (playerData.stamina >= rollStaminaCost) // Check if enough stamina
            {
                playerData.ConsumeStamina(rollStaminaCost); // Consume stamina

                Vector3 rollDirection = Vector3.zero; // Default roll direction
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f; // Ignore vertical tilt
                right.y = 0f;   // Ignore vertical tilt
                forward.Normalize();
                right.Normalize();

                if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
                {
                    rollDirection = forward + -right; // Roll diagonally forward-left
                }
                else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
                {
                    rollDirection = forward + right; // Roll diagonally forward-right
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
                {
                    rollDirection = -forward + -right; // Roll diagonally backward-left
                }
                else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
                {
                    rollDirection = -forward + right; // Roll diagonally backward-right
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    rollDirection = forward; // Roll forward
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    rollDirection = -forward; // Roll backward
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    rollDirection = -right; // Roll left
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    rollDirection = right; // Roll right
                }

                if (rollDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
                    transform.rotation = targetRotation;
                }

                isRolling = true;
                playerData.SetInvulnerability(true); // Enable invulnerability
                Debug.Log("Player started rolling. Invulnerability enabled.");
                animator.SetBool("Roll", true); // Trigger roll animation
                Invoke("EndRoll", 1.0f); // Roll duration set to 1 second
            }
            else
            {
                Debug.Log("Not enough stamina to roll.");
            }
        }
    }

    void EndRoll()
    {
        isRolling = false;
        GetComponent<PlayerData>().SetInvulnerability(false); // Disable invulnerability
        Debug.Log("Player finished rolling. Invulnerability disabled.");
        animator.SetBool("Roll", false); // End roll animation
    }

    void HandleAttack()
    {
        
        PlayerData playerData = GetComponent<PlayerData>(); // Reference to PlayerData

        int lightAttackStaminaCost = playerData.lightAttackStaminaCost;
        int heavyAttackStaminaCost = playerData.heavyAttackStaminaCost;

        // Original attack logic remains
        if (isRolling || isAttacking) return;

        if (Input.GetMouseButtonDown(0)) // Left Mouse Button for Light Attack
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // Heavy Attack
            {
                if (playerData.stamina >= heavyAttackStaminaCost)
                {
                    playerData.ConsumeStamina(heavyAttackStaminaCost); // Consume stamina
                    Debug.Log("Heavy Attack started.");
                    if (!isAttacking)
                    {
                        GetComponent<AudioSelector>().PlayToporSwing();
                    }
                    isAttacking = true;
                    animator.SetTrigger("Heavy");
                    Invoke("EndAttack", 1.5f); // Adjust duration if needed
                }
                else
                {
                    Debug.Log("Not enough stamina for Heavy Attack.");
                }
            }
            else // Light Attack
            {
                if (playerData.stamina >= lightAttackStaminaCost)
                {
                    playerData.ConsumeStamina(lightAttackStaminaCost); // Consume stamina
                    Debug.Log("Light Attack started.");
                    if (!isAttacking)
                    {
                        GetComponent<AudioSelector>().PlayToporSwing();
                    }
                    isAttacking = true;
                    animator.SetTrigger("LightAttack1");
                    Invoke("EndAttack", 1.0f); // Adjust duration if needed
                }
                else
                {
                    Debug.Log("Not enough stamina for Light Attack.");
                }
            }
        }
    }

    void HandleBlock()
    {
        if (Input.GetMouseButton(1)) // Hold right-click for Block
        {
            if (!isBlocking) // Start blocking only if not already blocking
            {
                isBlocking = true;
                animator.SetBool("Block", true); // Trigger block animation
            }

            // Pause the animation at its current frame
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Block"))
            {
                animator.speed = 0f; // Pause animation at the current frame
            }
        }
        else
        {
            if (isBlocking) // Stop blocking only if currently blocking
            {
                isBlocking = false;
                animator.SetBool("Block", false); // Stop block animation
                animator.speed = 1f; // Reset animation speed for smooth playback next time
            }
        }
    }

    void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Attack ended.");
    }

    void HandleDrink()
    {
        // Prevent drinking if already drinking, rolling, attacking, or blocking
        if (isDrinking || isRolling || isAttacking || isBlocking || isJumping) return;

        PlayerData playerData = GetComponent<PlayerData>(); // Reference to PlayerData

        // Detect 'F' key press
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<AudioSource>().PlayOneShot(nomnomnom);
            // Start drinking action
            isDrinking = true; // Set drinking state
            animator.SetTrigger("Drink"); // Trigger Drink animation

            if (playerData != null)
            {
                int totalHealthToRestore = playerData.maxHealth / 2; // Half of max health
                float restoreDuration = 2.0f; // Duration over which health is restored
                StartCoroutine(GradualHeal(playerData, totalHealthToRestore, restoreDuration));
            }

            // Deactivate Hip_Estus
            if (hipEstus != null)
            {
                hipEstus.SetActive(false); // Hide Hip_Estus
            }

            // Activate the Estus Flask prefab
            estusFlaskPrefab.SetActive(true); // Show Estus Flask Variant during animation

            // Schedule to end the drinking animation (adjust timing to match your animation length)
            Invoke("EndDrink", 2.0f); // Animation duration: 2 seconds
        }
    }

    IEnumerator GradualHeal(PlayerData playerData, int totalHealthToRestore, float duration)
    {
        float elapsedTime = 0f;
        float initialHealth = playerData.health;
        float targetHealth = Mathf.Clamp(initialHealth + totalHealthToRestore, 0, playerData.maxHealth);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentHealth = Mathf.Lerp(initialHealth, targetHealth, elapsedTime / duration);
            playerData.health = Mathf.Clamp((int)currentHealth, 0, playerData.maxHealth);

            // Update the health bar
            if (playerData.healthBar != null)
            {
                playerData.healthBar.SetHealth(playerData.health);
            }

            yield return null;
        }

        // Ensure the final health value is set
        playerData.health = (int)targetHealth;
        if (playerData.healthBar != null)
        {
            playerData.healthBar.SetHealth(playerData.health);
        }
    }


    void EndDrink()
    {
        // Reset the drinking state
        isDrinking = false;

        // Reactivate the Hip_Estus
        if (hipEstus != null)
        {
            hipEstus.SetActive(true); // Show Hip_Estus again
        }

        // Deactivate the Estus Flask prefab
        estusFlaskPrefab.SetActive(false); // Hide Estus Flask Variant after animation ends
    }
}