using System.Collections.Generic;
using System.Text; // Used for building the debug string neatly
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketSequenceReader : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Drag your sockets here in the EXACT order you want to read them.")]
    public List<XRSocketInteractor> orderedSockets = new List<XRSocketInteractor>();

    [Header("Debug")]
    [Tooltip("Check this to print '0' for empty sockets. Uncheck to skip them.")]
    public bool treatEmptyAsZero = true;

    // Use this Context Menu to test it from the Inspector while playing
    [ContextMenu("Read Values Now")]
    public void ReadAndPrintValues()
    {
        StringBuilder resultString = new StringBuilder();
        resultString.Append("Socket Sequence: [ ");

        // 1. Loop through the sockets in the order defined in the list
        for (int i = 0; i < orderedSockets.Count; i++)
        {
            int foundValue = 0;
            bool hasItem = false;

            XRSocketInteractor socket = orderedSockets[i];

            // 2. Check if the socket is holding anything
            if (socket != null && socket.hasSelection)
            {
                // Get the object currently inside
                // (We use transform.gameObject to get the actual object reference)
                var heldObject = socket.interactablesSelected[0].transform.gameObject;

                // 3. Try to find the ItemData script on that object
                if (heldObject.TryGetComponent<ItemData>(out ItemData data))
                {
                    foundValue = data.itemValue;
                    hasItem = true;
                }
                else
                {
                    Debug.LogWarning($"Socket {i} has an object ({heldObject.name}), but it has no ItemData script!");
                }
            }

            // 4. Format the output
            if (hasItem)
            {
                resultString.Append(foundValue + " ");
            }
            else if (treatEmptyAsZero)
            {
                resultString.Append("0(Empty) ");
            }
            else
            {
                resultString.Append("- ");
            }
        }

        resultString.Append("]");
        
        // 5. Print to Console
        Debug.Log(resultString.ToString());
    }

    public bool checkForNoZeros()
    {
        StringBuilder resultString = new StringBuilder();
        resultString.Append("Socket Sequence: [ ");

        // 1. Loop through the sockets in the order defined in the list
        for (int i = 0; i < orderedSockets.Count; i++)
        {
            int foundValue = 0;
            bool hasItem = false;

            XRSocketInteractor socket = orderedSockets[i];

            // 2. Check if the socket is holding anything
            if (socket != null && socket.hasSelection)
            {
                // Get the object currently inside
                // (We use transform.gameObject to get the actual object reference)
                var heldObject = socket.interactablesSelected[0].transform.gameObject;

                // 3. Try to find the ItemData script on that object
                if (heldObject.TryGetComponent<ItemData>(out ItemData data))
                {
                    foundValue = data.itemValue;
                    hasItem = true;
                }
                else
                {
                    Debug.LogWarning($"Socket {i} has an object ({heldObject.name}), but it has no ItemData script!");
                }
            }

            // 4. Format the output
            if (hasItem)
            {
                resultString.Append(foundValue + " ");
            }
            else if (treatEmptyAsZero)
            {
                resultString.Append("0(Empty) ");
            }
            else
            {
                resultString.Append("- ");
            }
        }

        return true;
    }

    // Helper to auto-fill the list based on Hierarchy order
    [ContextMenu("Auto-Find Sockets (Hierarchy Order)")]
    public void AutoPopulateSockets()
    {
        orderedSockets.Clear();
        GetComponentsInChildren(true, orderedSockets);
        Debug.Log($"Found {orderedSockets.Count} sockets.");
    }
}