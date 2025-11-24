using UnityEngine;

public class Billboard : MonoBehaviour
{
    [Tooltip("If true, the object will only rotate on the Y axis (like a tree sprite). False = faces camera directly.")]
    public bool lockYAxis = false;

    private Camera mainCamera;

    void LateUpdate()
    {
        // Lazy Loading: Only look for the camera if we don't have one yet.
        if (mainCamera == null) 
        {
            mainCamera = Camera.main;
            
            // If we still can't find it (e.g., game just started), stop here to prevent errors.
            if (mainCamera == null) return;
        }

        // 1. Calculate the direction to look at the camera
        // We use (transform.position + camera.forward) to look in the same direction as the camera
        // which makes the UI face the camera properly (instead of being mirrored)
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);

        // 2. Optional: Lock rotation if you want it to stay upright (vertical)
        if (lockYAxis)
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0;
            eulerAngles.z = 0;
            transform.eulerAngles = eulerAngles;
        }
    }
}