using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackProgress : MonoBehaviour
{
    [SerializeField] private Task[] tasks;

    public void PerformTaskByIndex(int index)
    {
        if (index < 0 || index >= tasks.Length)
        {
            Debug.LogError("index out of range");
            return;
        }
        
        tasks[index].PerformTask();
    }

}
