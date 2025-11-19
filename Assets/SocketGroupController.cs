using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketGroupController : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("If empty, we will find them automatically.")]
    public List<XRSocketInteractor> sockets = new List<XRSocketInteractor>();

    [Header("Settings")]
    [Tooltip("Force all sockets to be visible and usable when the game starts?")]
    public bool forceShowOnStart = true;

    private void Awake()
    {
        // 1. Auto-find if list is empty
        if (sockets.Count == 0)
        {
            // Include inactive children in search just in case
            GetComponentsInChildren(true, sockets);
        }
    }

    private void Start()
    {
        // 2. SAFETY CHECK: Force everything ON at the start
        if (forceShowOnStart)
        {
            Debug.Log($"[SocketController] Force resetting {sockets.Count} sockets to Visible/Usable.");
            SetGroupVisibility(true);
        }
    }

    public void SetGroupVisibility(bool isVisible)
    {
        Debug.Log($"[SocketController] Setting Visibility: {isVisible}");

        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // --- A. Force the Socket GameObject Active ---
            // (If the GameObject is off, nothing else matters)
            if (!socket.gameObject.activeSelf) 
                socket.gameObject.SetActive(true);

            // --- B. Handle the Socket Visuals/Physics ---
            ToggleVisualsAndPhysics(socket.gameObject, isVisible);

            // --- C. Handle the Item inside (if any) ---
            if (socket.hasSelection)
            {
                // Note: In newer XRI, interactablesSelected is a list of IXRSelectInteractable
                var item = socket.interactablesSelected[0].transform.gameObject;
                ToggleVisualsAndPhysics(item, isVisible);
                
                // Ensure the item's script is enabled so it can be interacted with
                var interactableComp = item.GetComponent<XRBaseInteractable>();
                if (interactableComp != null) interactableComp.enabled = isVisible;
            }
        }
    }

    private void ToggleVisualsAndPhysics(GameObject target, bool state)
    {
        // 1. Renderers (Mesh)
        var renderers = target.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) r.enabled = state;

        // 2. Colliders (Physics/Touch)
        var colliders = target.GetComponentsInChildren<Collider>(true);
        foreach (var c in colliders) c.enabled = state;

        // 3. Canvases (World UI)
        var canvases = target.GetComponentsInChildren<Canvas>(true);
        foreach (var c in canvases) c.enabled = state;
    }
    
    // Debug Tools
    [ContextMenu("Force SHOW All")]
    public void DebugShow() => SetGroupVisibility(true);

    [ContextMenu("Force HIDE All")]
    public void DebugHide() => SetGroupVisibility(false);
}