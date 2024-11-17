using UnityEditor;
using UnityEngine;
using System.IO;

public class TerrainMeshExtractor : MonoBehaviour
{
    [SerializeField]
    private Terrain terrain;
    
    [SerializeField]
    private string saveFileName = "TerrainMesh";
    
    [SerializeField]
    private int resolution = 100; // Number of vertices per side
    
    [System.Serializable]
    public class SerializedTerrainData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public Vector3[] normals;
        public float[,] heights;
        public TerrainData originalTerrainData;
    }

    public void ExtractAndSerialize()
    {
        if (terrain == null)
        {
            Debug.LogError("No terrain assigned!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        
        // Calculate vertex spacing
        float meshWidth = terrainData.size.x;
        float meshLength = terrainData.size.z;
        float xSpacing = meshWidth / (resolution - 1);
        float zSpacing = meshLength / (resolution - 1);

        // Create arrays for mesh data
        Vector3[] vertices = new Vector3[resolution * resolution];
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // Get height data
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        
        // Generate vertices and UVs
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int index = z * resolution + x;
                
                // Calculate normalized position for sampling height
                float normalizedX = x * xSpacing / meshWidth;
                float normalizedZ = z * zSpacing / meshLength;
                
                // Sample height using bilinear interpolation
                float height = terrainData.GetInterpolatedHeight(normalizedX, normalizedZ);
                
                vertices[index] = new Vector3(x * xSpacing, height, z * zSpacing);
                uvs[index] = new Vector2(normalizedX, normalizedZ);
                
                // Calculate normal using terrain data
                normals[index] = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
            }
        }

        // Generate triangles
        int triangleIndex = 0;
        for (int z = 0; z < resolution - 1; z++)
        {
            for (int x = 0; x < resolution - 1; x++)
            {
                int vertexIndex = z * resolution + x;
                
                // First triangle
                triangles[triangleIndex] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + resolution;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                
                // Second triangle
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + resolution;
                triangles[triangleIndex + 5] = vertexIndex + resolution + 1;
                
                triangleIndex += 6;
            }
        }

        // Create serializable data container
        SerializedTerrainData serializedData = new SerializedTerrainData
        {
            vertices = vertices,
            triangles = triangles,
            uvs = uvs,
            normals = normals,
            heights = heights,
            originalTerrainData = terrainData
        };

        // Save to JSON
        string json = JsonUtility.ToJson(serializedData, true);
        string path = Path.Combine(Application.dataPath, saveFileName + ".json");
        File.WriteAllText(path, json);
        
        Debug.Log($"Terrain mesh data saved to: {path}");
        
        // Optional: Create a preview mesh
        CreatePreviewMesh(vertices, triangles, uvs, normals);

        AssetDatabase.Refresh();
    }

    private void CreatePreviewMesh(Vector3[] vertices, int[] triangles, Vector2[] uvs, Vector3[] normals)
    {
        GameObject previewObj = new GameObject("TerrainMeshPreview");
        previewObj.transform.position = terrain.transform.position;
        
        MeshFilter meshFilter = previewObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = previewObj.AddComponent<MeshRenderer>();
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        
        meshFilter.sharedMesh = mesh;
        
        // Assign a default material
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
    }
}

[CustomEditor(typeof(TerrainMeshExtractor))]
public class TerrainMeshExtractorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Extract"))
        {
            ((TerrainMeshExtractor)target).ExtractAndSerialize();
        }
    }
}