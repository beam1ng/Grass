using System;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/RFResources")]
public class RfResources : ScriptableObject
{
    [SerializeField]
    private Mesh elevatedQuad;
    public Mesh ElevatedQuad => elevatedQuad;

    private void OnEnable()
    {
        if (elevatedQuad == null)
        {
            CreateElevatedQuad();
        }
    }

    private void OnDisable()
    {
        if (elevatedQuad != null)
        {
            elevatedQuad.Clear();
        }
    }

    private void CreateElevatedQuad()
    {
        // Create a new mesh if it doesn't exist
        if (elevatedQuad == null)
        {
            elevatedQuad = new Mesh();
            elevatedQuad.name = "ElevatedQuad";
#if UNITY_EDITOR
            string assetPath = "Assets/ElevatedQuad.asset";
            AssetDatabase.CreateAsset(elevatedQuad, assetPath);
            elevatedQuad = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
#endif
        }

        elevatedQuad.Clear();

        elevatedQuad.vertices = new[]
        {
            new Vector3(-0.5f, 0.0f, 0), // Bottom-left
            new Vector3(0.5f, 0.0f, 0),  // Bottom-right
            new Vector3(-0.5f, 1.0f, 0), // Top-left
            new Vector3(0.5f, 1.0f, 0),  // Top-right
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

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(elevatedQuad);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(RfResources))]
    public class RfResourcesEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            RfResources rfResources = (RfResources)target;

            if (GUILayout.Button("Recreate Elevated Quad"))
            {
                rfResources.CreateElevatedQuad();
            }
        }
    }
#endif
}
