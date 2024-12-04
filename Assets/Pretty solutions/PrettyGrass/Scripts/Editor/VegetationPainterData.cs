using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pretty_solutions.PrettyGrass.Scripts.Editor
{
    [CreateAssetMenu(fileName = "VegetationPainterData", menuName = "PrettySolutions/VegetationPainterData")]
    public class VegetationPainterData : ScriptableObject
    {
        public LayerMask brushLayerMask;
        public PaintableGridTree PaintableGridTree = new PaintableGridTree();
        public static bool ShouldRenderGrid = true;
    }

    [Serializable]
    public class PaintableGridTree
    {
        private const float TileLength = 0.5f;
        
        [SerializeReference]
        private static readonly HashSet<Vector2Int> PaintedTiles = new();

        public void TryPaint(Vector2 xy)
        {
            Vector2Int ids = GridPositionToTileId(xy);
            PaintTile(ids.x,ids.y);
        }

        public bool IsTilePainted(int x, int y)
        {
            return PaintedTiles.Contains(new Vector2Int(x, y));
        }

        private void PaintTile(int x, int y)
        {
            PaintedTiles.Add(new Vector2Int(x, y));
        }

        // 0  1  2  3
        // [x][ ][ ][ ]
        private static Vector2Int GridPositionToTileId(Vector2 gridPosition)
        {
            Vector2Int ids = new Vector2Int((int)(gridPosition.x % TileLength), (int)(gridPosition.y % TileLength));
            return ids;
        }
    }
}