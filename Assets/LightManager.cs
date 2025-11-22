using System.Collections.Generic;
using System.Linq; // Used for easy checking of the array
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Drag the GameObjects (e.g., Lamps) here. The script will find the lights inside them.")]
    public GameObject[] lightObjects;

    [Header("Debug")]
    [Tooltip("Enter numbers here and right-click component -> 'Test Update Lights' to test.")]
    public int[] debugIndices;

    /// <summary>
    /// Turns ON lights at the specified indices, and turns OFF all others.
    /// </summary>
    /// <param name="activeIndices">Array of integers representing the index in 'lightObjects' to turn on.</param>
    public void UpdateLights(int[] activeIndices)
    {
        // 1. Convert array to a HashSet for fast lookup
        HashSet<int> activeSet = new HashSet<int>(activeIndices);

        // 2. Iterate through every object in our main list
        for (int i = 0; i < lightObjects.Length; i++)
        {
            if (lightObjects[i] == null) continue;

            // Determine if this specific index should be ON or OFF
            bool turnOn = activeSet.Contains(i);

            // 3. Find all Light components inside this object (children)
            Light[] lightsInChildren = lightObjects[i].GetComponentsInChildren<Light>(true);

            // 4. Apply the state
            foreach (Light l in lightsInChildren)
            {
                l.enabled = turnOn;
            }

            // OPTIONAL: If you have "Emissive" meshes (glowing glass), you could toggle them here too.
        }
    }
    
    // --- OVERLOAD for easier use with UnityEvents ---
    // Unity Events can't pass int[], but they can pass a single int. 
    // This turns on ONE light and turns off the rest.
    public void SetSingleLightActive(int index)
    {
        UpdateLights(new int[] { index });
    }

    // --- DEBUGGING TOOLS ---
    [ContextMenu("Test Update Lights")]
    public void DebugTest()
    {
        UpdateLights(debugIndices);
    }

    [ContextMenu("Turn All Off")]
    public void DebugOff()
    {
        UpdateLights(new int[0]); // Pass empty array
    }
}