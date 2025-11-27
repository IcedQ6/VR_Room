using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CharacterEventTrigger : MonoBehaviour
{
    [Header("Filtering")]
    [Tooltip("If set, only objects with this Tag will trigger the event (e.g. 'Player').")]
    public string targetTag = "Player";

    [Tooltip("If true, the object MUST have a CharacterController component to trigger this.")]
    public bool requireCharacterController = true;

    [Header("Settings")]
    [Tooltip("If true, the Enter event will only fire once and then disable itself.")]
    public bool triggerOnceOnly = false;

    [Header("Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerExit;

    private bool hasTriggered = false;

    private void Awake()
    {
        // Safety check to ensure the collider is actually a trigger
        Collider col = GetComponent<Collider>();
        if (!col.isTrigger)
        {
            Debug.LogWarning($"[CharacterEventTrigger] The Collider on {name} is not set to 'Is Trigger'. Events may not fire.", this);
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnceOnly && hasTriggered) return;

        if (IsValidObject(other))
        {
            Debug.Log($"[EventTrigger] Player entered {name}");
            onTriggerEnter.Invoke();
            hasTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Note: We don't check 'triggerOnceOnly' for exit, usually you want the exit event to finish up
        if (IsValidObject(other))
        {
            Debug.Log($"[EventTrigger] Player exited {name}");
            onTriggerExit.Invoke();
        }
    }

    private bool IsValidObject(Collider other)
    {
        // 1. Tag Check (Optimization: Check tag first as it's cheap)
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag))
        {
            return false;
        }

        // 2. Component Check (Slower, but accurate for "Character Controller" request)
        if (requireCharacterController)
        {
            // Look for the CharacterController on the object or its parent (common in VR rigs)
            CharacterController cc = other.GetComponentInParent<CharacterController>();
            if (cc == null) return false;
        }

        return true;
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}