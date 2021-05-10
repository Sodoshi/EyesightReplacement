#pragma once

#ifdef RADEONPRORENDERWRAPPER_EXPORTS
#define RADEONPRORENDERWRAPPER_API __declspec(dllexport)
#else
#define RADEONPRORENDERWRAPPER_API __declspec(dllimport)
#endif

extern "C" RADEONPRORENDERWRAPPER_API rpr_light CreateAmbientLight(rpr_float const * transform);
extern "C" RADEONPRORENDERWRAPPER_API rpr_light CreateDirectionalLight(rpr_float const * transform, rpr_float size, rpr_float intensity, rpr_float const * color);

extern "C" RADEONPRORENDERWRAPPER_API int CreateContext(rpr_uint width, rpr_uint height);
extern "C" RADEONPRORENDERWRAPPER_API rpr_image CreateImage(rpr_char *fileName);
extern "C" RADEONPRORENDERWRAPPER_API rpr_light CreateLightFromImage(rpr_image img, rpr_float intensity);
extern "C" RADEONPRORENDERWRAPPER_API rpr_material_node CreateMaterial(rpr_int materialNodeType);
extern "C" RADEONPRORENDERWRAPPER_API rpr_shape CreateMesh(rpr_float const * vertices, size_t num_vertices, rpr_int vertex_stride, rpr_float const * normals, size_t num_normals, rpr_int normal_stride, rpr_float const * texcoords, size_t num_texcoords, rpr_int texcoord_stride, rpr_int const * vertex_indices, rpr_int vidx_stride, rpr_int const * normal_indices, rpr_int nidx_stride, rpr_int const * texcoord_indices, rpr_int tidx_stride, rpr_int const * num_face_vertices, size_t num_faces);
extern "C" RADEONPRORENDERWRAPPER_API rpr_shape CreateMeshInstance(rpr_shape mesh);
extern "C" RADEONPRORENDERWRAPPER_API void DeleteObject(void*obj);
extern "C" RADEONPRORENDERWRAPPER_API void Dispose();
extern "C" RADEONPRORENDERWRAPPER_API void MaterialNodeSetInputFByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_float x, rpr_float y, rpr_float z, rpr_float w);
extern "C" RADEONPRORENDERWRAPPER_API void MaterialNodeSetInputNByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_material_node materialToApplyNode);
extern "C" RADEONPRORENDERWRAPPER_API void MaterialNodeSetInputUByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_uint materialUber);
extern "C" RADEONPRORENDERWRAPPER_API void PositionCamera(rpr_float posx, rpr_float posy, rpr_float posz, rpr_float atx, rpr_float aty, rpr_float atz, rpr_float upx, rpr_float upy, rpr_float upz, rpr_float flength);
extern "C" RADEONPRORENDERWRAPPER_API int RenderToImage(rpr_char *fileName, rpr_int iterations);
extern "C" RADEONPRORENDERWRAPPER_API void TranslateShape(rpr_shape shape, float x, float y, float z);
extern "C" RADEONPRORENDERWRAPPER_API void SetCameraTransform(rpr_float const * transform, rpr_float flength);
extern "C" RADEONPRORENDERWRAPPER_API void SetShapeMaterial(rpr_shape shape, rpr_material_node materialNode);
extern "C" RADEONPRORENDERWRAPPER_API void SetShapeTransform(rpr_shape shape, rpr_float const * worldTransform, rpr_float const * localTransform);
