using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class SocketGhostVisuals : MonoBehaviour
{
    [Tooltip("The mesh/object to show when the socket is empty.")]
    public GameObject placeholderVisual;

    private XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    void OnEnable()
    {
        // Listen for Hover (Coming near)
        socket.hoverEntered.AddListener(OnStateChange);
        socket.hoverExited.AddListener(OnStateChange);
        
        // Listen for Select (Snapping in)
        socket.selectEntered.AddListener(OnStateChange);
        socket.selectExited.AddListener(OnStateChange);

        // Set initial state
        UpdateVisualState();
    }

    void OnDisable()
    {
        socket.hoverEntered.RemoveListener(OnStateChange);
        socket.hoverExited.RemoveListener(OnStateChange);
        socket.selectEntered.RemoveListener(OnStateChange);
        socket.selectExited.RemoveListener(OnStateChange);
    }

    // We use a generic handler because we just need to re-evaluate the state whenever anything changes
    private void OnStateChange(BaseInteractionEventArgs args)
    {
        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (placeholderVisual == null) return;

        // Check 1: Is something snapped in?
        bool isHoldingItem = socket.hasSelection;

        // Check 2: Is something hovering (near the trigger)?
        // Note: interactablesHovered is the list of valid items currently in the trigger zone
        bool isHoveringItem = socket.interactablesHovered.Count > 0;

        // If EITHER is true, we hide the ghost to prevent visual clutter
        if (isHoldingItem || isHoveringItem)
        {
            placeholderVisual.SetActive(false);
        }
        else
        {
            placeholderVisual.SetActive(true);
        }
    }
}