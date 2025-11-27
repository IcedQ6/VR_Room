using System.Collections;
using UnityEngine;

/// <summary>
/// This script rotates its own transform between an "open" and "closed" state.
/// Place this on an empty GameObject pivot (the "hinge").
/// Make all objects you want to rotate (like a lid) children of this pivot.
/// </summary>
public class OpenCase : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The total degrees to rotate.")]
    public float rotationAmount = 90.0f;

    [Tooltip("The time it takes to complete the rotation.")]
    public float duration = 1.0f;

    [Tooltip("The LOCAL axis to rotate around. Use the gizmo to find the correct one.")]
    public Vector3 localRotationAxis = Vector3.up; // e.g., (1,0,0) for X-axis

    [Header("References")]
    [Tooltip("The script that manages the sockets inside the case. Required for hiding/showing contents.")]
    public SocketGroupMemory memoryScript;

    // --- Private State ---
    private bool isRotating = false;
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

    void Start()
    {
        // Ensure sockets start hidden if the case starts closed
        // We assume it starts closed based on Awake logic
        if (memoryScript != null)
        {
            memoryScript.SetGroupVisibility(false);
        }
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
            // If socketed, force closed
            targetRotation = closedRotation;
        }
        else
        {
            targetRotation = needsToOpen ? openRotation : closedRotation;
        }

        if (targetRotation == transform.localRotation) return;

        // VISIBILITY LOGIC (OPENING):
        // If we are about to OPEN, show the contents immediately so they appear as the lid lifts.
        if (targetRotation == openRotation && memoryScript != null)
        {
            memoryScript.SetGroupVisibility(true);
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

        // VISIBILITY LOGIC (CLOSING):
        // If we just finished CLOSING, hide the contents now that the lid is fully down.
        // This prevents items from "popping" out of existence while the lid is still moving.
        if (target == closedRotation && memoryScript != null)
        {
            memoryScript.SetGroupVisibility(false);
        }

        // Reset the flag
        isRotating = false;
    }
    
    public void SetIsInSocket(bool isInSocket)
    {
        this.isInSocket = isInSocket;
    }
}