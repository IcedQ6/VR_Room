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

        // --- START: MODIFIED LOGIC ---

        float elapsedTime = 0f;
        float currentAngle = 0f; // Track how much we've rotated so far

        // Get the pivot point and axis *once*
        Vector3 pivotPoint = rotationPoint.transform.position;
        Vector3 axis = rotationAxis.normalized;

        // Loop until the elapsed time is greater than or equal to the duration
        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor (t) from 0 to 1
            float t = elapsedTime / duration;
            
            // Optional: Add easing for a smoother effect (e.g., "SmoothStep")
            // t = t * t * (3f - 2f * t);

            // Calculate what our target angle should be at this specific moment
            float targetAngleThisFrame = Mathf.Lerp(0, rotationAmount, t);
            
            // Calculate the small angle to rotate *this frame* to catch up
            float deltaAngle = targetAngleThisFrame - currentAngle;

            // Iterate through all objects and rotate them around the pivot
            foreach (GameObject obj in objectsToRotate)
            {
                if (obj != null)
                {
                    obj.transform.RotateAround(pivotPoint, axis, deltaAngle);
                }
            }

            // Wait for the next frame
            elapsedTime += Time.deltaTime;
            // Update our current angle
            currentAngle = targetAngleThisFrame; 
            yield return null;
        }

        // --- Rotation complete ---

        // Snap to the final target angle to ensure 100% accuracy
        float finalDelta = rotationAmount - currentAngle;
        foreach (GameObject obj in objectsToRotate)
        {
            if (obj != null)
            {
                obj.transform.RotateAround(pivotPoint, axis, finalDelta);
            }
        }

        // --- END: MODIFIED LOGIC ---

        // Reset the flag
        isRotating = false;
    }
}