using System;
using System.Runtime.InteropServices;
namespace EyesightReplacement.Services
{
    public static class RPR
    {
        private const string dllName = "RadeonProRenderWrapper";
        [DllImport(dllName)]
        public static extern int CreateContext(int width, int height);
        [DllImport(dllName)]
        public static extern IntPtr CreateImage(string fileName);
        [DllImport(dllName)]
        public static extern IntPtr CreateLightFromImage(IntPtr img);
        [DllImport(dllName)]
        public static extern IntPtr CreateMaterial(int materialNodeType);
        [DllImport(dllName)]
        public static extern IntPtr CreateMesh(float[] vertices, long num_vertices, int vertex_stride,
            float[] normals, long num_normals, int normal_stride, float[] texcoords, long num_texcoords,
            int texcoord_stride, int[] vertex_indices, int vidx_stride, int[] normal_indices, int nidx_stride,
            int[] texcoord_indices, int tidx_stride, int[] num_face_vertices, long num_faces);
        [DllImport(dllName)]
        public static extern IntPtr CreateMeshInstance(IntPtr mesh);
        [DllImport(dllName)]
        public static extern void DeleteObject(IntPtr obj);
        [DllImport(dllName)]
        public static extern void Dispose();
        [DllImport(dllName)]
        public static extern void MaterialNodeSetInputFByKey(IntPtr materialNode, int materialNodeInput, float x,
            float y, float z, float w);
        [DllImport(dllName)]
        public static extern void MaterialNodeSetInputNByKey(IntPtr materialNode, int materialNodeInput,
            IntPtr materialToApplyNode);
        [DllImport(dllName)]
        public static extern void MaterialNodeSetInputUByKey(IntPtr materialNode, int materialNodeInput,
            uint materialUber);
        [DllImport(dllName)]
        public static extern void PositionCamera(float posx, float posy, float posz, float atx, float aty, float atz,
            float upx, float upy, float upz, float flength);
        [DllImport(dllName)]
        public static extern int RenderToImage(string fileName, int iterations);
        [DllImport(dllName)]
        public static extern void SetCameraTransform(float[] transform, float flength);
        [DllImport(dllName)]
        public static extern void SetShapeMaterial(IntPtr shape, IntPtr materialNode);
        [DllImport(dllName)]
        public static extern void SetShapeTransform(IntPtr shape, float[] worldTransform, float[] localTransform);
        [DllImport(dllName)]
        public static extern void TranslateShape(IntPtr shape, float x, float y, float z);
    }
}