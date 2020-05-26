using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AIGrid))]
public class AIGridEditor : Editor
{
    // Increase this to be able to edit grid form further away
    const float MaxRayDistance = 5000;
    private List<Vector3Int> _hitPoints = new List<Vector3Int>();
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        AIGrid aiGrid = target as AIGrid;
        if (aiGrid.showGrid)
        {
            if (GUILayout.Button("Update/Draw Grid"))
            {
//                Material[] tempMaterials = aiGrid.gridPlane.GetComponent<MeshRenderer>().sharedMaterials;
//                if (tempMaterials[tempMaterials.Length - 1] != aiGrid.gridMaterial)
//                {
//                    Material[] materials = new Material[tempMaterials.Length + 1];
//                    for (int i = 0; i < tempMaterials.Length; i++)
//                        materials[i] = tempMaterials[i];
//                    materials[materials.Length - 1] = aiGrid.gridMaterial;
//                    aiGrid.gridPlane.GetComponent<MeshRenderer>().sharedMaterials = materials;
//                }

                DrawToGrid();
            }
        }
    }

    private void DrawToGrid()
    {
        AIGrid aiGrid = target as AIGrid;
        
        aiGrid.gridPlane.GetComponent<MeshRenderer>().sharedMaterial = aiGrid.gridMaterial;

        Color[] colors = new Color[aiGrid.resolution * aiGrid.resolution];
        for (int i = 0; i < _hitPoints.Count; i++)
        {
            int colorCoord = _hitPoints[i].x + _hitPoints[i].z * aiGrid.resolution - 1;

            colors[colorCoord] = Color.red;
        }

        colors[0] = Color.cyan;

        Texture2D texture = new Texture2D(aiGrid.resolution, aiGrid.resolution);
        texture.SetPixels(colors);
        texture.Apply();
        aiGrid.gridMaterial.mainTexture = texture;
    }

    private void OnSceneGUI()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePosition);
            mousePosition = ray.origin;

            if (((AIGrid) target).gridPlane.GetComponent<Collider>().Raycast(ray, out RaycastHit rayInfo, AIGridEditor.MaxRayDistance))
            {
                Vector3 hitPoint = rayInfo.point;
                Vector3Int gridPoint = WorldToGrid(hitPoint, (AIGrid) target);
                _hitPoints.Add(gridPoint);
                
                Debug.DrawLine (mousePosition, hitPoint, Color.red, 5);
                Debug.Log(hitPoint);
                Debug.Log(gridPoint);
                
                DrawToGrid();
            }
        }
    }

    private Vector3Int WorldToGrid(Vector3 worldPos, AIGrid aiGrid)
    {
        Vector3 extents = aiGrid.gridPlane.GetComponent<MeshFilter>().sharedMesh.bounds.extents * aiGrid.transform.localScale.x;
        // Calculate grid pos
        Vector3 result = (worldPos - aiGrid.gridPlane.transform.position + extents) / extents.x * aiGrid.resolution / 2f;
        // Make grid pos the same as texture pos(0,0 is top-left)
        result = new Vector3(aiGrid.resolution, 0, aiGrid.resolution) - result;
        
        // Round the final result to a grid cell
        return RoundVec(result);
    }

    private Vector3Int RoundVec(Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }
}