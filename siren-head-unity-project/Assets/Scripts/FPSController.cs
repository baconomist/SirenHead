using UnityEngine;

public class FPSController : MonoBehaviour
{
    public float xRotSensitivity = 1f;
    
    private Camera _camera;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        _camera.transform.Rotate(Vector3.up, GameInput.GetCameraRotationX() * xRotSensitivity);
    }
}