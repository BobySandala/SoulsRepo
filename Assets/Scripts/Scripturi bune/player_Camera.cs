using UnityEngine;

public class player_Camera : MonoBehaviour
{
    public Transform player;               // Player to follow
    public float distance = 10.0f;         // Desired fixed distance from the player
    public float minDistance = 2.0f;       // Minimum distance to maintain from the player
    public float rotationSpeed = 5.0f;     // Speed of rotation
    public float collisionOffset = 0.2f;   // Offset to prevent clipping
    public float heightOffset = 2.0f;      // Height offset to keep the camera slightly above the player
    public float lockOnHeightOffset = 4.0f; // Higher height offset when locked onto an enemy
    public float collisionRadius = 0.5f;   // Radius for SphereCast to detect thin obstacles
    public LayerMask collisionLayers;      // Layers to check for collisions
    public Transform target;               // Current lock-on target

    private Vector3 offset;                // Offset based on distance
    private float yaw = 0.0f;              // Horizontal rotation
    private float pitch = 0.0f;            // Vertical rotation
    private bool isLockedOn = false;       // Lock-on state

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
            return;
        }

        // Set the initial offset based on the desired distance
        offset = new Vector3(0, 0, -distance);

        // Initialize rotation based on the current camera orientation
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }

        // Handle Lock-On Input
        if (Input.GetKeyDown(KeyCode.Q)) // Press Q to toggle lock-on
        {
            ToggleLockOn();
        }

        if (isLockedOn && target != null)
        {
            LockOnTarget(); // Focus camera on target
        }
        else
        {
            FreeLookCamera(); // Default free-look behavior
        }
    }

    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true;                 // Make cursor visible
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
            Cursor.visible = false;                   // Hide the cursor
        }
    }

    void ToggleLockOn()
    {
        if (isLockedOn)
        {
            // Unlock
            isLockedOn = false;
            target = null;
        }
        else
        {
            // Lock onto the nearest enemy
            FindClosestEnemy();
        }
    }

    void FindClosestEnemy()
    {
        float radius = 20.0f; // Lock-on search radius
        Collider[] hits = Physics.OverlapSphere(player.position, radius, LayerMask.GetMask("Enemy")); // Assume enemies are on "Enemy" layer

        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider hit in hits)
        {
            float distance = Vector3.Distance(player.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = hit.transform;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy;
            isLockedOn = true;
        }
    }

    void LockOnTarget()
    {
        if (target == null)
        {
            isLockedOn = false;
            return;
        }

        // Calculate direction towards the target without affecting player rotation
        Vector3 targetDirection = (target.position - player.position).normalized;

        // Calculate desired position for the camera
        Vector3 relativePosition = -targetDirection * distance + Vector3.up * lockOnHeightOffset;
        Vector3 desiredPosition = player.position + relativePosition;

        // Collision handling
        RaycastHit hit;
        if (Physics.SphereCast(player.position + Vector3.up * heightOffset, collisionRadius, relativePosition.normalized, out hit, distance, collisionLayers))
        {
            desiredPosition = hit.point + hit.normal * collisionOffset;
        }

        // Smooth camera transition
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);

        // Look at the target without forcing player rotation
        Vector3 lookAtPosition = target.position + Vector3.up * 1.0f;
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);

        // Smooth rotation to avoid jitter
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }



    void FreeLookCamera()
    {
        // Get mouse inputs for rotation
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Update yaw and pitch based on input
        yaw += mouseX;
        pitch -= mouseY;

        // Clamp pitch to prevent flipping
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // Calculate the desired rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Calculate the desired position without collision handling
        Vector3 desiredPosition = player.position + rotation * offset;
        desiredPosition.y += heightOffset; // Apply height offset

        // Perform a SphereCast to handle collisions with obstacles
        RaycastHit hit;
        Vector3 direction = desiredPosition - (player.position + Vector3.up * heightOffset);
        float maxDistance = direction.magnitude;

        if (Physics.SphereCast(player.position + Vector3.up * heightOffset, collisionRadius, direction.normalized, out hit, maxDistance, collisionLayers))
        {
            float hitDistance = Vector3.Distance(player.position, hit.point);
            float adjustedDistance = Mathf.Clamp(hitDistance - collisionOffset, minDistance, distance);
            desiredPosition = player.position + rotation * new Vector3(0, 0, -adjustedDistance);
            desiredPosition.y += heightOffset;
        }

        // Apply the final position and rotation
        transform.position = desiredPosition;
        transform.LookAt(player.position + Vector3.up * heightOffset);
    }
}
