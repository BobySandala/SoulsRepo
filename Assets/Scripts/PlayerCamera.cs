using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCamera : MonoBehaviour
{

    public Transform player; // Assign the player's transform in the Inspector
    public float followSpeed = 5f;
    public float rotationSpeed = 100f;
    public float collisionOffset = 0.2f; // Offset to avoid clipping through objects
    public LayerMask collisionMask; // LayerMask for objects to check collision against

    private Vector3 initialOffset; // Offset from player to camera

    public static PlayerCamera instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //Debug.Log("else");
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void LateUpdate()
    {
        HandleAllCameraAction();
    }

    public void HandleAllCameraAction()
    {
        FollowPlayer();
        RotateAroundPlayer();
        HandleCameraCollision();
    }

    private void FollowPlayer()
    {
        if (player == null) return;

        // Smoothly move the camera to follow the player's position
        Vector3 targetPosition = player.position + initialOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    private void RotateAroundPlayer()
    {
        if (player == null) return;

        // Rotate the camera around the player based on mouse input
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Rotate the camera around the player horizontally and vertically
        Quaternion rotation = Quaternion.Euler(vertical, horizontal, 0);
        initialOffset = rotation * initialOffset;

        // Apply the rotation to the camera
        transform.LookAt(player.position);
    }

    private void HandleCameraCollision()
    {
        if (player == null) return;

        // Raycast from player to camera to detect collisions
        Vector3 desiredCameraPosition = player.position + initialOffset;
        RaycastHit hit;

        if (Physics.Linecast(player.position, desiredCameraPosition, out hit, collisionMask))
        {
            // Adjust the camera position to be just before the collision point
            transform.position = hit.point - (hit.normal * collisionOffset);
        }
        else
        {
            // If no collision, move the camera to its desired position
            transform.position = desiredCameraPosition;
        }
    }
    

}
