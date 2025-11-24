using System.Collections.Generic;
using UnityEngine;

public class ConversationTrigger : MonoBehaviour
{
    [Header("Configuration")]
    public SimpleDialogueSystem dialogueSystem; // Drag the System here

    [Header("Content")]
    public List<DialogueLine> conversation = new List<DialogueLine>();

    // Call this via a Button, a Trigger Enter, or another script
    public void TriggerDialogue()
    {
        if (dialogueSystem != null)
        {
            dialogueSystem.StartDialogue(conversation);
        }
        else
        {
            Debug.LogError("Dialogue System not assigned on " + gameObject.name);
        }
    }

    // Optional: Auto-trigger for testing
    /*
    private void Start()
    {
        TriggerDialogue();
    }
    */
}