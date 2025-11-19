using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [Tooltip("If true, lights are found every time you toggle. If false, they are only found once at Start (better performance).")]
    [SerializeField]
    private bool searchDynamically = false;

    // Array to store references to all the lights
    private Light[] childLights;

    void Awake()
    {
        // Initial fetch of lights
        FetchLights();
    }

    /// <summary>
    /// Finds all lights in children and updates the list.
    /// </summary>
    public void FetchLights()
    {
        childLights = GetComponentsInChildren<Light>(true); // 'true' includes inactive children
        Debug.Log($"[GroupLightController] Found {childLights.Length} lights in {name}");
    }

    /// <summary>
    /// Turns all child lights ON.
    /// </summary>
    public void TurnAllOn()
    {
        SetAllLights(true);
    }

    /// <summary>
    /// Turns all child lights OFF.
    /// </summary>
    public void TurnAllOff()
    {
        SetAllLights(false);
    }

    /// <summary>
    /// Toggles the state of the lights (On -> Off, Off -> On).
    /// </summary>
    public void ToggleLights()
    {
        if (searchDynamically) FetchLights();

        foreach (Light l in childLights)
        {
            if (l != null)
            {
                l.enabled = !l.enabled;
            }
        }
    }

    // Internal helper function
    private void SetAllLights(bool state)
    {
        if (searchDynamically) FetchLights();

        foreach (Light l in childLights)
        {
            if (l != null)
            {
                l.enabled = state;
            }
        }
    }
}
