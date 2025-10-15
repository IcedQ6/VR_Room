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

    private IEnumerator MoveHandle(GameObject handle, GameObject handleOrigin, float duration, float targetRot)
    {
        Quaternion startRot = handle.transform.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            handle.transform.RotateAround(handleOrigin.transform.position, Vector3.up, targetRot * Time.deltaTime);
            yield return null;
        }

       
    }

    public void ChangeTurntable()
    {
        if (timeSinceLastInteraction + animationDelay < Time.time)
        {
            

            if (!isPlaying)
            {
                a.Play();
                StartCoroutine(MoveHandle(handle, handleOrigin, 1.0f, 50f));
            }
            else
            {
                a.Pause();
                StartCoroutine(MoveHandle(handle, handleOrigin, 1.0f, -50f));
            }

            timeSinceLastInteraction = Time.time;

            isPlaying = !isPlaying;
        }
    }

    private void Update()
    {
        if (isPlaying)
        {
            record.transform.Rotate(new Vector3(0f, (35f * Time.deltaTime), 0f));
        }
    }
}
