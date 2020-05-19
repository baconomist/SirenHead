using System;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class DistanceCullingObject : MonoBehaviour
{
    public bool enableInEditor = false;
    public float cullDistance = 100;

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnValidate()
    {
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
        try
        {
            _meshRenderer.enabled = Vector3.Distance(Camera.main.transform.position, transform.position) < cullDistance;
        }
        catch
        {
            // Prevent lag in case camera is disabled
            _meshRenderer.enabled = false;
            // Throw an error instead
            throw new Exception("DistanceCulling error. Could the camera be null?");
        }
    }
}