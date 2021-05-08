using System;
using System.Collections.Generic;
using System.Xml;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class Geometry
    {
        public Geometry(XmlNamespaceManager xmlns, XmlElement geometryElement)
        {
            MeshId = IntPtr.Zero;
            GeometryName = (geometryElement.HasAttribute("id")
                ? geometryElement.Attributes["id"].Value
                : string.Empty).Replace("#", string.Empty);
            var vertices = new List<Vertex>();
            var posFloats =
                ArrayHelper.ConvertToFloatArray(geometryElement.SelectSingleNode(
                        "descendant::C:mesh/C:source[contains(@id, '-pos')]/C:float_array", xmlns)
                    ?.InnerText ?? string.Empty);
            var normFloats = ArrayHelper.ConvertToFloatArray(geometryElement.SelectSingleNode(
                    "descendant::C:mesh/C:source[contains(@id, '-norm')]/C:float_array", xmlns)
                ?.InnerText ?? string.Empty);
            if (posFloats.Length > 2 && posFloats.Length == normFloats.Length)
            {
                for (var i = 0; i + 2 < posFloats.Length; i += 3)
                {
                    vertices.Add(new Vertex(posFloats[i], posFloats[i + 1], posFloats[i + 2], normFloats[i],
                        normFloats[i + 1], normFloats[i + 2], 0f, 0f));
                }
            }
            Vertices = vertices.ToArray();
            Indices = ArrayHelper.ConvertToIntArray(geometryElement.SelectSingleNode(
                "descendant::C:mesh/C:triangles/C:p", xmlns)?.InnerText ?? string.Empty);
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
        public Vertex[] Vertices { get; }
        public int[] Indices { get; }
        public int[] Faces { get; }
        public string GeometryName { get; }
        public IntPtr MeshId { get; set; }
    }
}