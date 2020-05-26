using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogFollower : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    private Player _player;
    
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _player = FindObjectOfType<Player>();
    }
    
    void Update()
    {
        if (_player != null)
        {
            ParticleSystem.ShapeModule shape = _particleSystem.shape;
            shape.position = _player.transform.position;
        }
    }
}
