using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCase : MonoBehaviour
{
    [Tooltip(("Objects that will be rotated."))]
    public GameObject[] objectsToRotate;

    [Tooltip("Hinge to rotate the object(s)")]
    public GameObject rotationPoint;
    
    [Tooltip("The total degrees to rotate.")]
    public float rotationAmount = 90.0f;

    [Tooltip("The time it takes to complete the rotation.")]
    public float duration = 1.0f;

    [Tooltip("The axis around which to rotate. (0, 1, 0) is the Y-axis.")]
    public Vector3 rotationAxis = Vector3.up;

    // --- Private state ---
    private bool isRotating = false;
    private Coroutine rotationCoroutine;
    
    // Public function to call rotation
    public void StartRotation()
    {
        // Prevent starting a new rotation if one is already in progress
        if (!isRotating)
        {
            rotationCoroutine = StartCoroutine(RotateOverTime());
        }
    }


    /// Coroutine to handle the rotation over a set duration.
    private IEnumerator RotateOverTime()
    {
        // Set the rotating flag to true
        isRotating = true;

        // Store the starting rotation
        Quaternion startRotation = transform.rotation;

        // Calculate the target rotation
        // We multiply the start rotation by a new rotation based on our axis and amount
        Quaternion targetRotation = startRotation * Quaternion.AngleAxis(rotationAmount, rotationAxis.normalized);
        
        // Store the point for the object(s) to rotate around

        float elapsedTime = 0f;

        // Loop until the elapsed time is greater than or equal to the duration
        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor (t) from 0 to 1
            float t = elapsedTime / duration;
            
            // Optional: Add easing for a smoother effect (e.g., "SmoothStep")
            // t = t * t * (3f - 2f * t);

            // Slerp (Spherical Linear Interpolation) from start to target rotation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            // Wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- Rotation complete ---

        // Snap to the final target rotation to ensure 100% accuracy
        transform.rotation = targetRotation;

        // Reset the flag
        isRotating = false;
    }
}
