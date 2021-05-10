using System;
using System.Collections.Generic;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class GeometryIndices
    {
        public GeometryIndices(string triangleNorms)
        {
            Indices = ArrayHelper.ConvertToIntArray(triangleNorms);
            var faces = new List<int>();
            if (Indices.Length > 2)
            {
                for (var i = 0; i + 2 < Indices.Length; i += 3)
                {
                    faces.Add(3);
                }
            }
            Faces = faces.ToArray();
        }
        public IntPtr MeshId { get; set;  }
        public readonly int[] Indices;
        public readonly int[] Faces;
    }
}