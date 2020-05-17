
using System;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<SirenHead>() != null)
            GetComponent<Animation>().Play();
    }
}
