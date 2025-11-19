using System.Collections;
using UnityEngine;

/// <summary>
/// This script rotates its own transform between an "open" and "closed" state.
/// Place this on an empty GameObject pivot (the "hinge").
/// Make all objects you want to rotate (like a lid) children of this pivot.
/// </summary>
public class OpenCase : MonoBehaviour
{
    [Tooltip("The total degrees to rotate.")]
    public float rotationAmount = 90.0f;

    [Tooltip("The time it takes to complete the rotation.")]
    public float duration = 1.0f;

    [Tooltip("The LOCAL axis to rotate around. Use the gizmo to find the correct one.")]
    public Vector3 localRotationAxis = Vector3.up; // e.g., (1,0,0) for X-axis

    // --- Private State ---
    private bool isRotating = false;
    //private bool isOpen = false;
    public bool isInSocket = false;
    
    private Coroutine rotationCoroutine;
    private Quaternion closedRotation;
    private Quaternion openRotation;


    void Awake()
    {
        // Store the initial LOCAL rotation as the "closed" state
        closedRotation = transform.localRotation;
        
        // Calculate the "open" state by applying the rotation
        openRotation = closedRotation * Quaternion.AngleAxis(rotationAmount, localRotationAxis.normalized);
    }

    /// <summary>
    /// Public function to start the rotation.
    /// This will toggle between the open and closed states.
    /// </summary>
    public void StartRotation(bool needsToOpen)
    {
        // Do nothing if we are already in the middle of a rotation
        if (isRotating)
        {
            return;
        }

        // Figure out our target rotation
        Quaternion targetRotation;
        if (isInSocket)
        {
            targetRotation = closedRotation;
        }
        else
        {
            targetRotation = needsToOpen ? openRotation : closedRotation;
        }
        
        
        // Start the rotation coroutine
        rotationCoroutine = StartCoroutine(RotateOverTime(targetRotation));
    }

    /// <summary>
    /// Coroutine to handle the rotation over a set duration.
    /// </summary>
    private IEnumerator RotateOverTime(Quaternion target)
    {
        isRotating = true;
        float elapsedTime = 0f;
        
        // Get the rotation we are starting from
        Quaternion start = transform.localRotation;

        while (elapsedTime < duration)
        {
            // Calculate the interpolation factor (t) from 0 to 1
            float t = elapsedTime / duration;

            // Slerp (Spherical Linear Interpolation) this object's LOCAL rotation
            transform.localRotation = Quaternion.Slerp(start, target, t);

            // Wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- Rotation complete ---

        // Snap to the final target rotation to ensure 100% accuracy
        transform.localRotation = target;

        // Reset the flag
        isRotating = false;
    }
    
    public void SetIsInSocket(bool isInSocket)
    {
        this.isInSocket = isInSocket;
    }

}