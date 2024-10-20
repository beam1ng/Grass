using System.Collections.Generic;
using UnityEngine;

namespace Common.Scripts
{
    public static class ListMatrixExtensions
    {
        public static Matrix4x4[] AsPositionMatricesArray(this List<Vector3> positions)
        {
            int count = positions.Count;
            Matrix4x4[] matrices = new Matrix4x4[count];
            for (int i = 0; i < count; i++)
            {
                matrices[i] = Matrix4x4.TRS(positions[i], Quaternion.identity, Vector3.one);
            }
            return matrices;
        }
    }
}