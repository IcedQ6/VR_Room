using System.Collections;
using UnityEngine;

/// <summary>
/// Saves the object's original local (parent-relative) position and rotation
/// and returns it to that state when ReturnToOriginal() is called.
/// </summary>
public class ReturnToPosition : MonoBehaviour
{
    [Tooltip("The time it takes to complete the reset animation.")]
    public float duration = 0.5f;

    // --- Private state ---
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;
    
    private bool isResetting = false;
    private Coroutine resetCoroutine;

    /// <summary>
    /// Stores the object's starting local transform values.
    /// </summary>
    void Awake()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    /// <summary>
    /// Public function to start the reset process.
    /// </summary>
    public void ReturnToOriginal()
    {
        // If an animation is already running, stop it first to start the new one.
        if (isResetting)
        {
            StopCoroutine(resetCoroutine);
        }

        resetCoroutine = StartCoroutine(ResetPositionRoutine());
    }

    /// <summary>
    /// Coroutine to handle the reset over a set duration.
    /// </summary>
    private IEnumerator ResetPositionRoutine()
    {
        isResetting = true;
        float elapsedTime = 0f;

        // Get the current local position/rotation to interpolate from
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            
            // Optional: Easing for a smoother effect
            // t = t * t * (3f - 2f * t);

            // Interpolate local position
            transform.localPosition = Vector3.Lerp(startPosition, originalLocalPosition, t);
            
            // Interpolate local rotation
            transform.localRotation = Quaternion.Slerp(startRotation, originalLocalRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // --- Reset complete ---

        // Snap to the original values to ensure 100% accuracy
        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;
        
        isResetting = false;
    }
}