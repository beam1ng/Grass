using System.Collections.Generic;
using UnityEngine;

namespace Solutions._5_RendererFeature_IndirectInstancingPacked.Scripts
{
    [RequireComponent(typeof(RendererTransformBaker))]
    [ExecuteAlways]
    public class GrassRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material grassMaterial;
        public Material GrassMaterial => grassMaterial;
        
        private GraphicsBuffer positionBuffer;

        private RendererTransformBaker positionBaker;

        public static List<GrassRenderer> AllGrassRenderers = new();
        private static readonly int GrassBuffer = Shader.PropertyToID("_PositionBuffer");

        private void Awake()
        {
            positionBaker = GetComponent<RendererTransformBaker>();
        }

        private void OnEnable()
        {
            AllGrassRenderers.Add(this);
            InitializePositionBuffer();
            
        }

        private void OnDisable()
        {
            AllGrassRenderers.Remove(this);
            DisposePositionBuffer();
        }
        
        // Ideally I wouldn't want to call this every frame, but for some reason after editor code recompilation,
        // all material properties including buffers are pruned.
        // I tried to plug the script into UnityEditor.Compilation.CompilationPipeline.compilationFinished, but it 
        // didn't work, maybe the properties were being pruned a bit further down the pipeline.
        private void Update()
        {
            grassMaterial.SetBuffer(GrassBuffer, positionBuffer);
        }

        private void InitializePositionBuffer()
        {
            positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,positionBaker.RendererTransformPositions.Count, 3 * sizeof(float));
            OnGrassTransformsUpdate();
        }

        private void DisposePositionBuffer()
        {
            positionBuffer.Dispose();
            positionBuffer = null;
            
        }

        private void OnGrassTransformsUpdate()
        {
            positionBuffer.SetData(positionBaker.RendererTransformPositions.ToArray());
            grassMaterial.SetBuffer(GrassBuffer, positionBuffer);
        }
    }
}
