using UnityEditor;
using UnityEngine;

namespace Pretty_solutions.PrettyGrass.Scripts.Editor
{
    public class VegetationPainter : EditorWindow
    {
        private static VegetationPainter _window;

        private VegetationPainterData data;

        [MenuItem("Custom/VegetationPainter")]
        private static void CreateMenu()
        {
            _window = GetWindow<VegetationPainter>();
            _window.titleContent = new GUIContent("Vegetation Painter");
        }

        private void OnEnable()
        {
            LoadData();
        }

        private void OnGUI()
        {
            if (data == null)
            {
                EditorGUILayout.HelpBox("Please assign a VegetationPainterData asset.", MessageType.Warning);
                if (GUILayout.Button("Create New Data Asset"))
                {
                    CreateNewDataAsset();
                }
                return;
            }

            if (GUILayout.Button("S4Y S0M3TH1NG"))
            {
                Debug.Log("HEHROW!");
            }

            EditorGUI.BeginChangeCheck();

            // Display LayerMask field and update the data asset if changed
            data.brushLayerMask = EditorGUILayout.MaskField("Brush layer mask", data.brushLayerMask, UnityEditorInternal.InternalEditorUtility.layers);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(data); // Mark the asset as dirty to save changes
            }
        }

        private void LoadData()
        {
            // Load the data asset, or prompt to create one if not found
            string[] guids = AssetDatabase.FindAssets("t:VegetationPainterData");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                data = AssetDatabase.LoadAssetAtPath<VegetationPainterData>(path);
            }
        }

        private void CreateNewDataAsset()
        {
            // Create a new VegetationPainterData asset if none exists
            data = ScriptableObject.CreateInstance<VegetationPainterData>();
            AssetDatabase.CreateAsset(data, "Assets/VegetationPainterData.asset");
            AssetDatabase.SaveAssets();
        }
    }
}
