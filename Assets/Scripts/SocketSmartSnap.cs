using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketSmartSnap : MonoBehaviour, IXRSelectFilter
{
    [Header("Settings")]
    [Tooltip("How long (seconds) after releasing the object does the socket have to catch it?")]
    public float dropGracePeriod = 0.25f;

    private XRSocketInteractor socket;
    
    // List of objects currently hovering that we are "watching"
    private HashSet<IXRSelectInteractable> watchedObjects = new HashSet<IXRSelectInteractable>();
    
    // List of objects that were JUST dropped and are allowed to snap
    private HashSet<IXRSelectInteractable> validDropTargets = new HashSet<IXRSelectInteractable>();

    public bool canProcess => this.enabled;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socket.hoverEntered.AddListener(OnHoverEnter);
        socket.hoverExited.AddListener(OnHoverExit);
    }

    private void OnDisable()
    {
        socket.hoverEntered.RemoveListener(OnHoverEnter);
        socket.hoverExited.RemoveListener(OnHoverExit);
        
        // Cleanup listeners to prevent memory leaks
        foreach (var item in watchedObjects)
        {
            item.selectExited.RemoveListener(OnItemDropped);
        }
        watchedObjects.Clear();
    }

    // --- 1. TRACKING LOGIC ---

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactableObject is IXRSelectInteractable item)
        {
            // If the item is currently held, we want to watch it
            if (item.interactorsSelecting.Count > 0)
            {
                // Subscribe to the item's "Drop" event
                item.selectExited.AddListener(OnItemDropped);
                watchedObjects.Add(item);
            }
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (args.interactableObject is IXRSelectInteractable item)
        {
            // If it leaves the socket, stop watching it
            if (watchedObjects.Contains(item))
            {
                item.selectExited.RemoveListener(OnItemDropped);
                watchedObjects.Remove(item);
            }
        }
    }

    // --- 2. THE "DROP" EVENT ---
    
    // This runs ONLY when the player lets go of an object while it's inside the socket
    private void OnItemDropped(SelectExitEventArgs args)
    {
        var item = args.interactableObject;
        
        // Add to the "Allowed" list
        validDropTargets.Add(item);
        
        // Start a timer to remove it from the allowed list shortly
        StartCoroutine(ClearValidityRoutine(item));
    }

    private IEnumerator ClearValidityRoutine(IXRSelectInteractable item)
    {
        yield return new WaitForSeconds(dropGracePeriod);
        validDropTargets.Remove(item);
    }

    // --- 3. THE FILTER DECISION ---

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        // A. DIRECT HANDOFF: If the player is actively holding it (forcing it in), allow it.
        if (interactable.interactorsSelecting.Count > 0) return true;

        // B. RECENT DROP: If the player JUST let go of it inside the socket, allow it.
        if (validDropTargets.Contains(interactable)) return true;

        // C. GRAVITY/THROWN: Otherwise (random physics object), reject it.
        return false;
    }
}