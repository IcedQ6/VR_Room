using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SmartSocketValidator : MonoBehaviour
{
    public enum ValidationMode
    {
        ExactSequence,
        AnyOrderContains,
        SumEquals,
        NoZerosPresent,
        ContainsSpecific,
        CustomCheatCode
    }

    [System.Serializable]
    public class ValidationRule
    {
        public string stateName = "State Name";
        public ValidationMode mode;
        public int[] targetValues;
        public int targetSum;
        [Space]
        public UnityEvent onConditionMet;
    }

    [Header("First Time Override")]
    [Tooltip("If true, the 'On First Insert' event will fire the VERY first time a valid object is placed, ignoring all other rules.")]
    public bool enableFirstTimeOverride = false;
    
    [Tooltip("Fires only once, the first time a valid container is inserted.")]
    public UnityEvent onFirstInsert;

    private bool hasInteractionOccurred = false;

    [Header("State Machine")]
    [Tooltip("The logic checks these in order. The FIRST one that returns true fires.")]
    public List<ValidationRule> possibleStates = new List<ValidationRule>();

    [Header("Fallback")]
    public UnityEvent onNoStateMet;

    private XRSocketInteractor mainSocket;

    private void Awake()
    {
        mainSocket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        mainSocket.selectEntered.AddListener(OnObjectInserted);
    }

    private void OnDisable()
    {
        mainSocket.selectEntered.RemoveListener(OnObjectInserted);
    }

    private void OnObjectInserted(SelectEnterEventArgs args)
    {
        GameObject insertedObj = args.interactableObject.transform.gameObject;

        // 1. Search for the container script (root or children)
        // We look for 'true' (inactive objects) to ensure we find it even if the container is closed/hidden
        PortableContainer container = insertedObj.GetComponentInChildren<PortableContainer>(true);

        if (container != null)
        {
            // --- NEW: FIRST TIME OVERRIDE ---
            if (enableFirstTimeOverride && !hasInteractionOccurred)
            {
                Debug.Log($"[SmartSocket] First time interaction detected on {name}. Overriding logic.");
                hasInteractionOccurred = true;
                onFirstInsert.Invoke();
                return; // CRITICAL: Stop here. Do not check puzzle logic.
            }
            // --------------------------------

            // 2. Get the raw data
            int[] values = container.GetCurrentValues();
            Debug.Log($"[SmartSocket] Read values: {string.Join(",", values)}");

            // 3. Run the State Machine
            EvaluateStates(values);
        }
        else
        {
            Debug.LogWarning($"[Socket] Object '{insertedObj.name}' has no PortableContainer script.");
        }
    }

    private void EvaluateStates(int[] currentValues)
    {
        foreach (var rule in possibleStates)
        {
            if (CheckRule(rule, currentValues))
            {
                Debug.Log($"[SmartSocket] State Matched: {rule.stateName}");
                rule.onConditionMet.Invoke();
                return; 
            }
        }

        Debug.Log("[SmartSocket] No state matched. Firing Fallback.");
        onNoStateMet.Invoke();
    }

    private bool CheckRule(ValidationRule rule, int[] input)
    {
        switch (rule.mode)
        {
            case ValidationMode.ExactSequence:
                if (rule.targetValues.Length != input.Length) return false;
                return input.SequenceEqual(rule.targetValues);

            case ValidationMode.AnyOrderContains:
                if (rule.targetValues.Length != input.Length) return false;
                var sortedInput = input.OrderBy(x => x).ToArray();
                var sortedTarget = rule.targetValues.OrderBy(x => x).ToArray();
                return sortedInput.SequenceEqual(sortedTarget);

            case ValidationMode.SumEquals:
                return input.Sum() == rule.targetSum;

            case ValidationMode.NoZerosPresent:
                return !input.Contains(0);

            case ValidationMode.ContainsSpecific:
                if (rule.targetValues.Length == 0) return false;
                return input.Contains(rule.targetValues[0]);

            case ValidationMode.CustomCheatCode:
                return input.Length > 0 && input[0] == 999;

            default:
                return false;
        }
    }
    
    // Optional: Call this via another event to reset the "First Time" status
    public void ResetFirstTimeStatus()
    {
        hasInteractionOccurred = false;
    }
}