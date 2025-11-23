using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketGroupMemory : MonoBehaviour
{
    [Header("Configuration")]
    public List<XRSocketInteractor> sockets = new List<XRSocketInteractor>();

    // This Dictionary acts as our "Memory"
    // It links a Socket to the Item it was holding
    private Dictionary<XRSocketInteractor, XRBaseInteractable> savedItems = new Dictionary<XRSocketInteractor, XRBaseInteractable>();

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
        // CLEAR memory before we start hiding things
        savedItems.Clear();

        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. MEMORIZE: If the socket has an item, remember it!
            if (socket.hasSelection)
            {
                // Get the item (works for XRI 2.x and 3.x)
                var item = socket.interactablesSelected[0] as XRBaseInteractable;
                
                if (item != null)
                {
                    // Save to dictionary
                    savedItems.Add(socket, item);

                    // Freeze the item so it doesn't fall away while hidden
                    if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {
                        rb.isKinematic = true;
                    }

                    // Hide the item
                    ToggleObject(item.gameObject, false);
                }
            }

            // 2. Hide the Socket itself
            ToggleObject(socket.gameObject, false);
        }
    }

    private void ShowGroup()
    {
        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. Show the Socket first
            ToggleObject(socket.gameObject, true);
            
            // 2. RECALL: Did this socket have an item?
            if (savedItems.ContainsKey(socket))
            {
                var item = savedItems[socket];

                // Show the item
                ToggleObject(item.gameObject, true);

                // Unfreeze the item (allow physics again)
                if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = false;
                }

                // CRITICAL STEP: Force the socket to grab the item again manually
                // We use the Interaction Manager to force the connection
                var manager = socket.interactionManager;
                if (manager != null && item != null)
                {
                    manager.SelectEnter(socket as IXRSelectInteractor, item as IXRSelectInteractable);
                }
            }
        }
        
        // Clear memory after restoring
        savedItems.Clear();
    }

    private void ToggleObject(GameObject obj, bool state)
    {
        // Toggle Renderers
        foreach (var r in obj.GetComponentsInChildren<Renderer>()) r.enabled = state;
        
        // Toggle Colliders (This causes the drop, but our Memory fixes it)
        foreach (var c in obj.GetComponentsInChildren<Collider>()) c.enabled = state;
        
        // Toggle Canvases
        foreach (var c in obj.GetComponentsInChildren<Canvas>()) c.enabled = state;
    }
    
    /// <summary>
    /// Returns the object associated with a socket, whether it is physically there 
    /// OR currently hidden in memory.
    /// </summary>
    public GameObject GetObjectInSocket(XRSocketInteractor socket)
    {
        // 1. Check Memory first (Is it hidden?)
        if (savedItems.ContainsKey(socket))
        {
            var item = savedItems[socket];
            if (item != null) return item.gameObject;
        }

        // 2. Check Reality second (Is it visible/snapped?)
        if (socket.hasSelection)
        {
            // Handle XRI 2.x and 3.x
            return socket.interactablesSelected[0].transform.gameObject;
        }

        // 3. Socket is truly empty
        return null;
    }
}