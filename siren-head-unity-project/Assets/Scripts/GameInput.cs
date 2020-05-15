using UnityEngine;

public static class GameInput
{
    public static float GetCameraRotationX()
    {
        return Input.GetAxis("Horizontal");
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