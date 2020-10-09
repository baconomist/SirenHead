using System;
using UnityEngine;

[ExecuteInEditMode]
public class DistanceCullingObject : MonoBehaviour
{
    public bool enableInEditor = false;
    public float cullDistance = 100;

    private MeshRenderer _meshRenderer;
    private bool _showing = false;

    private void Start()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _meshRenderer.enabled = _showing;
    }

    private void OnValidate()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();

#if UNITY_EDITOR
        if (enableInEditor)
            UpdateVisibility();
        else
        {
            _meshRenderer.enabled = true;
        }
#endif

        _meshRenderer.enabled = _showing;
    }

    private void Update()
    {
        UpdateVisibility();
    }

    void UpdateVisibility()
    {
        if (!Application.isPlaying && !enableInEditor)
        {
            _meshRenderer.enabled = true;
            return;
        }
        
        try
        {
            bool shouldShow = (Camera.main.transform.position - transform.position).sqrMagnitude < cullDistance*cullDistance;
            if (_showing != shouldShow)
            {
                _showing = shouldShow;
                _meshRenderer.enabled = _showing;
            }
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