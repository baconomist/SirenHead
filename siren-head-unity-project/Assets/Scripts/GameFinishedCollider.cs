using System;
using UnityEngine;

public class GameFinishedCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null && other.GetComponent<Player>().wheelsFound == 4)
        {
            GameManager.OnGameFinished();

            // Destroy self to prevent multiple events from propagating
            Destroy(gameObject);
        }
    }
}