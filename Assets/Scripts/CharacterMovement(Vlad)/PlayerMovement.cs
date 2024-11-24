using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;

    public float speed = 5f;              // Movement speed
    public float gravity = -9.81f;        // Gravity force
    public float jumpHeight = 1.5f;       // Jump height
    public float rollSpeed = 10f;         // Speed of the roll
    public float rollDuration = 0.5f;     // Duration of the roll in seconds
    public Transform cameraTransform;    // Reference to the camera

    private CharacterController controller;
    private Vector3 velocity;             // Current vertical velocity of the player
    private Vector3 moveDirection;        // Current horizontal movement direction
    private bool isGrounded;              // Check if the player is grounded
    private bool isRolling;               // Check if the player is currently rolling
    private float rollTimer;              // Timer to track roll duration

    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if the player is grounded
        isGrounded = controller.isGrounded;

        if (isRolling)
        {
            // Handle rolling logic
            HandleRoll();
        }
        else
        {
            if (isGrounded)
            {
                HandleMovement();

                // Reset vertical velocity when grounded
                if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
            else
            {
                // Preserve momentum while airborne
                controller.Move(moveDirection * Time.deltaTime);
            }

            // Handle jump input
            if (Input.GetKeyDown(KeyCode.X) && isGrounded)
            {
                Jump();
            }

            // Handle roll (dash) input
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                StartRoll();
            }
        }

        // Always apply gravity
        ApplyGravity();
    }

    void HandleMovement()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D for strafing
        float vertical = Input.GetAxis("Vertical");     // W/S for forward/backward

        // Calculate movement direction relative to the camera
        Vector3 inputDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        inputDirection.y = 0f; // Prevent vertical movement

        // Normalize direction and apply speed and toggle animation
        if (inputDirection.magnitude > 0.1f)
        {
            animator.SetBool("IDLE_TO_WALK_FORWARD", true);
            moveDirection = inputDirection.normalized * speed;
            controller.Move(moveDirection * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IDLE_TO_WALK_FORWARD", false);
            moveDirection = Vector3.zero; // Stop movement if no input
        }
    }

    void ApplyGravity()
    {
        // Apply gravity to vertical velocity
        velocity.y += gravity * Time.deltaTime;

        // Apply the velocity to the character
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        // Perform jump by setting vertical velocity
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        if (moveDirection.magnitude > 0.1f) // Player is moving
        {
            Debug.Log("jump");
            animator.SetTrigger("MOVE_JUMP");
        }
        else
        {
            animator.SetTrigger("IDLE_JUMP");
        }
    }

    void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        // Scale movement direction to roll speed
        moveDirection = moveDirection.normalized * rollSpeed;
    }

    void HandleRoll()
    {
        // Move the player in the roll direction
        controller.Move(moveDirection * Time.deltaTime);

        // Decrease the roll timer
        rollTimer -= Time.deltaTime;

        // End the roll when the timer expires
        if (rollTimer <= 0f)
        {
            isRolling = false;
        }
    }
}
