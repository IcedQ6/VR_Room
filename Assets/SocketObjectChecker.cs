using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Be sure to add this!

public class SocketObjectChecker : MonoBehaviour
{
    [Tooltip("Drag your Socket Interactor here in the Inspector.")]
    [SerializeField]
    private XRSocketInteractor socket;

    // A public variable to see the object in the inspector (optional)
    public GameObject objectInSocket;

    void Start()
    {
        // Make sure the socket is assigned
        if (socket == null)
        {
            Debug.Log("Socket is not assigned!", this);
            enabled = false;
        }
    }

    // You can call this function from an event or other script
    public void CheckSocket()
    {
        // Get the interactable that is currently in the socket
        IXRSelectInteractable interactableInSocket = socket.firstInteractableSelected;

        if (interactableInSocket != null)
        {
            // --- 1. Get the GameObject ---
            // The .transform property is part of the interface, so this is safe
            objectInSocket = interactableInSocket.transform.gameObject;
            Debug.Log($"Socket contains: {objectInSocket.name}");
        }
        else
        {
            // Socket is empty
            objectInSocket = null;
            Debug.Log("Socket is empty.");
        }
    }
    
    // Sets Active of GameObject attached to socket based on bool parameter
    public void ChangeEnableOfSocket(bool enable)
    {
        // Get the interactable that is currently in the socket
        IXRSelectInteractable interactableInSocket = socket.firstInteractableSelected;

        if (interactableInSocket != null)
        {
            objectInSocket = interactableInSocket.transform.gameObject;
            objectInSocket.SetActive(enable);
        }
        else
        {
            // Socket is empty
            objectInSocket = null;
            Debug.Log("Socket is empty.");
        }
    }
}