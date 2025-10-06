using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomizebirds : MonoBehaviour
{

    public List<AudioClip> birdChirps = new List<AudioClip>();
    public float minChirp = 1.0f;
    public float maxChirp = 10.0f;
    float chirp0, chirp1, chirp2;

    private void Start()
    {
        chirp0 = Mathf.Clamp(Random.value, minChirp, maxChirp);
        chirp1 = Mathf.Clamp(Random.value, minChirp, maxChirp);
        chirp2 = Mathf.Clamp(Random.value, minChirp, maxChirp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
