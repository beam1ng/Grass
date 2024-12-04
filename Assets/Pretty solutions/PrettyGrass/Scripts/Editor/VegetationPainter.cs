using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Pretty_solutions.PrettyGrass.Scripts.Editor
{
    public class VegetationPainter : EditorWindow
    {
        private static VegetationPainter _window;

        private VegetationPainterData data;

        private int concatenatedLayersMask;
        private bool isPainting;

        [MenuItem("Custom/VegetationPainter")]
        private static void CreateMenu()
        {
            _window = GetWindow<VegetationPainter>();
            _window.titleContent = new GUIContent("Vegetation Painter");
        }

        private void OnEnable()
        {
            LoadData();
            SceneView.duringSceneGui += SceneViewUpdate;
            HandlePaintingEvents();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewUpdate;
        }

        private void SceneViewUpdate(SceneView sceneView)
        {
            if (isPainting)
            {
                EditorInputManager.ListenInputEvents(sceneView);
            }
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

            EditorGUI.BeginChangeCheck();

            concatenatedLayersMask = EditorGUILayout.MaskField("Brush layer mask", concatenatedLayersMask, UnityEditorInternal.InternalEditorUtility.layers);
            data.brushLayerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(concatenatedLayersMask);
            bool oldPaintingState = isPainting;
            isPainting = EditorGUILayout.Toggle("Is Painting",isPainting);
            VegetationPainterData.ShouldRenderGrid =
                EditorGUILayout.Toggle("Should Render Grid", VegetationPainterData.ShouldRenderGrid);
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(data);
                HandlePaintingState(oldPaintingState);
            }
        }

        private void HandlePaintingState(bool oldPaintingState)
        {
            if (isPainting == oldPaintingState)
            {
                return;
            }
            
            HandlePaintingEvents();
        }

        private void HandlePaintingEvents()
        {
            if (isPainting)
            {
                EditorInputManager.OnMouseClick += Paint;
                Selection.activeGameObject = null;
            }
            else
            {
                EditorInputManager.OnMouseClick -= Paint;
            }
        }

        private void Paint(Vector3 mousePosition)
        {
            if (SceneView.lastActiveSceneView == null)
            {
                Debug.LogWarning("SceneView not found!");
                return;
            }
            
            Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;

            if (sceneViewCamera == null)
            {
                Debug.LogWarning("SceneView camera not found!");
                return;
            }
            
            Ray screenToSceneRay = sceneViewCamera.ScreenPointToRay(mousePosition);
           if (Physics.Raycast(screenToSceneRay, out RaycastHit hit, Mathf.Infinity ,data.brushLayerMask))
            {
                Debug.DrawRay(hit.point,hit.normal,Color.red,1.0f);
                Vector2 tileRelativePositionWS = 
                    new Vector2(hit.point.x,hit.point.z)
                    - new Vector2(hit.transform.transform.position.x,hit.transform.position.z);

                Debug.Log($"{tileRelativePositionWS.x},{tileRelativePositionWS.y}");
                data.PaintableGridTree.TryPaint(tileRelativePositionWS);
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

    public static class EditorInputManager
    {
        public static event Action<Vector3> OnMouseClick;

        public static void ListenInputEvents(SceneView sceneView)
        {
            Event currentEvent = Event.current;
            
            if (currentEvent.type == EventType.Layout || currentEvent.type == EventType.MouseDown)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            
            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                Vector2 mousePositionInvertedY = new Vector2(currentEvent.mousePosition.x,
                    sceneView.camera.pixelHeight - currentEvent.mousePosition.y);
                
                OnMouseClick?.Invoke(mousePositionInvertedY);
            }
        }
    }
}
