using System.Collections.Generic;
using UnityEngine;

namespace Pretty_solutions.PrettyGrass.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class PrettyGrass : MonoBehaviour
    {
        public static readonly Dictionary<PrettyGrass, Renderer> PrettyGrassToRenderers = new();

        private void OnEnable()
        {
            PrettyGrassToRenderers.Add(this, GetComponent<Renderer>());
        }

        private void OnDisable()
        {
            PrettyGrassToRenderers.Remove(this);
        }
    }
}
