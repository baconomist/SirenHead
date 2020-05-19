using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    public PickupableObject pickupableObjectPrefab;
    public GameObject pickupSpawns;
    
    private void Start()
    {
        int[] spawnPositions = new int[4];
        List<int> possibleSpawnIndicies = new List<int>();
        
        for (int i = 0; i < pickupSpawns.transform.childCount; i++)
            possibleSpawnIndicies.Add(i);
        
        for (int i = 0; i < 4; i++)
        {
            int spawnIndex = Random.Range(0, possibleSpawnIndicies.Count);
            spawnPositions[i] = possibleSpawnIndicies[spawnIndex];
            possibleSpawnIndicies.RemoveAt(spawnIndex);
        }

        for (int i = 0; i < 4; i++)
        {
            PickupableObject p = Instantiate(pickupableObjectPrefab);
            p.transform.position = pickupSpawns.transform.GetChild(spawnPositions[i]).transform.position;
        }
    }
}