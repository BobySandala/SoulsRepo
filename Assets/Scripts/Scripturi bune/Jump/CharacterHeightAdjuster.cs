using UnityEngine;
using System.Collections;

public class CapsuleHeightAdjuster : MonoBehaviour
{
    public CapsuleCollider capsuleCollider; // Assign your character's Capsule Collider in the Inspector
    private float originalHeight; // Stores the original height of the capsule
    private Vector3 originalCenter; // Stores the original center of the capsule

    private void Start()
    {
        if (capsuleCollider == null)
        {
            capsuleCollider = GetComponent<CapsuleCollider>();
        }

        // Store the original height and center of the capsule
        if (capsuleCollider != null)
        {
            originalHeight = capsuleCollider.height;
            originalCenter = capsuleCollider.center;
        }
        else
        {
            //Debug.LogError("CapsuleCollider is not assigned!");
        }
    }

    /// <summary>
    /// Adjust the capsule height instantly.
    /// </summary>
    /// <param name="newHeight">The new height of the capsule.</param>
    public void AdjustCapsuleHeight(float newHeight)
    {
        if (capsuleCollider == null) return;

        capsuleCollider.height = newHeight;

        // Adjust the center to keep the capsule properly aligned
        capsuleCollider.center = new Vector3(
            capsuleCollider.center.x,
            newHeight / 2,
            capsuleCollider.center.z
        );
    }

    /// <summary>
    /// Smoothly adjust the capsule height over a given duration.
    /// </summary>
    /// <param name="targetHeight">The target height for the capsule.</param>
    /// <param name="duration">The time duration for the adjustment.</param>
    public void SmoothAdjustCapsuleHeight(float targetHeight, float duration)
    {
        if (capsuleCollider == null) return;

        StopAllCoroutines(); // Stop any ongoing height adjustment
        StartCoroutine(SmoothHeightChange(targetHeight, duration));
    }

    /// <summary>
    /// Reset the capsule height and center to their original values.
    /// </summary>
    public void ResetCapsuleHeight()
    {
        if (capsuleCollider == null) return;

        AdjustCapsuleHeight(originalHeight);
        capsuleCollider.center = originalCenter;
    }

    private IEnumerator SmoothHeightChange(float targetHeight, float duration)
    {
        float startHeight = capsuleCollider.height;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsedTime / duration);

            capsuleCollider.height = newHeight;
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                newHeight / 2,
                capsuleCollider.center.z
            );

            yield return null;
        }

        // Ensure the final height is set
        capsuleCollider.height = targetHeight;
        capsuleCollider.center = new Vector3(
            capsuleCollider.center.x,
            targetHeight / 2,
            capsuleCollider.center.z
        );
    }
}
