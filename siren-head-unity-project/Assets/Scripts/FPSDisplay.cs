using UnityEngine;
using System.Collections;
 
public class FPSDisplay : MonoBehaviour
{
    private float _msec = 0.0f;
    private float _fps = 0.0f;
 
    void Update()
    {
        _msec = Time.unscaledDeltaTime * 1000f;
        _fps = 1f / Time.unscaledDeltaTime;
    }
 
    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;
 
        GUIStyle style = new GUIStyle();
 
        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 10 / 100;
        style.normal.textColor = Color.green;
        
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", _msec, _fps);
        GUI.Label(rect, text, style);
    }
}