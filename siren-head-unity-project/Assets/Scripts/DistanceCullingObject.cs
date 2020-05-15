using System;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class DistanceCullingObject : MonoBehaviour
{
    public bool enableInEditor = false;
    public float cullDistance = 100;

    private MeshRenderer _meshRenderer;
    private Camera _camera;
    
    private void Start()
    {
        _camera = Camera.main;
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnValidate()
    {
        _camera = Camera.main;
        _meshRenderer = GetComponent<MeshRenderer>();
        
        #if UNITY_EDITOR
            if(enableInEditor)
                UpdateVisibility();
            else
            {
                _meshRenderer.enabled = true;
            }
        #endif
    }

    private void FixedUpdate()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        _meshRenderer.enabled = Vector3.Distance(_camera.transform.position, transform.position) < cullDistance;
    }
}