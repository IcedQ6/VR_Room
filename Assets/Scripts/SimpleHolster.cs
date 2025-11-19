using System;
using UnityEngine;

public class SimpleHolster : MonoBehaviour
{
    public Transform headCamera; // Assign your Main Camera here
    public float offsetDown = 0.4f; // How far down from head (waist height)
    public float offsetSide = 0.25f; // How far to the side (right hip)

    private void Awake()
    {
        if (headCamera == null)
        {
            Debug.LogError("Head camera not set");
        }
    }

    void Update()
    {
        // 1. Position: Follow the head, but offset down to the waist
        Vector3 targetPosition = headCamera.position;
        targetPosition.y -= offsetDown; // Move down to waist

        // 2. Rotation: Match the head's "Yaw" (looking left/right) but ignore looking up/down
        Quaternion targetRotation = Quaternion.Euler(0, headCamera.eulerAngles.y, 0);

        // 3. Apply Offset: Move it to the side relative to where we are facing
        // We use targetRotation * vector to move "Right" relative to our body direction
        transform.position = targetPosition + (targetRotation * new Vector3(offsetSide, 0, 0));
        transform.rotation = targetRotation;
    }
}