using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script provides public functions to modify a Rigidbody's constraints,
/// making them callable from UnityEvents (like buttons or interactables).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyConstraintHelper : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        // Get the Rigidbody on this same GameObject
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Freezes all positional movement (X, Y, Z)
    /// while preserving any existing rotation constraints.
    /// </summary>
    public void FreezePosition()
    {
        if (rb != null)
        {
            // Use a bitwise OR to add the FreezePosition flags
            // without affecting the FreezeRotation flags.
            rb.constraints = rb.constraints | RigidbodyConstraints.FreezePosition;
        }
    }

    /// <summary>
    /// Unfreezes all positional movement (X, Y, Z)
    /// while preserving any existing rotation constraints.
    /// </summary>
    public void UnfreezePosition()
    {
        if (rb != null)
        {
            // Use a bitwise AND NOT to remove only the FreezePosition flags
            // without affecting the FreezeRotation flags.
            rb.constraints = rb.constraints & ~RigidbodyConstraints.FreezePosition;
        }
    }
    
    /// <summary>
    /// Freezes all rotation (X, Y, Z)
    /// while preserving any existing position constraints.
    /// </summary>
    public void FreezeRotation()
    {
        if (rb != null)
        {
            rb.constraints = rb.constraints | RigidbodyConstraints.FreezeRotation;
        }
    }

    /// <summary>
    /// Unfreezes all rotation (X, Y, Z)
    /// while preserving any existing position constraints.
    /// </summary>
    public void UnfreezeRotation()
    {
        if (rb != null)
        {
            rb.constraints = rb.constraints & ~RigidbodyConstraints.FreezeRotation;
        }
    }
}
