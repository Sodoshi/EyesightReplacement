using System;
using System.Xml;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class MaterialEffect
    {
        public IntPtr EffectId { get; set; }
        public readonly float[] DiffuseColor = {0f, 0f, 0f, 0f};
        public MaterialEffect(XmlNamespaceManager xmlns, XmlElement effectElement)
        {
            EffectId = IntPtr.Zero;
            EffectName = (effectElement.HasAttribute("id")
                ? effectElement.Attributes["id"].Value
                : string.Empty).Replace("#", string.Empty);
            var phongElement =
                (XmlElement) effectElement.SelectSingleNode("descendant::C:phong", xmlns);
            if (phongElement == null)
            {
                return;
            }
            foreach (XmlElement phongChildElement in phongElement.ChildNodes)
            {
                if (phongChildElement.ChildNodes.Count <= 0)
                {
                    continue;
                }
                var phongChildElementData = (XmlElement) phongChildElement.ChildNodes[0];
                var sid = phongChildElementData.HasAttribute("sid")
                    ? phongChildElementData.Attributes["sid"].Value
                    : string.Empty;
                if (string.IsNullOrWhiteSpace(sid))
                {
                    continue;
                }
                var data = phongChildElementData.InnerText;
                if (string.IsNullOrWhiteSpace(data))
                {
                    continue;
                }
                switch(sid)
                {
                    case "diffuse":
                        MapFloatArray(data, ref DiffuseColor);
                        break;
                    default:
                        continue;
                }
            }
        }
        public string EffectName { get; }
        private static void MapFloatArray(string inputData, ref float[] outputData)
        {
            var floatData = ArrayHelper.ConvertToFloatArray(inputData);
            if (floatData.Length != 4)
            {
                return;
            }
            outputData[0] = floatData[0];
            outputData[1] = floatData[1];
            outputData[2] = floatData[2];
            outputData[3] = floatData[3];
        }
    }
}