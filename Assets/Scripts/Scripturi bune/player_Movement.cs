using UnityEngine;

public class player_Movement : MonoBehaviour
{
    // Variables
    public Animator animator;             // Reference to the Animator component
    public Transform cameraTransform;     // Reference to the camera transform
    public float rotationSpeed = 10.0f;   // Smooth rotation speed

    private bool isRolling = false;       // State to check if the character is rolling
    private bool isFacingBackward = false; // State to check if moving backward
    private bool isAttacking = false;     // State to check if the character is attacking
    private bool isBlocking = false;      // State to check if the character is blocking
    private bool isJumping = false; // Tracks if the player is currently jumping

    public GameObject estusFlaskPrefab; // Reference to the Estus Flask Variant prefab
    private bool isDrinking = false;    // State to track if the player is drinking
    public GameObject hipEstus;



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
        HandleJump();      // Handle jump and jump attack input
        HandleDrink();     // Handle drinking input
    }


    void HandleJump()
    {
        // Prevent jumping if already rolling, attacking, or blocking
        if (isRolling || isAttacking || isBlocking || isJumping) return;

        // Check if 'Ctrl' is pressed for JumpAttack
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) // Ctrl Key
        {
            // Trigger JumpAttack
            isAttacking = true; // Set attacking state
            isJumping = true;   // Set jumping state
            animator.SetTrigger("JumpAttack"); // Trigger JumpAttack animation
            Invoke("EndJump", 1.2f); // Adjust attack duration if needed
        }
        else if (Input.GetKeyDown(KeyCode.X)) // Single X press for Jump
        {
            // Trigger Jump
            isJumping = true; // Set jumping state
            animator.SetTrigger("Jump"); // Trigger Jump animation
            Invoke("EndJump", 1.0f); // Adjust jump duration if needed
        }
    }

    // Resets the jumping and attacking state
    void EndJump()
    {
        isJumping = false; // Reset jumping state
        isAttacking = false; // Reset attacking state

        // Clear animator triggers to avoid continuous activation
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("JumpAttack");
    }




    void HandleMovement()
    {
        // Disable movement while rolling, attacking, or blocking
        if (isRolling || isAttacking || isBlocking) return;

        // Get input axes
        float horizontal = Input.GetAxis("Horizontal"); // A/D input (-1 to 1)
        float vertical = Input.GetAxis("Vertical");     // W/S input (-1 to 1)

        // Get camera-relative directions
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // Ignore vertical tilt
        right.y = 0f;   // Ignore vertical tilt
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = Vector3.zero;

        // Movement controls
        if (Input.GetKey(KeyCode.W)) // Move forward
        {
            moveDirection = forward;
            isFacingBackward = false;
        }
        else if (Input.GetKey(KeyCode.A)) // Strafe left
        {
            moveDirection = isFacingBackward ? right : -right;
        }
        else if (Input.GetKey(KeyCode.D)) // Strafe right
        {
            moveDirection = isFacingBackward ? -right : right;
        }
        else if (Input.GetKey(KeyCode.S)) // Move backward toward camera
        {
            moveDirection = -forward;
            isFacingBackward = true;
            Quaternion targetRotation = Quaternion.LookRotation(-forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Rotate character to face forward direction only
        if (Input.GetKey(KeyCode.W) && moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update animator parameters for movement
        animator.SetFloat("Front", vertical); // Forward/backward movement
        animator.SetFloat("Right", horizontal * (isFacingBackward ? -1 : 1)); // Sideways movement (invert if backward)
        animator.SetFloat("Speed", moveDirection.magnitude); // Blended speed
    }

    void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling) // Press Space to roll
        {
            Vector3 rollDirection = Vector3.zero; // Default roll direction

            // Calculate movement directions relative to the camera
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f; // Ignore vertical tilt
            right.y = 0f;   // Ignore vertical tilt
            forward.Normalize();
            right.Normalize();

            // Determine roll direction based on input keys
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

            // Rotate character to face the roll direction
            if (rollDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
                transform.rotation = targetRotation; // Instantly rotate to roll direction
            }

            isRolling = true;
            animator.SetBool("Roll", true); // Trigger roll animation
            Invoke("EndRoll", 1.0f); // Roll duration set to 1 second
        }
    }


    void EndRoll()
    {
        isRolling = false;
        animator.SetBool("Roll", false); // End roll animation
    }

    void HandleAttack()
    {
        // Check if the player is already performing an action
        if (isRolling || isAttacking) return;

        // Detect Shift + Left Click for Heavy Attack
        if (Input.GetMouseButtonDown(0)) // Left Mouse Button
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) // Check for Shift key
            {
                // Trigger Heavy Attack
                isAttacking = true;
                animator.SetTrigger("Heavy"); // Trigger Heavy Attack animation
                Invoke("EndAttack", 1.5f); // Adjust duration if needed
            }
            else
            {
                // Trigger Light Attack
                isAttacking = true;
                animator.SetTrigger("LightAttack1"); // Trigger Light Attack animation
                Invoke("EndAttack", 1.0f); // Adjust duration if needed
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
    }

    void HandleDrink()
    {
        // Prevent drinking if already drinking, rolling, attacking, or blocking
        if (isDrinking || isRolling || isAttacking || isBlocking || isJumping) return;

        // Detect 'F' key press
        if (Input.GetKeyDown(KeyCode.F))
        {
            // Start drinking action
            isDrinking = true; // Set drinking state
            animator.SetTrigger("Drink"); // Trigger Drink animation

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