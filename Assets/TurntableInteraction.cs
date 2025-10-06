using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurntableInteraction : MonoBehaviour
{
    public AudioSource a;
    public GameObject record;
    public GameObject handle;
    public GameObject handleOrigin;

    public bool PlayOnStartup = false;
    public float animationDelay = 1.0f;

    bool isPlaying = false;
    
    float timeSinceLastInteraction = 0.0f;
    float timer;

    private void Start()
    {
        if (PlayOnStartup) ChangeTurntable();
    }

    private IEnumerator MoveHandle(GameObject handle, GameObject handleOrigin, float duration, Quaternion targetRot)
    {
        Quaternion startRot = handle.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            handle.transform.rotation = Quaternion.Lerp(startRot, targetRot, elapsedTime / duration);
            yield return null;
        }

        handle.transform.rotation = targetRot; // Ensure final rotation is exact

       
    }

    public void ChangeTurntable()
    {
        if (timeSinceLastInteraction + animationDelay < Time.time)
        {
            isPlaying = !isPlaying;

            if (isPlaying)
            {
                a.Play();
                MoveHandle(handle, handleOrigin, 1.0f, new Quaternion(0f, 35f, 0f, 0f));
            }
            else
            {
                a.Pause();
            }

            timeSinceLastInteraction = Time.time;
        }
    }

    
}
