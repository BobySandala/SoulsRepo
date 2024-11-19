using UnityEngine;

public class PlayerCameraMovement : MonoBehaviour
{
    public Transform player;               // Reference to the player
    public float cameraDistance = 2f;      // Distance from the player (closer default distance)
    public float mouseSensitivity = 5f;    // Sensitivity for mouse movement
    public float cameraPitchLimit = 45f;   // Max vertical angle of the camera
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3); // Adjust offset for centering

    private float pitch = 0f;              // Current vertical angle of the camera
    private float yaw = 0f;                // Current horizontal angle of the camera

    void LateUpdate()
    {
        RotateCameraAndPlayer();
        UpdateCameraPosition();
    }

    void RotateCameraAndPlayer()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Adjust yaw (horizontal rotation)
        yaw += mouseX;

        // Adjust pitch (vertical rotation)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -cameraPitchLimit, cameraPitchLimit); // Clamp pitch

        // Rotate the player to face the camera's horizontal direction
        player.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    void UpdateCameraPosition()
    {
        // Calculate the camera's rotation based on pitch and yaw
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Calculate the new camera position (closer to the character)
        Vector3 offset = rotation * new Vector3(0, cameraOffset.y, -cameraDistance);
        transform.position = player.position + offset;

        // Make the camera look at the player
        transform.LookAt(player.position + Vector3.up * cameraOffset.y);
    }
}
