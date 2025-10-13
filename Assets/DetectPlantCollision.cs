using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectPlantCollision : MonoBehaviour
{
    public ParticleSystem particle;
    
    [SerializeField] 
    private UnityEvent onParticleTriggered;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!particle)
        {
            particle = gameObject.AddComponent<ParticleSystem>();
        }
        
        
    }

    void OnParticleTrigger()
    {
        onParticleTriggered.Invoke();
    }
}
