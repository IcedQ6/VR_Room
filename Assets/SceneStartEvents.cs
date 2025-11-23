using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneStartEvents : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How long to wait after the scene starts before firing events. 0 = Instant.")]
    public float startDelay = 0.0f;

    [Header("Events")]
    public UnityEvent onSceneLoaded;

    private void Start()
    {
        if (startDelay > 0)
        {
            StartCoroutine(WaitAndFire());
        }
        else
        {
            FireEvents();
        }
    }

    private IEnumerator WaitAndFire()
    {
        yield return new WaitForSeconds(startDelay);
        FireEvents();
    }

    private void FireEvents()
    {
        Debug.Log($"[SceneStart] Firing events on {name}");
        onSceneLoaded.Invoke();
    }
}