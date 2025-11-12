using UnityEngine;




/// <summary>
/// A helper script to set Rigidbody constraints from UnityEvents.
/// This allows you to choose which axes to freeze or unfreeze for
/// position and rotation separately.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyConstraintHelper : MonoBehaviour
{
    // (The enum definition is no longer in here)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Sets the position constraints, preserving any existing rotation constraints.
    /// </summary>
    /// <param name="axesToFreeze">The position axes you want to freeze.</param>
    public void SetPositionFreeze(string axis)
    {
        if (rb == null) return;

        // Get the current constraints
        RigidbodyConstraints newConstraints = rb.constraints;

        // 1. Clear all *existing* position constraints
        newConstraints &= ~RigidbodyConstraints.FreezePosition;

        // 2. Add back the ones specified by the enum
        switch (axis)
        {
            case "None":
                break;
            case "X":
                newConstraints |= RigidbodyConstraints.FreezePositionX;
                break;
            case "All":
                newConstraints |= RigidbodyConstraints.FreezePosition;
                break;
            /*
            case ConstraintSelection.Y:
                newConstraints |= RigidbodyConstraints.FreezePositionY;
                break;
            case ConstraintSelection.Z:
                newConstraints |= RigidbodyConstraints.FreezePositionZ;
                break;
            case ConstraintSelection.XY:
                newConstraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
                break;
            case ConstraintSelection.XZ:
                newConstraints |= RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                break;
            case ConstraintSelection.YZ:
                newConstraints |= RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                break;
            case ConstraintSelection.All:
                newConstraints |= RigidbodyConstraints.FreezePosition;
                break;
            */
        }

        // Apply the new combined constraints
        rb.constraints = newConstraints;
    }

    /// <summary>
    /// Sets the rotation constraints, preserving any existing position constraints.
    /// </summary>
    /// <param name="axesToFreeze">The rotation axes you want to freeze.</param>
    public void SetRotationFreeze(ConstraintSelection axesToFreeze)
    {
        if (rb == null) return;

        // Get the current constraints
        RigidbodyConstraints newConstraints = rb.constraints;

        // 1. Clear all *existing* rotation constraints
        newConstraints &= ~RigidbodyConstraints.FreezeRotation;

        // 2. Add back the ones specified by the enum
        switch (axesToFreeze)
        {
            case ConstraintSelection.None:
                break;
            case ConstraintSelection.X:
                newConstraints |= RigidbodyConstraints.FreezeRotationX;
                break;
            case ConstraintSelection.Y:
                newConstraints |= RigidbodyConstraints.FreezeRotationY;
                break;
            case ConstraintSelection.Z:
                newConstraints |= RigidbodyConstraints.FreezeRotationZ;
                break;
            case ConstraintSelection.XY:
                newConstraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                break;
            case ConstraintSelection.XZ:
                newConstraints |= RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                break;
            case ConstraintSelection.YZ:
                newConstraints |= RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            case ConstraintSelection.All:
                newConstraints |= RigidbodyConstraints.FreezeRotation;
                break;
        }

        // Apply the new combined constraints
        rb.constraints = newConstraints;
    }
}