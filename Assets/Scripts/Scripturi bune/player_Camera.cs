using UnityEngine;
using System.Collections.Generic;

public class player_Camera : MonoBehaviour
{
    public Transform player;               // Player to follow
    public float distance = 10.0f;         // Desired fixed distance from the player
    public float minDistance = 2.0f;       // Minimum distance to maintain from the player
    public float rotationSpeed = 3.0f;     // Reduced speed for smoother rotation
    public float collisionOffset = 0.3f;   // Offset to prevent clipping
    public float heightOffset = 2.0f;      // Height offset to keep the camera slightly above the player
    public float lockOnHeightOffset = 4.0f; // Higher height offset when locked onto an enemy
    public LayerMask collisionLayers;      // Layers to check for collisions
    public Transform target;               // Current lock-on target

    private float yaw = 0.0f;              // Horizontal rotation
    private float pitch = 0.0f;            // Vertical rotation
    private bool isLockedOn = false;       // Lock-on state
    private List<Transform> nearbyTargets = new List<Transform>(); // List of nearby enemies
    private int targetIndex = 0;           // Index to track current target
    private HashSet<Renderer> hiddenRenderers = new HashSet<Renderer>();

    public float collisionRadius = 0.5f; // Radius for SphereCast to detect thin obstacles

    [Range(0.05f, 2.0f)] // Adjustable slider for minimum closeness in the Inspector
    public float adjustableMinCloseDistance = 0.1f; // Allow the camera to get very close to the character
    public float smoothingTime = 0.4f; // Increased for smoother transitions
    public float heightSmoothingTime = 0.5f; // Increased for smoother height adjustments
    public float jitterThreshold = 0.05f; // Minimum position change to update
    public float heightStabilityThreshold = 0.05f; // Ignore minor height changes

