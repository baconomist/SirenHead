using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        NoiseGenerator noiseGen = target as NoiseGenerator;

        GUILayout.Label("Preview:");
        GUILayout.Label(noiseGen.GetTexture());
    }
}