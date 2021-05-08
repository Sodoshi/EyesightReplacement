using System.Xml;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class GeometryInstance
    {
        public GeometryInstance(XmlNamespaceManager xmlns, XmlElement instanceElement, float[] parentTransform)
        {
            WorldTransform = parentTransform;
            var matrix = instanceElement.SelectSingleNode(
                    "C:matrix", xmlns)
                ?.InnerText ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(matrix))
            {
                LocalTransform =
                    ArrayHelper.ConvertToFloatArray(instanceElement.SelectSingleNode(
                            "C:matrix", xmlns)
                        ?.InnerText ?? string.Empty);
            }

            var instanceGeometryElement =
                (XmlElement) instanceElement.SelectSingleNode("descendant::C:instance_geometry", xmlns);
            if (instanceGeometryElement == null)
            {
                return;
            }
            GeometryName = (instanceGeometryElement.HasAttribute("url")
                ? instanceGeometryElement.Attributes["url"].Value
                : string.Empty).Replace("#", string.Empty);
            var instanceMaterialElement =
                (XmlElement) instanceGeometryElement.SelectSingleNode("descendant::C:instance_material", xmlns);
            if (instanceMaterialElement == null)
            {
                return;
            }
            MaterialName = (instanceMaterialElement.HasAttribute("target")
                ? instanceMaterialElement.Attributes["target"].Value
                : string.Empty).Replace("#", string.Empty);
        }
        public string GeometryName { get; }
        public string MaterialName { get; }
        public float[] WorldTransform { get; }
        public float[] LocalTransform { get; }
    }
}