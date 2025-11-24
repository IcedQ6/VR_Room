using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SimpleDialogueSystem : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("The TextMeshPro object that displays the dialogue.")]
    public TextMeshProUGUI textComponent;
    
    [Tooltip("The background image/panel (used to hide/show the box).")]
    public GameObject dialogueBoxVisuals;

    [Tooltip("The Button component. We disable this to prevent clicks.")]
    public Button advanceButton;

    [Tooltip("The Raycaster on the Canvas. We disable this to stop the VR ray from hitting the empty box.")]
    public GraphicRaycaster canvasRaycaster; 

    [Header("Settings")]
    [Tooltip("If true, events fire as soon as the line appears. If false, they fire when you click to leave the line.")]
    public bool triggerEventsOnStart = true;

    // Internal State
    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    private bool isDialogueActive = false;
    private DialogueLine currentLine;

    private void Start()
    {
        // 1. Try to find components automatically if not assigned
        if (textComponent == null) textComponent = GetComponentInChildren<TextMeshProUGUI>();
        if (advanceButton == null) advanceButton = GetComponentInChildren<Button>();
        if (canvasRaycaster == null) canvasRaycaster = GetComponentInParent<GraphicRaycaster>();

        // --- NEW FIX: Force Text to not block clicks ---
        if (textComponent != null) 
            textComponent.raycastTarget = false;

        // 2. Clear old text
        if(textComponent != null) textComponent.text = "";

        // 3. Hide visuals
        if(dialogueBoxVisuals != null) 
            dialogueBoxVisuals.SetActive(false);

        // 4. Disable interaction logic
        ToggleInteraction(false);
    }

    public void StartDialogue(List<DialogueLine> newLines)
    {
        lines.Clear();
        isDialogueActive = true;

        // Show UI
        if (dialogueBoxVisuals != null) dialogueBoxVisuals.SetActive(true);
        
        // Enable Interaction (Button + Raycaster)
        ToggleInteraction(true);

        foreach (DialogueLine line in newLines)
        {
            lines.Enqueue(line);
        }

        DisplayNextLine();
    }

    public void OnClickDialogueBox()
    {
        if (!isDialogueActive) return;

        if (!triggerEventsOnStart && currentLine != null)
        {
            currentLine.onLineEvent.Invoke();
        }

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();
        if(textComponent != null) textComponent.text = currentLine.text;

        if (triggerEventsOnStart)
        {
            currentLine.onLineEvent.Invoke();
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        if(textComponent != null) textComponent.text = "";
        
        // Hide visuals
        if (dialogueBoxVisuals != null) 
            dialogueBoxVisuals.SetActive(false);

        // Disable Interaction (Stop the ray from hitting the empty air)
        ToggleInteraction(false);
    }

    private void ToggleInteraction(bool state)
    {
        // Toggle the button logic
        if (advanceButton != null) advanceButton.enabled = state;

        // Toggle the physical hit detection
        // (This works for both standard GraphicRaycaster and TrackedDeviceGraphicRaycaster)
        if (canvasRaycaster != null) canvasRaycaster.enabled = state;
    }
}

// This simple class defines what a single "slide" of dialogue looks like
[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)] // Makes the text box bigger in the Inspector
    public string text;
    public UnityEvent onLineEvent;
}