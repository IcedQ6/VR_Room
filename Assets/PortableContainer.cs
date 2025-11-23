using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PortableContainer : MonoBehaviour
{
    [Header("Sub-Sockets")]
    [Tooltip("Drag the child sockets here in order (Left to Right).")]
    public List<XRSocketInteractor> childSockets = new List<XRSocketInteractor>();

    // Reference to the memory script (optional)
    private SocketGroupMemory memoryScript;

    private void Awake()
    {
        // Try to find the memory script on this same object
        memoryScript = GetComponent<SocketGroupMemory>();
    }

    public int[] GetCurrentValues()
    {
        int[] results = new int[childSockets.Count];

        for (int i = 0; i < childSockets.Count; i++)
        {
            int val = 0;
            XRSocketInteractor socket = childSockets[i];
            GameObject foundObj = null;

            // --- SMART CHECK START ---
            
            // A. If we have the Memory script, ask it for the object (Hidden or Visible)
            if (memoryScript != null)
            {
                foundObj = memoryScript.GetObjectInSocket(socket);
            }
            // B. If no Memory script, just check the physical socket
            else if (socket.hasSelection)
            {
                foundObj = socket.interactablesSelected[0].transform.gameObject;
            }
            
            // --- SMART CHECK END ---

            // If we found an object (even if it's invisible/disabled), read its data
            if (foundObj != null)
            {
                // Note: GetComponent works even if the object is disabled
                if (foundObj.TryGetComponent<ItemData>(out ItemData data))
                {
                    val = data.itemValue;
                }
            }

            results[i] = val;
        }

        return results;
    }
}