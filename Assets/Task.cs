using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Task : MonoBehaviour
{
    public string taskName;
    
    // Method that completes the task
    public UnityEvent onPerformTask;
    
    // What happens because task is completed
    public UnityEvent onFinishTask;
    
    [HideInInspector]
    public bool finished = false;

    public void PerformTask()
    {
        if (finished) return;
        
        onPerformTask.Invoke();
        
        finished = true;
        
        onFinishTask.Invoke();
    }
}
