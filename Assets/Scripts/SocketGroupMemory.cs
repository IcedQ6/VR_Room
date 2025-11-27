using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketGroupMemory : MonoBehaviour
{
    [Header("Configuration")]
    public List<XRSocketInteractor> sockets = new List<XRSocketInteractor>();

    // Memory Dictionary
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
        savedItems.Clear();

        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. If the socket is holding something, save it and disable it
            if (socket.hasSelection)
            {
                var item = socket.interactablesSelected[0] as XRBaseInteractable;
                
                if (item != null)
                {
                    savedItems.Add(socket, item);

                    // Freeze physics so it stays in place
                    if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {
                        rb.isKinematic = true;
                    }

                    // NUCLEAR OPTION: Turn the item object completely off.
                    // This guarantees 100% invisibility and stops all interaction scripts.
                    item.gameObject.SetActive(false);
                }
            }

            // 2. Turn the Socket object completely off.
            // This stops the 'SocketGhostVisuals' script from running and hides the ghost mesh.
            socket.gameObject.SetActive(false);
        }
    }

    private void ShowGroup()
    {
        foreach (var socket in sockets)
        {
            if (socket == null) continue;

            // 1. Turn the socket back on
            socket.gameObject.SetActive(true);
            
            // 2. Restore the item if we had one
            if (savedItems.ContainsKey(socket))
            {
                var item = savedItems[socket];

                if (item != null)
                {
                    // Turn the item back on
                    item.gameObject.SetActive(true);

                    // Unfreeze physics
                    if (item.TryGetComponent<Rigidbody>(out Rigidbody rb))
                    {
                        rb.isKinematic = false;
                    }

                    // Force the snap immediately so the socket reclaims the item
                    var manager = socket.interactionManager;
                    if (manager != null)
                    {
                        manager.SelectEnter(socket as IXRSelectInteractor, item as IXRSelectInteractable);
                    }
                }
            }
        }
        
        savedItems.Clear();
    }

    /// <summary>
    /// Helper for the PortableContainer script to find items even when they are disabled/hidden.
    /// </summary>
    public GameObject GetObjectInSocket(XRSocketInteractor socket)
    {
        // Check Memory first (Hidden Items)
        if (savedItems.ContainsKey(socket))
        {
            var item = savedItems[socket];
            if (item != null) return item.gameObject;
        }

        // Check Reality second (Visible Items)
        if (socket.hasSelection)
        {
            return socket.interactablesSelected[0].transform.gameObject;
        }

        return null;
    }
}