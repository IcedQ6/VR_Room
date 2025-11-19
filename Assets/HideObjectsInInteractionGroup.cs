using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HideObjectsInInteractionGroup : MonoBehaviour
{
    [Tooltip("Drag the GameObject that acts as the parent or root of your group here.")]
    [SerializeField]
    private GameObject groupRootObject;

    // This array will be populated with the results
    private XRSocketInteractor[] allSocketsInGroup;

    void Start()
    {
        FindAllSockets();
    }
    
    public void FindAllSockets()
    {
        if (groupRootObject == null)
        {
            Debug.LogError("Group Root Object is not assigned!", this);
            return;
        }
        
        allSocketsInGroup = groupRootObject.GetComponentsInChildren<XRSocketInteractor>();

        // --- Log the results ---
        Debug.Log($"Found {allSocketsInGroup.Length} sockets in '{groupRootObject.name}':");
        foreach (XRSocketInteractor socket in allSocketsInGroup)
        {
            Debug.Log($"- {socket.gameObject.name}", socket.gameObject);
        }
    }
}

