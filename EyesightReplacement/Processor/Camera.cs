using System.Xml;
using EyesightReplacement.Helpers;
namespace EyesightReplacement.Processor
{
    public class Camera
    {
        public Camera(XmlNamespaceManager xmlns, XmlElement cameraSceneElement, XmlElement cameraLibraryElement)
        {
            Transform =
                ArrayHelper.ConvertToFloatArray(cameraSceneElement.SelectSingleNode(
                        "descendant::C:matrix", xmlns)
                    ?.InnerText ?? string.Empty);
            Width = 1920;
            Height = 1080;
            var resX = cameraLibraryElement.SelectSingleNode("descendant::C:resolution_x", xmlns);
            if (resX != null)
            {
                if (int.TryParse(resX.InnerText, out var width))
                {
                    Width = width;
                }
            }
            var resY = cameraLibraryElement.SelectSingleNode("descendant::C:resolution_y", xmlns);
            if (resY != null)
            {
                if (int.TryParse(resY.InnerText, out var height))
                {
                    Height = height;
                }
            }
        }
        public int Width { get; }
        public int Height { get; }
        public float[] Transform { get; }
    }
}