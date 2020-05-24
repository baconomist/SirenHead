using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animation _animation;

    private void Start()
    {
        _animation = GetComponentInChildren<Animation>();
    }

    public void StartShake()
    {
        if(!_animation.isPlaying)
            _animation.Play();
    }

    public void StopShake()
    {
        _animation.Stop();
    }
}