using UnityEngine;

public class player_Camera : MonoBehaviour
{
    public Transform player;               // Player to follow
    public float distance = 10.0f;         // Desired fixed distance from the player
    public float minDistance = 2.0f;       // Minimum distance to maintain from the player
    public float rotationSpeed = 5.0f;     // Speed of rotation
    public float collisionOffset = 0.2f;   // Offset to prevent clipping
    public float heightOffset = 2.0f;     // Height offset to keep the camera slightly above the player
    public float collisionRadius = 0.5f;   // Radius for SphereCast to detect thin obstacles
    public LayerMask collisionLayers;      // Layers to check for collisions

    private Vector3 offset;                // Offset based on distance
    private float yaw = 0.0f;              // Horizontal rotation
    private float pitch = 0.0f;            // Vertical rotation

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
    }

    void Update()
    {
        if (player == null) return;

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
        Vector3 direction = desiredPosition - (player.position + Vector3.up * heightOffset); // Direction from player to camera
        float maxDistance = direction.magnitude; // Max distance to check for collisions

        if (Physics.SphereCast(player.position + Vector3.up * heightOffset, collisionRadius, direction.normalized, out hit, maxDistance, collisionLayers))
        {
            // Calculate the distance from the player to the hit point
            float hitDistance = Vector3.Distance(player.position, hit.point);

            // Maintain a minimum distance from the player
            float adjustedDistance = Mathf.Clamp(hitDistance - collisionOffset, minDistance, distance);

            // Update the desired position based on the adjusted distance
            desiredPosition = player.position + rotation * new Vector3(0, 0, -adjustedDistance);
            desiredPosition.y += heightOffset; // Re-apply height offset after adjustment
        }

        // Apply the final position and rotation
        transform.position = desiredPosition;
        transform.LookAt(player.position + Vector3.up * heightOffset); // Look slightly above the player
    }
}