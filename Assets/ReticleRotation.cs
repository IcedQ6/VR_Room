using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReticleRotation : MonoBehaviour
{
    public Transform tr;
    // Update is called once per frame
    void Update()
    {
        tr.Rotate( new Vector3(0, 45, 0) * Time.deltaTime);
    }
}
