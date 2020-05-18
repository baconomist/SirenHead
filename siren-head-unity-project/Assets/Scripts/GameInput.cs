using UnityEngine;

public static class GameInput
{
    public static float GetCameraRotationX()
    {
        return Input.GetAxis("Mouse X");
    }
    
    public static float GetCameraRotationY()
    {
        return -Input.GetAxis("Mouse Y");
    }

    public static float GetForward()
    {
        return Input.GetAxis("Vertical");
    }

    public static bool IsSprinting()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }
}