    private Vector3 currentVelocity; // SmoothDamp velocity for smoothing camera movement
    private float currentHeightVelocity; // SmoothDamp velocity for height adjustment

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player Transform is not assigned!");
            return;
        }

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

        if (Input.GetKeyDown(KeyCode.Tab)) // Press Tab to cycle targets
        {
            CycleLockOnTarget();
        }

        if (isLockedOn && target != null)
        {
            LockOnTarget();
        }
        else
        {
            FreeLookCamera();
        }

        // Handle obstacles and adjust camera
        AdjustForObstacles();
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
            FindNearbyEnemies();
            SelectTarget(0); // Select the first target
        }
    }

    void FindNearbyEnemies()
    {
        float radius = 20.0f; // Lock-on search radius
        Collider[] hits = Physics.OverlapSphere(player.position, radius, LayerMask.GetMask("Enemy")); // Assume enemies are on "Enemy" layer

        nearbyTargets.Clear(); // Clear previous targets

        foreach (Collider hit in hits)
        {
            nearbyTargets.Add(hit.transform);
        }
    }

    void SelectTarget(int index)
    {
        if (nearbyTargets.Count > 0)
        {
            targetIndex = index % nearbyTargets.Count; // Wrap around the list
            target = nearbyTargets[targetIndex];       // Set the current target
            isLockedOn = true;                         // Enable lock-on
        }
        else
        {
            target = null; // No targets available
            isLockedOn = false;
        }
    }

    void CycleLockOnTarget()
    {
        if (isLockedOn && nearbyTargets.Count > 0)
        {
            targetIndex = (targetIndex + 1) % nearbyTargets.Count; // Move to the next target
            target = nearbyTargets[targetIndex];
        }
        else
        {
            FindNearbyEnemies(); // Refresh targets if not locked on
            SelectTarget(0);     // Lock on to the first target
        }
    }

    void FreeLookCamera()
    {
        PlayerData playerData = player.GetComponent<PlayerData>();
        if (playerData != null && playerData.state == 1) // If the player is sitting
        {
            // Allow only horizontal camera movement
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;

            yaw += mouseX; // Update horizontal rotation
                           // Lock the pitch (no vertical movement)
            Quaternion rotation = Quaternion.Euler(0, yaw, 0);

            Vector3 offset = new Vector3(0, 0, -distance);
            Vector3 desiredPosition = player.position + rotation * offset;
            desiredPosition.y += heightOffset; // Maintain height offset

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);
            transform.LookAt(player.position + Vector3.up * heightOffset); // Always look at the player
            Debug.Log("Horizontal-only camera movement in sitting state.");
            return;
        }

        // Existing free look camera logic for when not sitting
        float mouseXFree = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseYFree = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseXFree;
        pitch -= mouseYFree;

        pitch = Mathf.Clamp(pitch, -89f, 89f);

        Quaternion rotationFree = Quaternion.Euler(pitch, yaw, 0);

        float dynamicDistance = distance;
        if (pitch > 45f) // When looking up
        {
            dynamicDistance = Mathf.Lerp(distance, minDistance, (pitch - 45f) / 45f); // Gradually reduce distance
        }

        Vector3 offsetFree = new Vector3(0, 0, -dynamicDistance);
        Vector3 desiredPositionFree = player.position + rotationFree * offsetFree;
        desiredPositionFree.y += heightOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPositionFree, Time.deltaTime * rotationSpeed);
        transform.LookAt(player.position + Vector3.up * heightOffset);
    }



    void LockOnTarget()
    {
        if (target == null)
        {
            isLockedOn = false;
            return;
        }

        Vector3 targetDirection = (target.position - player.position).normalized;

        float clampedDistance = Mathf.Max(distance, minDistance); // Prevent getting too close
        Vector3 relativePosition = -targetDirection * clampedDistance + Vector3.up * lockOnHeightOffset; // Maintain height offset
        Vector3 desiredPosition = player.position + relativePosition;

        // Increase the speed of catching up to the target position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * (rotationSpeed * 2.0f));

        Vector3 lookAtPosition = target.position + Vector3.up * 1.5f; // Focus slightly above target's center
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPosition - transform.position);

        // Increase the speed of catching up to the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * (rotationSpeed * 2.0f));
    }


    void AdjustForObstacles()
    {
        float minCloseDistance = adjustableMinCloseDistance; // Closest distance allowed to the player
        Vector3 targetPosition = transform.position;

        // Re-enable previously hidden renderers
        foreach (Renderer renderer in hiddenRenderers)
        {
            if (renderer != null) renderer.enabled = true;
        }
        hiddenRenderers.Clear();

        // Calculate direction from camera to player
        Vector3 playerCenter = player.position + Vector3.up * heightOffset; // Center of the player
        Vector3 cameraToPlayerDirection = playerCenter - transform.position;

        // Check for obstacles using SphereCast
        if (Physics.SphereCast(playerCenter, collisionRadius, -cameraToPlayerDirection.normalized, out RaycastHit hit, distance, collisionLayers))
        {
            // If an obstacle is detected, move the camera close to the player
            float obstacleDistance = Mathf.Clamp(hit.distance - collisionOffset, minCloseDistance, distance);
            targetPosition = playerCenter - cameraToPlayerDirection.normalized * obstacleDistance;

            // Add extra height to avoid floor obstruction
            if (Vector3.Dot(cameraToPlayerDirection.normalized, Vector3.up) > 0.5f) // Looking upwards
            {
                targetPosition.y += 1.0f; // Raise the camera height to clear the floor
            }

            // Hide the obstacle renderer to prevent blocking
            Renderer obstacleRenderer = hit.collider.GetComponent<Renderer>();
            if (obstacleRenderer != null)
            {
                obstacleRenderer.enabled = false;
                hiddenRenderers.Add(obstacleRenderer);
            }
        }
        else
        {
            // Default position when no obstacles are detected
            targetPosition = playerCenter - cameraToPlayerDirection.normalized * distance;
        }

        // Smoothly move the camera to the target position
        if (Vector3.Distance(transform.position, targetPosition) > jitterThreshold)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothingTime);
        }

        // Adjust for height and upward-looking scenarios
        AdjustForHeight(ref targetPosition);
        AdjustCameraWhenLookingUp(ref targetPosition);

        // Ensure the character remains visible
        EnsureCharacterVisibility();
    }


    void AdjustForHeight(ref Vector3 targetPosition)
    {
        float groundHeight = GetGroundHeight();
        float currentHeight = transform.position.y;

        if (Mathf.Abs(currentHeight - groundHeight) > heightStabilityThreshold)
        {
            float smoothedHeight = Mathf.SmoothDamp(currentHeight, groundHeight, ref currentHeightVelocity, heightSmoothingTime);
            targetPosition.y = smoothedHeight;
        }
        else
        {
            targetPosition.y = currentHeight; // Maintain current height for minor changes
        }
    }

    void AdjustCameraWhenLookingUp(ref Vector3 targetPosition)
    {
        Vector3 cameraDirection = transform.forward;
        if (Vector3.Dot(cameraDirection, Vector3.up) > 0.8f) // Check if camera is pointing upwards
        {
            float minHeightAboveGround = player.position.y + adjustableMinCloseDistance;
            targetPosition.y = Mathf.Max(targetPosition.y, minHeightAboveGround);
        }
    }

    void EnsureCharacterVisibility()
    {
        Vector3 directionToCharacter = (player.position + Vector3.up * heightOffset) - transform.position;
        if (Physics.Raycast(transform.position, directionToCharacter.normalized, out RaycastHit hit, directionToCharacter.magnitude, collisionLayers))
        {
            Renderer obstacleRenderer = hit.collider.GetComponent<Renderer>();
            if (obstacleRenderer != null)
            {
                obstacleRenderer.enabled = false;
                hiddenRenderers.Add(obstacleRenderer);
            }
        }
    }

    float GetGroundHeight()
    {
        if (Physics.Raycast(player.position + Vector3.up * 10.0f, Vector3.down, out RaycastHit hit, 20.0f, collisionLayers))
        {
            return hit.point.y + heightOffset;
        }

        return player.position.y + heightOffset;
    }
}