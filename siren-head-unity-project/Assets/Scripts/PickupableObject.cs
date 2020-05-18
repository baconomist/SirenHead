using System;
using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    public GameObject infoTextPrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            Instantiate(infoTextPrefab);
        }
    }
}