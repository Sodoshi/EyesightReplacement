using System;
using System.Collections.Generic;
using System.Xml;
using EyesightReplacement.Enums;
using EyesightReplacement.Helpers;
using EyesightReplacement.Services;
namespace EyesightReplacement.Processor
{
    public class Collada
    {
        private readonly XmlDocument _colladaDocument;
        private readonly Dictionary<string, MaterialEffect> _effects = new Dictionary<string, MaterialEffect>();
        private readonly Dictionary<string, Geometry> _geometries = new Dictionary<string, Geometry>();
        private readonly Dictionary<string, Light> _lights = new Dictionary<string, Light>();
        private readonly Dictionary<string, Material> _materials = new Dictionary<string, Material>();
        private readonly XmlNamespaceManager _xmlns;
        private Camera _camera;
        private string _rootNodeName;
        private float[] _worldMatrix;
        public Collada(string fileName)
        {
            _colladaDocument = new XmlDocument();
            var settings = new XmlReaderSettings {NameTable = new NameTable()};
            _xmlns = new XmlNamespaceManager(settings.NameTable);
            _xmlns.AddNamespace("RoundingEdgeNormal", "http://www.w3.org/2001/XMLSchema-instance");
            _xmlns.AddNamespace("RoughSurface", "http://www.w3.org/2001/XMLSchema-instance");
            _xmlns.AddNamespace("PearlGroupPrincipledBSDF", "http://www.w3.org/2001/XMLSchema-instance");

            var context = new XmlParserContext(null, _xmlns, "", XmlSpace.Default);
            using (var reader = XmlReader.Create(fileName, settings, context))
            {
                _colladaDocument.Load(reader);
            }
            _xmlns.AddNamespace("C", _colladaDocument.DocumentElement?.NamespaceURI ?? string.Empty);

            var scene = (XmlElement) _colladaDocument.SelectSingleNode("//C:scene/C:instance_visual_scene", _xmlns);
            if (scene == null)
            {
                return;
            }
            ProcessScene(scene);
        }
        public int CameraWidth => _camera?.Width ?? 0;
        public int CameraHeight => _camera?.Height ?? 0;
        private void AddGeometryInstance(Renderer renderer, GeometryInstance geometryInstance)
        {
            var geometry = _geometries[geometryInstance.GeometryName];

            var effect = renderer.DefaultMaterialId;
            var materialName = "defaultMaterial";
            if (_materials.ContainsKey(geometryInstance.MaterialName))
            {
                var material = _materials[geometryInstance.MaterialName];
                if (_effects.ContainsKey(material.EffectName))
                {
                    var possibleEffect = _effects[material.EffectName];
                    if (possibleEffect.EffectId == IntPtr.Zero)
                    {
                        Console.WriteLine($"Generating effect: {possibleEffect.EffectName}");
                        possibleEffect.EffectId = renderer.CreateMaterial(MaterialNodeType.UBERV2);
                        renderer.SetMaterialNode(possibleEffect.EffectId,
                            MaterialNodeInput.UBER_DIFFUSE_COLOR, possibleEffect.DiffuseColor[0],
                            possibleEffect.DiffuseColor[1], possibleEffect.DiffuseColor[2],
                            possibleEffect.DiffuseColor[3]);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_DIFFUSE_WEIGHT, 0.5f, 0.5f, 0.5f, 0.5f);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_DIFFUSE_ROUGHNESS, 0.5f, 0.5f, 0.5f, 0.5f);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_BACKSCATTER_COLOR, possibleEffect.DiffuseColor[0],
                        //    possibleEffect.DiffuseColor[1], possibleEffect.DiffuseColor[2],
                        //    possibleEffect.DiffuseColor[3]);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_BACKSCATTER_WEIGHT, 0.5f, 0.5f, 0.5f, 0.5f);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_COLOR    ,1, 1, 1, 1);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_WEIGHT    ,0, 0, 0, 0);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_ROUGHNESS   ,0, 0, 0, 0);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_MODE     ,MaterialModeUber.PBR);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_IOR     ,3, 3, 3, 3);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_METALNESS     ,0, 0, 0, 0);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_TRANSMISSION_COLOR  ,1, 1, 1, 1);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_COATING_THICKNESS     ,2, 2, 2, 2);


                        renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_EMISSION_COLOR     ,1, 1, 1, 1);
                        renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_EMISSION_WEIGHT    ,0, 0, 0, 0);
                        renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_EMISSION_MODE     ,MaterialEmissionUber.DOUBLESIDED);

                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_REFLECTION_COLOR, possibleEffect.DiffuseColor[0],
                        //    possibleEffect.DiffuseColor[1], possibleEffect.DiffuseColor[2],
                        //    possibleEffect.DiffuseColor[3]);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_REFLECTION_WEIGHT, 0f, 0f, 0f, 1f);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_REFLECTION_ROUGHNESS, 0f, 0f, 0f, 0f);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_REFLECTION_MODE, MaterialModeUber.METALNESS);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_REFLECTION_METALNESS, 0f, 0f, 0f, 1f);

                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_SHEEN, 1,
                        //    1, 1, 1);
                        //renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_SHEEN_TINT,
                        //    0, 0, 0, 0);
                        //renderer.SetMaterialNode(possibleEffect.EffectId,
                        //    MaterialNodeInput.UBER_SHEEN_WEIGHT, 1, 1, 1, 1);
                        if(possibleEffect.Transparency < 1)
                        {
                            renderer.SetMaterialNode(possibleEffect.EffectId, MaterialNodeInput.UBER_TRANSPARENCY,  possibleEffect.Transparency, possibleEffect.Transparency, possibleEffect.Transparency, possibleEffect.Transparency);
                        }
                    }
                    effect = possibleEffect.EffectId;
                    materialName = possibleEffect.EffectName;
                }
            }

            foreach(var geometryIndices in geometry.GeometryIndices)
            {
                if (geometryIndices.MeshId == IntPtr.Zero)
                {
                    Console.WriteLine(
                        $"Creating mesh: {geometryInstance.GeometryName} with Effect {materialName}");

                    var newMeshId = renderer.CreateMesh(geometry.Vertices, geometryIndices.Indices, geometryIndices.Faces,
                        geometryIndices.Faces.Length);

                    if (newMeshId != IntPtr.Zero)
                    {
                        geometryIndices.MeshId=newMeshId;
                        renderer.SetShapeMaterial(newMeshId, effect);
                        renderer.SetShapeTransform(newMeshId, geometryInstance.WorldTransform, geometryInstance.LocalTransform);
                    }
                    else
                    {
                        Console.WriteLine($"Mesh Build Error: {geometryInstance.GeometryName}");
                        _geometries.Remove(geometryInstance.GeometryName);
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"Creating mesh instance: {geometryInstance.GeometryName} with Effect {materialName}");
                    var meshId= renderer.CreateMeshInstance(geometryIndices.MeshId);
                    renderer.SetShapeMaterial(meshId, effect);
                    renderer.SetShapeTransform(meshId, geometryInstance.WorldTransform, geometryInstance.LocalTransform);
                }
            }


        }
        private void ProcessInstanceCamera(XmlElement cameraSceneElement, XmlElement instanceElement)
        {
            var cameraSceneInstanceName = (instanceElement.HasAttribute("url")
                ? instanceElement.Attributes["url"].Value
                : string.Empty).Replace("#", string.Empty);

            XmlElement cameraLibraryElement;
            if (string.IsNullOrWhiteSpace(cameraSceneInstanceName))
            {
                cameraLibraryElement = (XmlElement) _colladaDocument.SelectSingleNode("//C:camera[C:optics]", _xmlns);
            }
            else
            {
                cameraLibraryElement =
                    (XmlElement) _colladaDocument.SelectSingleNode($"//C:camera[@id='{cameraSceneInstanceName}']",
                        _xmlns);
            }
            if (cameraLibraryElement == null)
            {
                return;
            }
            _camera = new Camera(_xmlns, cameraSceneElement, cameraLibraryElement);
        }
        private void ProcessInstanceLight(XmlElement lightSceneElement, XmlElement instanceElement)
        {
            var lightSceneInstanceName = (instanceElement.HasAttribute("url")
                ? instanceElement.Attributes["url"].Value
                : string.Empty).Replace("#", string.Empty);

            XmlElement lightLibraryElement = null;
            if (!string.IsNullOrWhiteSpace(lightSceneInstanceName))
            {
                lightLibraryElement =
                    (XmlElement) _colladaDocument.SelectSingleNode($"//C:light[@id='{lightSceneInstanceName}']",
                        _xmlns);
            }
            if (lightLibraryElement == null)
            {
                return;
            }
            _lights.Add(lightSceneInstanceName, new Light(_xmlns, lightSceneElement, lightLibraryElement));
        }
        private void ProcessLibraryNode(Renderer renderer, string nodeName, float[] parentTransform)
        {
            var libraryNodeElement =
                (XmlElement) _colladaDocument.SelectSingleNode($"//C:library_nodes//C:node[@id='{nodeName}']", _xmlns);
            if (libraryNodeElement == null)
            {
                return;
            }
            var localMatrix = libraryNodeElement.SelectSingleNode(
                    "C:matrix", _xmlns)
                ?.InnerText ?? string.Empty;
            var transform = string.IsNullOrWhiteSpace(localMatrix)
                ? parentTransform
                : ArrayHelper.Multiply(parentTransform,
                    ArrayHelper.ConvertToFloatArray(localMatrix));
            foreach (XmlElement childNode in libraryNodeElement.ChildNodes)
            {
                switch (childNode.LocalName)
                {
                    case "instance_node":
                        var instanceNodeName = (childNode.HasAttribute("url")
                            ? childNode.Attributes["url"].Value
                            : string.Empty).Replace("#", string.Empty);
                        if (!string.IsNullOrWhiteSpace(instanceNodeName))
                        {
                            ProcessLibraryNode(renderer, instanceNodeName, transform);
                        }
                        break;
                    case "node":
                        if (childNode.HasAttribute("id"))
                        {
                            var childNodeName = childNode.Attributes["id"].Value.Replace("#", string.Empty);
                            if (!string.IsNullOrWhiteSpace(childNodeName))
                            {
                                ProcessLibraryNode(renderer, childNodeName, transform);
                            }
                        }
                        else
                        {
                            var instance_geometry = childNode.SelectSingleNode(
                                "C:instance_geometry", _xmlns);
                            if (instance_geometry != null)
                            {
                                var geometryInstance = new GeometryInstance(_xmlns, childNode, transform);
                                if (!string.IsNullOrWhiteSpace(geometryInstance.GeometryName))
                                {
                                    AddGeometryInstance(renderer, geometryInstance);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private void ProcessLights(Renderer renderer)
        {
            foreach(var light in _lights.Values)
            {
                switch(light.Technique)
                {
                    case "ambient":
                        _ = renderer.CreateAmbientLight(light.Transform, light.Size, light.Intensity, light.Color);
                        break;
                    case "directional":
                        _ = renderer.CreateDirectionalLight(light.Transform, light.Size, light.Intensity, light.Color);
                        break;
                    default:
                        break;
                }
            }
        }
        private void ProcessScene(XmlElement scene)
        {
            var visualSceneName = (scene.HasAttribute("url")
                ? scene.Attributes["url"].Value
                : string.Empty).Replace("#", string.Empty);

            var visualScene =
                (XmlElement) _colladaDocument.SelectSingleNode($"//C:visual_scene[@id='{visualSceneName}']", _xmlns);
            var sceneNodes = visualScene?.SelectNodes("descendant::C:node[C:matrix]", _xmlns);
            if (sceneNodes == null)
            {
                return;
            }
            foreach (XmlElement sceneNode in sceneNodes)
            {
                foreach (XmlElement childNode in sceneNode.ChildNodes)
                {
                    if (!childNode.LocalName.StartsWith("instance_"))
                    {
                        continue;
                    }
                    switch (childNode.LocalName)
                    {
                        case "instance_light":
                            ProcessInstanceLight(sceneNode, childNode);
                            break;
                        case "instance_camera":
                            if (_camera == null)
                            {
                                ProcessInstanceCamera(sceneNode, childNode);
                            }
                            break;
                        case "instance_node":
                            var rootNodeName = (childNode.HasAttribute("url")
                                ? childNode.Attributes["url"].Value
                                : string.Empty).Replace("#", string.Empty);
                            var worldMatrix = sceneNode.SelectSingleNode(
                                    "C:matrix", _xmlns)
                                ?.InnerText ?? string.Empty;
                            if (string.IsNullOrWhiteSpace(rootNodeName) || string.IsNullOrWhiteSpace(worldMatrix))
                            {
                                break;
                            }
                            _rootNodeName = rootNodeName;
                            _worldMatrix = ArrayHelper.ConvertToFloatArray(worldMatrix);
                            break;
                        default:
                            break;
                    }
                }
            }
            var effectNodes = _colladaDocument.SelectNodes("//C:effect[//C:phong]", _xmlns);
            if (effectNodes != null)
            {
                foreach (XmlElement effectElement in effectNodes)
                {
                    var effect = new MaterialEffect(_xmlns, effectElement);
                    if (!string.IsNullOrWhiteSpace(effect.EffectName) && !_effects.ContainsKey(effect.EffectName))
                    {
                        _effects.Add(effect.EffectName, effect);
                    }
                }
            }

            var materialNodes = _colladaDocument.SelectNodes("//C:material[C:instance_effect]", _xmlns);
            if (materialNodes != null)
            {
                foreach (XmlElement materialElement in materialNodes)
                {
                    var material = new Material(_xmlns, materialElement);
                    if (!string.IsNullOrWhiteSpace(material.MaterialName) &&
                        !_materials.ContainsKey(material.MaterialName) && _effects.ContainsKey(material.EffectName))
                    {
                        _materials.Add(material.MaterialName, material);
                    }
                }
            }

            var geometryNodes = _colladaDocument.SelectNodes("//C:geometry", _xmlns);
            if (geometryNodes == null)
            {
                return;
            }
            foreach (XmlElement geometryElement in geometryNodes)
            {
                var geometry = new Geometry(_xmlns, geometryElement);
                if (!string.IsNullOrWhiteSpace(geometry.GeometryName) &&
                    !_geometries.ContainsKey(geometry.GeometryName))
                {
                    _geometries.Add(geometry.GeometryName, geometry);
                }
            }
        }
        public void RenderScene(Renderer renderer)
        {
            if (string.IsNullOrWhiteSpace(_rootNodeName))
            {
                return;
            }
            ProcessLights(renderer);
            renderer.SetCameraTransform(_camera.Transform, 75f);
            ProcessLibraryNode(renderer, _rootNodeName, _worldMatrix);
        }
    }
}