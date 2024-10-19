using UnityEngine;
using UnityEditor;

public class MeshGenerator : ScriptableObject
{
    private static void CreateElevatedQuad()
    {
        Mesh elevatedQuad = new()
        {
            name = "ElevatedQuad"
        };
        
        const string assetPath = "Assets/ElevatedQuad.asset";
        AssetDatabase.CreateAsset(elevatedQuad, assetPath);
        elevatedQuad = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);

        elevatedQuad.Clear();

        elevatedQuad.vertices = new[]
        {
            new Vector3(-0.5f, 0.0f, 0), // Bottom-left
            new Vector3(0.5f, 0.0f, 0), // Bottom-right
            new Vector3(-0.5f, 1.0f, 0), // Top-left
            new Vector3(0.5f, 1.0f, 0), // Top-right
        };

        elevatedQuad.triangles = new[] { 0, 1, 2, 2, 1, 3 }; // Index buffer

        elevatedQuad.uv = new[]
        {
            new Vector2(0f, 0f), // Bottom-left
            new Vector2(1f, 0f), // Bottom-right
            new Vector2(0f, 1f), // Top-left
            new Vector2(1f, 1f), // Top-right
        };

        elevatedQuad.normals = new[]
        {
            new Vector3(0, 0, 1), // Bottom-left
            new Vector3(0, 0, 1), // Bottom-right
            new Vector3(0, 0, 1), // Top-left
            new Vector3(0, 0, 1), // Top-right
        };

        elevatedQuad.RecalculateBounds();
        
        EditorUtility.SetDirty(elevatedQuad);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    [CustomEditor(typeof(MeshGenerator))]
    public class MeshGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MeshGenerator meshGenerator = (MeshGenerator)target;

            if (GUILayout.Button("Recreate Elevated Quad"))
            {
                CreateElevatedQuad();
            }
        }
    }
}