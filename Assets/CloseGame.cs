using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseGame : MonoBehaviour
{
    [Tooltip("Time in seconds to wait before closing the application.")]
    public float timeUntilQuit = 5.0f;

    [Tooltip("If true, the timer starts automatically when the scene loads.")]
    public bool startOnAwake = true;

    private void Start()
    {
        if (startOnAwake)
        {
            StartTimer();
        }
    }

    /// <summary>
    /// Call this to begin the countdown manually.
    /// </summary>
    public void StartTimer()
    {
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        Debug.Log($"[QuitAfterTime] Application will close in {timeUntilQuit} seconds...");
        
        yield return new WaitForSeconds(timeUntilQuit);

        QuitApplication();
    }

    /// <summary>
    /// Immediately closes the app.
    /// </summary>
    public void QuitApplication()
    {
        Debug.Log("[QuitAfterTime] Quitting now.");

        #if UNITY_EDITOR
            // stop playing the scene in the editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // close the built application
            Application.Quit();
        #endif
    }
}
