using System.Xml;
namespace EyesightReplacement.Processor
{
    public class Material
    {
        public Material(XmlNamespaceManager xmlns, XmlElement materialElement)
        {
            MaterialName = (materialElement.HasAttribute("id")
                ? materialElement.Attributes["id"].Value
                : string.Empty).Replace("#", string.Empty);
            var effectElement =
                (XmlElement) materialElement.SelectSingleNode("descendant::C:instance_effect", xmlns);
            if (effectElement == null)
            {
                return;
            }
            EffectName = (effectElement.HasAttribute("url")
                ? effectElement.Attributes["url"].Value
                : string.Empty).Replace("#", string.Empty);
        }
        public string MaterialName { get; }
        public string EffectName { get; }
    }
}