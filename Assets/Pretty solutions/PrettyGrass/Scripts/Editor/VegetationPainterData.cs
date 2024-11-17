using UnityEngine;

namespace Pretty_solutions.PrettyGrass.Scripts.Editor
{
    [CreateAssetMenu(fileName = "VegetationPainterData", menuName = "PrettySolutions/VegetationPainterData")]
    public class VegetationPainterData : ScriptableObject
    {
        public LayerMask brushLayerMask;
    }
}