using UnityEngine;

public static class GameInput
{
    public static float GetCameraRotationX()
    {
        return Input.GetAxis("Horizontal");
    }

    public static bool GetLeft()
    {
        return Input.GetKey("A");
    }

    public static bool GetRight()
    {
        return Input.GetKey("D");
    }
}