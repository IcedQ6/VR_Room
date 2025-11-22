using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PortableContainer : MonoBehaviour
{
    [Header("Sub-Sockets")]
    [Tooltip("Drag the child sockets here in order (Left to Right).")]
    public List<XRSocketInteractor> childSockets = new List<XRSocketInteractor>();

    /// <summary>
    /// Returns an array of integers representing the items currently in the sockets.
    /// Returns 0 for empty sockets.
    /// </summary>
    public int[] GetCurrentValues()
    {
        int[] results = new int[childSockets.Count];

        for (int i = 0; i < childSockets.Count; i++)
        {
            int val = 0;
            XRSocketInteractor socket = childSockets[i];

            if (socket.hasSelection)
            {
                // Grab the object
                var obj = socket.interactablesSelected[0].transform.gameObject;
                
                // Read the ItemData (from the script we made earlier)
                if (obj.TryGetComponent<ItemData>(out ItemData data))
                {
                    val = data.itemValue;
                }
            }
            results[i] = val;
        }

        return results;
    }
}