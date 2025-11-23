using System.Collections.Generic;
using System.Linq; // Needed for fancy array checking
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SmartSocketValidator : MonoBehaviour
{
    // Define the types of logic this socket understands
    public enum ValidationMode
    {
        ExactSequence,      // Must match [1, 2, 3] exactly
        AnyOrderContains,   // Must contain [1, 2, 3] mixed up -> [3, 1, 2] is OK
        SumEquals,          // All numbers added together must equal X
        NoZerosPresent,     // Fails if any socket is empty or has value 0
        ContainsSpecific,   // Must contain at least one of value X
        CustomCheatCode     // Example: 999
    }

    [System.Serializable]
    public class ValidationRule
    {
        public string stateName = "State Name";
        public ValidationMode mode;
        
        [Tooltip("Used for Sequence, AnyOrder, or ContainsSpecific")]
        public int[] targetValues;
        
        [Tooltip("Used for SumEquals")]
        public int targetSum;

        [Space]
        public UnityEvent onConditionMet;
    }

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
        // 1. Search the object and all its children for the script
        PortableContainer container = insertedObj.GetComponentInChildren<PortableContainer>();

        // 2. Standard null check to see if we found it
        if (container != null)
        {
            // Get the raw data
            int[] values = container.GetCurrentValues();
    
            // ... Run your existing logic ...
            EvaluateStates(values); 
        }
        else
        {
            Debug.LogWarning($"[Socket] Object '{insertedObj.name}' has no PortableContainer script on it or its children.");
        }
    }

    private void EvaluateStates(int[] currentValues)
    {
        Debug.Log(string.Join(", ", currentValues));
        foreach (var rule in possibleStates)
        {
            if (CheckRule(rule, currentValues))
            {
                Debug.Log($"[SmartSocket] State Matched: {rule.stateName}");
                rule.onConditionMet.Invoke();
                return; // Stop checking after the first match!
            }
        }

        // If we get here, no rules matched
        Debug.Log("[SmartSocket] No state matched. Firing Fallback.");
        onNoStateMet.Invoke();
    }

    private bool CheckRule(ValidationRule rule, int[] input)
    {
        switch (rule.mode)
        {
            case ValidationMode.ExactSequence:
                // Length check + Content check
                if (rule.targetValues.Length != input.Length) return false;
                return input.SequenceEqual(rule.targetValues);

            case ValidationMode.AnyOrderContains:
                // Checks if the two arrays have the same elements, regardless of order
                if (rule.targetValues.Length != input.Length) return false;
                var sortedInput = input.OrderBy(x => x).ToArray();
                var sortedTarget = rule.targetValues.OrderBy(x => x).ToArray();
                return sortedInput.SequenceEqual(sortedTarget);

            case ValidationMode.SumEquals:
                return input.Sum() == rule.targetSum;

            case ValidationMode.NoZerosPresent:
                // Returns true if NO zeros exist in the array
                return !input.Contains(0);

            case ValidationMode.ContainsSpecific:
                // Returns true if the array contains the first number in targetValues
                if (rule.targetValues.Length == 0) return false;
                return input.Contains(rule.targetValues[0]);

            case ValidationMode.CustomCheatCode:
                // Example: If the first slot is 999, it's a dev cheat
                return input.Length > 0 && input[0] == 999;

            default:
                return false;
        }
    }
}