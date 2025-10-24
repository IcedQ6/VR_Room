using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blowup : MonoBehaviour
{
    public GameObject bu;
    public float delay;

    public void BlowUp()
    {
        StartCoroutine(doit());

    }

    IEnumerator doit()
    {
        yield return new WaitForSeconds(delay);
        Instantiate(bu, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }
}
