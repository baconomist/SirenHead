using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        NoiseGenerator noiseGen = target as NoiseGenerator;

        GUILayout.Label("Preview:");
        GUILayout.Label(noiseGen.GetTexture());

        if (GUILayout.Button("Save Texture"))
        {
            var path = EditorUtility.SaveFilePanel(
                "Save texture as PNG",
                "",
                "NoiseTexture.png",
                "png");

            if (path.Length != 0)
            {
                var pngData = noiseGen.GetTexture().EncodeToPNG();
                if (pngData != null)
                    File.WriteAllBytes(path, pngData);
            }
        }

    }
}