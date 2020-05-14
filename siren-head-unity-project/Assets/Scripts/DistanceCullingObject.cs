using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class DistanceCullingObject : MonoBehaviour
{
    public float cullDistance = 100;

    private MeshRenderer _meshRenderer;
    private Camera _camera;
    
    private void Start()
    {
        _camera = Camera.main;
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        _meshRenderer.enabled = Vector3.Distance(_camera.transform.position, transform.position) < cullDistance;
    }
}