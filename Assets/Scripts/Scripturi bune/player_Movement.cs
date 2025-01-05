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
            string rollDirectionName = "";       // Store roll direction name

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
                rollDirectionName = "Forward-Left";
            }
            else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            {
                rollDirection = forward + right; // Roll diagonally forward-right
                rollDirectionName = "Forward-Right";
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            {
                rollDirection = -forward + -right; // Roll diagonally backward-left
                rollDirectionName = "Backward-Left";
            }
            else if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            {
                rollDirection = -forward + right; // Roll diagonally backward-right
                rollDirectionName = "Backward-Right";
            }
            else if (Input.GetKey(KeyCode.W))
            {
                rollDirection = forward; // Roll forward
                rollDirectionName = "Forward";
            }
            else if (Input.GetKey(KeyCode.S))
            {
                rollDirection = -forward; // Roll backward
                rollDirectionName = "Backward";
            }
            else if (Input.GetKey(KeyCode.A))
            {
                rollDirection = -right; // Roll left
                rollDirectionName = "Left";
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rollDirection = right; // Roll right
                rollDirectionName = "Right";
            }

            // Rotate character to face the roll direction
            if (rollDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(rollDirection);
                transform.rotation = targetRotation; // Instantly rotate to roll direction
            }

            // Log the roll direction (for debugging)
            Debug.Log("Rolled " + rollDirectionName);

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
        // Trigger LightAttack1 animation
        if (Input.GetMouseButtonDown(0) && !isRolling) // Left-click for LightAttack1
        {
            isAttacking = true;
            animator.SetTrigger("LightAttack1"); // Trigger the LightAttack1 animation
            Invoke("EndAttack", 1.0f); // Attack duration set to 1 second
        }
    }

    void HandleBlock()
    {
        if (Input.GetMouseButton(1)) // Hold right-click for Block
        {
            isBlocking = true;
            animator.SetBool("Block", true); // Trigger block animation
        }
        else
        {
            isBlocking = false;
            animator.SetBool("Block", false); // Stop block animation
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }
}