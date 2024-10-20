using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Optimization_solutions._3_RendererFeatureSideInstancing.Scripts
{
    [RequireComponent(typeof(RendererTransformBaker))]
    [ExecuteAlways]
    public class GrassRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material grassMaterial;

        private RendererTransformBaker positionBaker;

        public static List<GrassRenderer> AllGrassRenderers = new ();

        public Material GrassMaterial => grassMaterial;

        public List<Vector3> GetGrassPositions()
        {
            return positionBaker.RendererTransformPositions;
        }

        private void Awake()
        {
            positionBaker = GetComponent<RendererTransformBaker>();
        }

        private void OnEnable()
        {
            AllGrassRenderers.Add(this);
        }

        private void OnDisable()
        {
            AllGrassRenderers.Remove(this);
        }
    }
}
