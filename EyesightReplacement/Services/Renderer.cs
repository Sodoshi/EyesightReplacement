using System;
using System.Collections.Generic;
using System.Linq;
using EyesightReplacement.Enums;
using EyesightReplacement.Processor;
namespace EyesightReplacement.Services
{
    public sealed class Renderer : IDisposable
    {
        private readonly List<IntPtr> _images = new List<IntPtr>();
        private readonly List<IntPtr> _instances = new List<IntPtr>();
        private readonly List<IntPtr> _lights = new List<IntPtr>();
        private readonly List<IntPtr> _materials = new List<IntPtr>();
        private readonly List<IntPtr> _meshes = new List<IntPtr>();
        public Renderer(int width, int height)
        {
            try
            {
                InitialisedSuccessfully = RPR.CreateContext(width, height) == 0;
                if (!InitialisedSuccessfully)
                {
                    return;
                }
                DefaultMaterialId = CreateMaterial(MaterialNodeType.UBERV2);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_DIFFUSE_COLOR, 0.9f, 0.0f, 0.0f, 1f);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_DIFFUSE_WEIGHT, 1f, 1f, 1f, 1f);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_REFLECTION_COLOR, 0.5f, 0.5f, 0.5f, 1f);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_REFLECTION_WEIGHT, 0.5f, 0.5f, 0.5f, 1f);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_REFLECTION_ROUGHNESS, 0.1f, 0f, 0f, 0f);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_REFLECTION_MODE, MaterialModeUber.METALNESS);
                SetMaterialNode(DefaultMaterialId, MaterialNodeInput.UBER_REFLECTION_METALNESS, 0f, 0f, 0f, 1f);
            }
            catch
            {
                InitialisedSuccessfully = false;
            }
        }
        public void Dispose()
        {
            DeleteObjectsInList(_materials);
            DeleteObjectsInList(_lights);
            DeleteObjectsInList(_images);
            DeleteObjectsInList(_instances);
            DeleteObjectsInList(_meshes);
            RPR.Dispose();
        }
        public IntPtr CreateAmbientLight(float[] transform, float size, float intensity, float[] color)
        {
            //var lightId = RPR.CreateAmbientLight(transform, size, intensity, color);
            //_lights.Add(lightId);
            //return lightId;
            return IntPtr.Zero;
        }
        public IntPtr CreateDirectionalLight(float[] transform, float size, float intensity, float[] color)
        {
            //var lightId = RPR.CreateDirectionalLight(transform, size, intensity, color);
            //_lights.Add(lightId);
            //return lightId;
            return IntPtr.Zero;
        }
        public IntPtr CreateImage(string fileName)
        {
            var imageId = RPR.CreateImage(fileName);
            if (imageId != IntPtr.Zero)
            {
                _images.Add(imageId);
            }
            return imageId;
        }
        public IntPtr CreateLightFromImage(IntPtr image)
        {
            var lightId = RPR.CreateLightFromImage(image, 2.5f);
            _lights.Add(lightId);
            return lightId;
        }
        public IntPtr CreateMaterial(MaterialNodeType materialNodeType)
        {
            var materialId = RPR.CreateMaterial((int) materialNodeType);
            if (materialId != IntPtr.Zero)
            {
                _materials.Add(materialId);
            }
            return materialId;
        }
        public IntPtr CreateMesh(Vertex[] meshData, int[] indices, int[] faceVertices, int numberOfFaces)
        {
            var positionData = meshData.SelectMany(item => item.PositionData()).ToArray();
            var normalData = meshData.SelectMany(item => item.NormalData()).ToArray();
            var textureData = meshData.SelectMany(item => item.TextureData()).ToArray();

            //var newArray = new float[normalData.Length + normalData.Length];
            //Array.Copy(normalData, newArray, normalData.Length);
            //Array.Copy(normalData, 0, newArray, normalData.Length, normalData.Length);
            //for (var i=0;i< normalData.Length;i++)
            //{
            //    newArray[i] = 0 - newArray[i];
            //}


            var meshId = RPR.CreateMesh(
                positionData, positionData.LongLength / 3, sizeof(float) * 3,
                normalData, normalData.LongLength / 3, sizeof(float) * 3,
                textureData, textureData.LongLength / 2, sizeof(float) * 2,
                indices, sizeof(int),
                indices, sizeof(int),
                indices, sizeof(int),
                faceVertices, numberOfFaces);
            if (meshId != IntPtr.Zero)
            {
                _meshes.Add(meshId);
            }
            return meshId;
        }
        public IntPtr CreateMeshInstance(IntPtr meshId)
        {
            var instanceId = RPR.CreateMeshInstance(meshId);
            if (instanceId != IntPtr.Zero)
            {
                _instances.Add(instanceId);
            }
            return instanceId;
        }
        private void DeleteObjectsInList(List<IntPtr> list)
        {
            if (!list.Any())
            {
                return;
            }
            var listItems = list.ToArray();
            for (var x = listItems.Length; x-- > 0;)
            {
                var listItem = listItems[x];
                RPR.DeleteObject(listItem);
            }
            list.Clear();
        }
        public bool InitialisedSuccessfully { get; }
        public IntPtr DefaultMaterialId { get; }
        public void RenderToImage(string fileName, int iterations)
        {
            _ = RPR.RenderToImage(fileName, iterations);
        }
        public void SetCameraTransform(float[] transform, float focalLength)
        {
            RPR.SetCameraTransform(transform, focalLength);
        }
        public void SetCameraPosition(float posx, float posy, float posz, float atx, float aty, float atz, float upx, float upy, float upz, float focalLength)
        {
            RPR.PositionCamera(posx, posy, posz, atx, aty, atz, upx, upy, upz, focalLength);
        }
        public void SetMaterialNode(IntPtr materialId, MaterialNodeInput materialNodeInput, float x, float y, float z, float w)
        {
            RPR.MaterialNodeSetInputFByKey(materialId, (int) materialNodeInput, x, y, z, w);
        }
        public void SetMaterialNode(IntPtr materialId, MaterialNodeInput materialNodeInput, IntPtr materialToApplyId)
        {
            RPR.MaterialNodeSetInputNByKey(materialId, (int) materialNodeInput, materialToApplyId);
        }
        public void SetMaterialNode(IntPtr materialId, MaterialNodeInput materialNodeInput, int materialUber)
        {
            RPR.MaterialNodeSetInputUByKey(materialId, (int) materialNodeInput, (uint) materialUber);
        }
        public void SetMaterialNode(IntPtr materialId, MaterialNodeInput materialNodeInput, MaterialEmissionUber materialEmissionUber)
        {
            RPR.MaterialNodeSetInputUByKey(materialId, (int) materialNodeInput, (uint) materialEmissionUber);
        }
        public void SetMaterialNode(IntPtr materialId, MaterialNodeInput materialNodeInput, MaterialModeUber materialUber)
        {
            RPR.MaterialNodeSetInputUByKey(materialId, (int) materialNodeInput, (uint) materialUber);
        }
        public void SetShapeMaterial(IntPtr shapeId, IntPtr materialId)
        {
            RPR.SetShapeMaterial(shapeId, materialId);
        }
        public void SetShapeTransform(IntPtr shapeId, float[] worldTransform, float[] localTransform)
        {
            RPR.SetShapeTransform(shapeId, worldTransform, localTransform);
        }
        public void TranslateShape(IntPtr shapeId, float x, float y, float z)
        {
            RPR.TranslateShape(shapeId, x, y, z);
        }
    }
}