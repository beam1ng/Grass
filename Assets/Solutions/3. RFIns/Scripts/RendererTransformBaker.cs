using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public class RendererTransformBaker : MonoBehaviour
    {
        [SerializeField]
        private List<Vector3> rendererTransformPositions;

        public List<Vector3> RendererTransformPositions => rendererTransformPositions;

        public void BakeTransformPositions()
        {
            rendererTransformPositions = GetComponentsInChildren<Renderer>()
                .Select(r => r.transform.position)
                .ToList();
        }
    }

    [CustomEditor(typeof(RendererTransformBaker))]
    public class RendererTransformBakerCustomEditor : Editor
    {
        private RendererTransformBaker rtb;

        private void OnEnable()
        {
            rtb = (RendererTransformBaker)target;
        }
    
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("bake child renderers positions"))
            {
                rtb.BakeTransformPositions();
            }
        }
    }
}