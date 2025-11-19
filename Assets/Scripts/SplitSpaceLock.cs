using UnityEngine;

public class SplitSpaceLock : MonoBehaviour
{
    [Header("Position Locking (Local Space)")]
    public bool lockLocalPosX = false;
    public bool lockLocalPosY = false;
    public bool lockLocalPosZ = false;

    [Header("Rotation Locking (World Space)")]
    public bool lockWorldRotX = false;
    public bool lockWorldRotY = false;
    public bool lockWorldRotZ = false;

    // Storage for the initial values
    private Vector3 targetLocalPos;
    private Vector3 targetWorldRot;

    void Awake()
    {
        // Capture the initial Local Position (relative to parent)
        targetLocalPos = transform.localPosition;

        // Capture the initial World Rotation (relative to the universe)
        targetWorldRot = transform.eulerAngles;
    }

    void LateUpdate()
    {
        // --- 1. Handle Position (Local) ---
        // Get current local position
        Vector3 currentLocalPos = transform.localPosition;

        // Overwrite only the locked axes
        float newX = lockLocalPosX ? targetLocalPos.x : currentLocalPos.x;
        float newY = lockLocalPosY ? targetLocalPos.y : currentLocalPos.y;
        float newZ = lockLocalPosZ ? targetLocalPos.z : currentLocalPos.z;

        // Apply back to LocalPosition
        transform.localPosition = new Vector3(newX, newY, newZ);


        // --- 2. Handle Rotation (World) ---
        // Get current world rotation
        Vector3 currentWorldRot = transform.eulerAngles;

        // Overwrite only the locked axes
        float newRotX = lockWorldRotX ? targetWorldRot.x : currentWorldRot.x;
        float newRotY = lockWorldRotY ? targetWorldRot.y : currentWorldRot.y;
        float newRotZ = lockWorldRotZ ? targetWorldRot.z : currentWorldRot.z;

        // Apply back to World Rotation (eulerAngles)
        transform.eulerAngles = new Vector3(newRotX, newRotY, newRotZ);
    }
}