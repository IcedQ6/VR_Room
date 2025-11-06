using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // Needed for UnityEvent

/// <summary>
/// Creates a rotary dial for VR that outputs its value as an integer.
/// 
/// --- SETUP ---
/// 1. Place this script on the GameObject that will be rotated (the "dial").
/// 2. This GameObject MUST also have:
///    - A Rigidbody
///    - An XRGrabInteractable (or similar VR grab component)
///    - A HingeJoint
/// 
/// 3. Configure the HingeJoint:
///    - Set the 'Axis' to your local rotation axis (e.g., (0, 1, 0) for Y-axis).
///    - Check 'Use Limits'.
///    - Set the 'Min' and 'Max' angles (e.g., -135 and 135).
/// 
/// 4. Configure this script:
///    - Set 'NumberOfSteps' to the total integer values (e.g., 10 for values 0-9).
///    - Hook up the 'OnValueChanged' event in the Inspector.
/// </summary>
[RequireComponent(typeof(HingeJoint))]
public class VRRotaryDial : MonoBehaviour
{
    [Header("Dial Settings")]
    [Tooltip("Total number of integer steps the dial has. (e.g., 10 for values 0-9)")]
    public int numberOfSteps = 10;

    [Header("Events")]
    [Tooltip("Fired when the dial 'clicks' into a new integer value.")]
    public UnityEvent<int> OnValueChanged;

    // --- Private State ---
    private HingeJoint hinge;
    private float totalAngleRange;
    private float minAngle;
    private int currentValue = -1; // Start at -1 to force the first event
    
    void Awake()
    {
        hinge = GetComponent<HingeJoint>();

        // Check for valid setup
        if (!hinge.useLimits)
        {
            Debug.LogWarning("VRRotaryDial: The HingeJoint on this object must have 'Use Limits' enabled.", this);
            return;
        }

        if (numberOfSteps <= 1)
        {
            Debug.LogWarning("VRRotaryDial: NumberOfSteps must be 2 or more.", this);
            numberOfSteps = 2; // Default to a min of 2 to avoid division by zero
        }
        
        // Read the limits from the HingeJoint
        minAngle = hinge.limits.min;
        totalAngleRange = hinge.limits.max - hinge.limits.min;

        if (totalAngleRange <= 0)
        {
            Debug.LogWarning("VRRotaryDial: HingeJoint limits are invalid (Max angle must be greater than Min angle).", this);
        }
    }

    void Update()
    {
        if (totalAngleRange <= 0) return; // Don't run if setup is invalid

        // 1. Get the current angle from the hinge
        float currentAngle = hinge.angle;
        
        // 2. Normalize the angle (get a value from 0.0 to 1.0)
        // Mathf.InverseLerp finds where 'currentAngle' sits between 'min' and 'max'
        float normalizedValue = Mathf.InverseLerp(minAngle, hinge.limits.max, currentAngle);

        // 3. Map the normalized value to our integer steps
        // (numberOfSteps - 1) is because the steps are 0-indexed (e.g., 10 steps is 0 to 9)
        float floatValue = normalizedValue * (numberOfSteps - 1);
        
        // 4. Round to the nearest integer to get the current "stop"
        int newValue = Mathf.RoundToInt(floatValue);
        
        // 5. Fire the event if the value has changed
        if (newValue != currentValue)
        {
            currentValue = newValue;
            OnValueChanged.Invoke(currentValue);
            
            // Optional: Log the new value
            Debug.Log($"Dial value changed to: {currentValue}");
        }
    }
}