using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketGroupMemory : MonoBehaviour
{
    [Header("Configuration")]
    public List<XRSocketInteractor> sockets = new List<XRSocketInteractor>();

    // Memory Dictionary
    private Dictionary<XRSocketInteractor, XRBaseInteractable> savedItems = new Dictionary<XRSocketInteractor, XRBaseInteractable>();

    // State tracking to prevent "Double Hiding" (which deletes items)
    private bool isHidden = false;

    private void Awake()
    {
        if (sockets.Count == 0) GetComponentsInChildren(true, sockets);
    }

    public void SetGroupVisibility(bool isVisible)
    {
        if (isVisible)
            ShowGroup();
        else
            HideGroup();
    }

    private void HideGroup()
    {
        // SAFETY CHECK: If we are already hidden, STOP.
        // Running this twice wipes the 'savedItems' map and deletes your items.
        if (isHidden) return;

        savedItems.Clear();

        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. Save and Disable Items
            if (socket.hasSelection)
            {
                var item = socket.interactablesSelected[0] as XRBaseInteractable;
                if (item != null)
                {
                    savedItems.Add(socket, item);
                    if (item.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = true;
                    item.gameObject.SetActive(false); // Hide the gun/key
                }
            }

            // 2. Disable Components on the Socket
            socket.enabled = false;

            // 3. FORCE DISABLE GHOST VISUALS
            // We access the visual object directly to ensure it vanishes, 
            // even if the Ghost script doesn't handle OnDisable correctly.
            if (socket.TryGetComponent<SocketGhostVisuals>(out var ghost)) 
            {
                ghost.enabled = false;
                if (ghost.placeholderVisual != null) ghost.placeholderVisual.SetActive(false);
            }

            if (socket.TryGetComponent<Collider>(out var col)) col.enabled = false;
        }

        isHidden = true;
    }

    private void ShowGroup()
    {
        // SAFETY CHECK: If we are already visible, don't try to show again.
        if (!isHidden) return;

        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. Re-enable Components
            if (socket.TryGetComponent<Collider>(out var col)) col.enabled = true;
            
            // Re-enable Ghost Script (It will handle its own visual state)
            if (socket.TryGetComponent<SocketGhostVisuals>(out var ghost)) 
            {
                ghost.enabled = true;
                // We don't force SetActive(true) here; we let the ghost script decide 
                // if it should be visible (based on whether the socket is empty)
            }
            
            socket.enabled = true; 
            
            // 2. Restore Items
            if (savedItems.ContainsKey(socket))
            {
                var item = savedItems[socket];
                if (item != null)
                {
                    item.gameObject.SetActive(true);
                    if (item.TryGetComponent<Rigidbody>(out Rigidbody rb)) rb.isKinematic = false;

                    // Force Snap
                    var manager = socket.interactionManager;
                    if (manager != null)
                        manager.SelectEnter(socket as IXRSelectInteractor, item as IXRSelectInteractable);
                }
            }
        }
        
        savedItems.Clear();
        isHidden = false;
    }

    public GameObject GetObjectInSocket(XRSocketInteractor socket)
    {
        if (savedItems.ContainsKey(socket)) return savedItems[socket].gameObject;
        if (socket.hasSelection) return socket.interactablesSelected[0].transform.gameObject;
        return null;
    }
}