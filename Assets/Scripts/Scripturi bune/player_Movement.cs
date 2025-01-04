using UnityEngine;

public class player_Movement : MonoBehaviour
{
    public Animator animator;             // Reference to the Animator component
    public Transform cameraTransform;     // Reference to the camera transform
    public float rotationSpeed = 10.0f;   // Smooth rotation speed

    private bool isRolling = false;       // State to check if the character is rolling

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
    }

    void Update()
    {
        HandleMovement();  // Handle movement input
        HandleRoll();      // Handle rolling input
        HandleAttack();    // Handle attack input
    }

    void HandleMovement()
    {
        // Disable movement while rolling
        if (isRolling) return;

        // Get input axes
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = Vector3.zero;

        // Move relative to camera orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f; // Ignore vertical tilt
        right.y = 0f;   // Ignore vertical tilt
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        if (Input.GetKey(KeyCode.W)) // Move forward in camera direction when pressing W
        {
            moveDirection = forward;
        }
        else if (Input.GetKey(KeyCode.A)) // Strafe left
        {
            moveDirection = -right;
        }
        else if (Input.GetKey(KeyCode.D)) // Strafe right
        {
            moveDirection = right;
        }

        // Rotate the character to face movement direction only when pressing W
        if (Input.GetKey(KeyCode.W) && moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update animator parameters for movement
        animator.SetFloat("Front", vertical); // Forward/backward movement
        animator.SetFloat("Right", horizontal); // Sideways movement
        animator.SetFloat("Speed", moveDirection.magnitude); // Blended speed
    }

    void HandleRoll()
    {
        // Trigger roll animation
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling) // Press Space to roll
        {
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
            animator.SetTrigger("LightAttack1"); // Trigger the LightAttack1 animation
        }
    }
}
