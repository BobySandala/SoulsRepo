using UnityEngine;

public class NeckMovement : MonoBehaviour
{
    public Transform neckBone;             // Reference to the neck bone in the rig
    public float mouseSensitivity = 5f;    // Sensitivity for mouse movement
    public float maxNeckPitch = 30f;       // Maximum tilt angle for looking up/down
    public float maxNeckYaw = 30f;         // Maximum rotation angle for looking left/right

    private float pitch = 0f;              // Current pitch value (up/down)
    private float yaw = 0f;                // Current yaw value (left/right)

    void Update()
    {
        MoveNeck();
    }

    void MoveNeck()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity; // Horizontal input
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity; // Vertical input

        // Adjust pitch (up/down movement)
        pitch -= mouseX; // Subtract to invert mouse movement for natural behavior
        pitch = Mathf.Clamp(pitch, -maxNeckPitch, maxNeckPitch); // Limit pitch

        // Adjust yaw (left/right movement)
        yaw -= mouseY;
        yaw = Mathf.Clamp(yaw, -maxNeckYaw, maxNeckYaw); // Limit yaw

        // Apply pitch and yaw to the neck bone
        neckBone.localRotation = Quaternion.Euler(pitch, 0f, yaw); // Adjust axes if needed
    }
}
