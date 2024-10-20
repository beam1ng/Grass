using System.Collections.Generic;
using UnityEngine;

namespace Solutions._4_RendererFeatureSideIndirectInstancing.Scripts
{
    [RequireComponent(typeof(RendererTransformBaker))]
    [ExecuteAlways]
    public class GrassRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material grassMaterial;
        public Material GrassMaterial => grassMaterial;

        public GraphicsBuffer PositionBuffer;

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
            PositionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured,positionBaker.RendererTransformPositions.Count, 3 * sizeof(float));
            
            OnGrassTransformsUpdate();
        }

        private void OnGrassTransformsUpdate()
        {
            PositionBuffer.SetData(positionBaker.RendererTransformPositions.ToArray());
            grassMaterial.SetBuffer(GrassBuffer, PositionBuffer);
        }

        private void OnDisable()
        {
            AllGrassRenderers.Remove(this);
            PositionBuffer.Dispose();
            PositionBuffer = null;
        }
    }
}