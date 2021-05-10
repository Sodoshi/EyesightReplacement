using System.Xml;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class Light
    {
        public Light(XmlNamespaceManager xmlns, XmlElement lightSceneElement, XmlElement lightLibraryElement)
        {
            Transform =
                ArrayHelper.ConvertToFloatArray(lightSceneElement.SelectSingleNode(
                        "descendant::C:matrix", xmlns)
                    ?.InnerText ?? string.Empty);
            Intensity = float.Parse(lightLibraryElement.SelectSingleNode(
                    "descendant::C:extra/C:technique/C:intensity", xmlns)
                ?.InnerText.Trim() ?? "1.0f");
            Size = float.Parse(lightLibraryElement.SelectSingleNode(
                    "descendant::C:extra/C:technique/C:size", xmlns)
                ?.InnerText.Trim() ?? "1.0f");
            var techniqueNode = lightLibraryElement.SelectSingleNode(
                "descendant::C:technique_common", xmlns)?.ChildNodes[0];
            Technique = techniqueNode?.LocalName ?? string.Empty;
            if (techniqueNode != null)
            {
                Color =
                    ArrayHelper.ConvertToFloatArray(techniqueNode.SelectSingleNode(
                            "descendant::C:color", xmlns)
                        ?.InnerText ?? string.Empty);
            }
        }
        public float[] Color { get; }
        public float[] Transform { get; }
        public string Technique { get; }
        public float Size { get; }
        public float Intensity { get; }
    }
}