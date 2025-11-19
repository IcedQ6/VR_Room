using UnityEngine;

public class TransformLock : MonoBehaviour
{
    [Header("Position Locking")]
    public bool lockPosX = false;
    public bool lockPosY = false;
    public bool lockPosZ = false;

    [Header("Rotation Locking")]
    public bool lockRotX = false;
    public bool lockRotY = false;
    public bool lockRotZ = false;

    [Header("Settings")]
    [Tooltip("TRUE = Lock to Universe Coordinates. FALSE = Lock relative to Parent.")]
    public bool useWorldSpace = false;

    // Storage for the values we want to keep
    private Vector3 targetPosition;
    private Vector3 targetRotation;

    void Awake()
    {
        // 1. Capture the starting values when the game begins
        if (useWorldSpace)
        {
            targetPosition = transform.position;
            targetRotation = transform.eulerAngles;
        }
        else
        {
            targetPosition = transform.localPosition;
            targetRotation = transform.localEulerAngles;
        }
    }

    void LateUpdate()
    {
        // 2. Get the current state (in case other scripts moved it this frame)
        Vector3 currentPos = useWorldSpace ? transform.position : transform.localPosition;
        Vector3 currentRot = useWorldSpace ? transform.eulerAngles : transform.localEulerAngles;

        // 3. Calculate the new Position
        // If locked, use the stored 'targetPosition'. If not locked, use the 'currentPos'.
        float newPosX = lockPosX ? targetPosition.x : currentPos.x;
        float newPosY = lockPosY ? targetPosition.y : currentPos.y;
        float newPosZ = lockPosZ ? targetPosition.z : currentPos.z;

        // 4. Calculate the new Rotation
        float newRotX = lockRotX ? targetRotation.x : currentRot.x;
        float newRotY = lockRotY ? targetRotation.y : currentRot.y;
        float newRotZ = lockRotZ ? targetRotation.z : currentRot.z;

        // 5. Apply the changes
        Vector3 finalPos = new Vector3(newPosX, newPosY, newPosZ);
        Vector3 finalRot = new Vector3(newRotX, newRotY, newRotZ);

        if (useWorldSpace)
        {
            transform.position = finalPos;
            transform.eulerAngles = finalRot;
        }
        else
        {
            transform.localPosition = finalPos;
            transform.localEulerAngles = finalRot;
        }
    }
    
    // Optional: Call this if you need to reset the "Lock" point to the object's current location
    public void UpdateLockTargets()
    {
        if (useWorldSpace)
        {
            targetPosition = transform.position;
            targetRotation = transform.eulerAngles;
        }
        else
        {
            targetPosition = transform.localPosition;
            targetRotation = transform.localEulerAngles;
        }
    }
}