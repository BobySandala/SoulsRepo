using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform player;                // Reference to the player
    [SerializeField] private float cameraDistance = 5f;       // Default distance from the player
    [SerializeField] private float mouseSensitivity = 5f;     // Sensitivity for mouse movement
    [SerializeField] private float cameraPitchLimit = 45f;    // Max vertical angle of the camera (up/down)
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, -3); // Offset for camera positioning
    [SerializeField] private LayerMask collisionLayers;       // Layers to detect for collisions

    private float pitch = 0f;               // Current vertical angle of the camera
    private float yaw = 0f;                 // Current horizontal angle of the camera

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleCameraRotation();
        UpdateCameraPosition();
    }

    void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Adjust yaw (horizontal rotation)
        yaw += mouseX;

        // Adjust pitch (vertical rotation)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -cameraPitchLimit, cameraPitchLimit); // Clamp pitch to prevent flipping

        // Rotate the player to face the camera's horizontal direction
        player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void UpdateCameraPosition()
    {
        // Calculate the desired camera position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 desiredPosition = player.position + rotation * new Vector3(0, offset.y, -cameraDistance);

        // Perform a raycast to check for collisions
        RaycastHit hit;
        Vector3 cameraTargetPosition = player.position + Vector3.up * offset.y;

        if (Physics.Raycast(cameraTargetPosition, desiredPosition - cameraTargetPosition, out hit, cameraDistance, collisionLayers))
        {
            // Adjust the camera position to the collision point
            transform.position = hit.point - (desiredPosition - cameraTargetPosition).normalized * 0.2f; // Small offset to prevent clipping
        }
        else
        {
            // If no collision, position the camera at the desired location
            transform.position = desiredPosition;
        }

        // Make the camera look at the player
        transform.LookAt(cameraTargetPosition);
    }
}
