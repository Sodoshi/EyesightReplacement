#include "RadeonProRender.h"
#include "Math/mathutils.h"
#include "common/common.h"
#include <cassert>
#include <iostream>
#include "RadeonProRenderWrapper.h"
static rpr_camera camera = nullptr;
static rpr_context context = nullptr;
static rpr_framebuffer frame_buffer = nullptr;
static rpr_framebuffer frame_buffer_resolved = nullptr;
static rpr_material_system matsys = nullptr;
static rpr_scene scene = nullptr;
static rpr_int status = RPR_SUCCESS;
struct UPDATE_CALLBACK_DATA { };
void UpdateCallback(float progress, void* userData )
{
	UPDATE_CALLBACK_DATA* userDataIn = (UPDATE_CALLBACK_DATA*)userData;
	std::cout <<  "progress = "<< (int)(progress*100) << "%" << std::endl;
	return;
}
int CreateContext(rpr_uint width, rpr_uint height)
{
	rpr_int pluginID = rprRegisterPlugin(RPR_PLUGIN_FILE_NAME); 
	CHECK_NE(pluginID , -1)
	rpr_int plugins[] = { pluginID };
	size_t pluginCount = sizeof(plugins) / sizeof(plugins[0]);
	CHECK(rprCreateContext(RPR_API_VERSION, plugins, pluginCount, g_ContextCreationFlags, NULL, NULL, &context));
	UPDATE_CALLBACK_DATA dataCallback;
	CHECK(rprContextSetParameterByKeyPtr(context, RPR_CONTEXT_RENDER_UPDATE_CALLBACK_FUNC, (void*)UpdateCallback));
	CHECK(rprContextSetParameterByKeyPtr(context, RPR_CONTEXT_RENDER_UPDATE_CALLBACK_DATA, &dataCallback));
	CHECK(rprContextSetActivePlugin(context, plugins[0]));
	CHECK(rprContextCreateMaterialSystem(context, 0, &matsys));
	if (status != RPR_SUCCESS)
	{
		return -1;
	}
	CHECK(rprContextCreateScene(context, &scene));
	CHECK(rprContextSetScene(context, scene));
	CHECK(rprContextCreateCamera(context, &camera));
	CHECK(rprSceneSetCamera(scene, camera));
	rpr_framebuffer_desc desc = { width, height };
	rpr_framebuffer_format fmt = {4, RPR_COMPONENT_TYPE_FLOAT32};
	CHECK(rprContextCreateFrameBuffer(context, fmt, &desc, &frame_buffer));
	CHECK(rprContextCreateFrameBuffer(context, fmt, &desc, &frame_buffer_resolved));
	CHECK(rprFrameBufferClear(frame_buffer));
	CHECK(rprContextSetAOV(context, RPR_AOV_COLOR, frame_buffer));
	return 0;
}
rpr_image CreateImage(rpr_char *fileName)
{
	rpr_image img = nullptr;
	{
		CHECK(rprContextCreateImageFromFile(context, fileName, &img));
	}
	return img;
}
rpr_light CreateLightFromImage(rpr_image img, rpr_float intensity)
{
	rpr_light light = nullptr;
	{
		CHECK(rprContextCreateEnvironmentLight(context, &light));
		CHECK(rprEnvironmentLightSetImage(light, img));
		CHECK(rprEnvironmentLightSetIntensityScale(light, intensity));
		CHECK(rprSceneAttachLight(scene, light));
	}
	return light;
}
rpr_material_node CreateMaterial(rpr_int materialNodeType)
{
	rpr_material_node materialNode = nullptr;
	{
		CHECK(rprMaterialSystemCreateNode(matsys, static_cast<rpr_material_node_type>(materialNodeType), &materialNode));
	}
	return materialNode;
}
rpr_shape CreateMesh(rpr_float const * vertices, size_t num_vertices, rpr_int vertex_stride, rpr_float const * normals, size_t num_normals, rpr_int normal_stride, rpr_float const * texcoords, size_t num_texcoords, rpr_int texcoord_stride, rpr_int const * vertex_indices, rpr_int vidx_stride, rpr_int const * normal_indices, rpr_int nidx_stride, rpr_int const * texcoord_indices, rpr_int tidx_stride, rpr_int const * num_face_vertices, size_t num_faces)
{
	rpr_shape mesh = nullptr;
	{
		CHECK(rprContextCreateMesh(context, vertices, num_vertices, vertex_stride, normals, num_normals, normal_stride, texcoords, num_texcoords, texcoord_stride, vertex_indices, vidx_stride, normal_indices, nidx_stride, texcoord_indices, tidx_stride, num_face_vertices, num_faces, &mesh));
		CHECK(rprSceneAttachShape(scene, mesh));
	}
	return mesh;
}
rpr_shape CreateMeshInstance(rpr_shape mesh)
{
	rpr_shape instance = nullptr;
	{
		rprContextCreateInstance(context, mesh, &instance);
		CHECK(rprSceneAttachShape(scene, instance));
	}
	return instance;
}
void DeleteObject(void*obj)
{
	CHECK(rprObjectDelete(obj));
}
void Dispose()
{
	CHECK(rprObjectDelete(frame_buffer));frame_buffer=nullptr;
	CHECK(rprObjectDelete(frame_buffer_resolved));frame_buffer_resolved=nullptr;
	CHECK(rprObjectDelete(camera));camera=nullptr;
	CHECK(rprObjectDelete(scene));scene=nullptr;
	CHECK(rprObjectDelete(matsys));matsys=nullptr;
	CheckNoLeak(context);
	CHECK(rprObjectDelete(context));context=nullptr;
}
void MaterialNodeSetInputFByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_float x, rpr_float y, rpr_float z, rpr_float w)
{
	CHECK(rprMaterialNodeSetInputFByKey(materialNode, static_cast<rpr_material_node_input>(materialNodeInput), x, y, z, w));
}
void MaterialNodeSetInputNByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_material_node materialToApplyNode)
{
	CHECK(rprMaterialNodeSetInputNByKey(materialNode, static_cast<rpr_material_node_input>(materialNodeInput), materialToApplyNode));
}
void MaterialNodeSetInputUByKey(rpr_material_node materialNode, rpr_int materialNodeInput, rpr_uint materialUber)
{
	CHECK(rprMaterialNodeSetInputUByKey(materialNode, static_cast<rpr_material_node_input>(materialNodeInput), materialUber));
}
rpr_float* MultiplyMatrices(rpr_float const * m1, rpr_float const * m2)
{
	RadeonProRender::matrix matrix1 = RadeonProRender::matrix(m1[0], m1[1], m1[2], m1[3], m1[4], m1[5], m1[6], m1[7], m1[8], m1[9], m1[10], m1[11], m1[12], m1[13], m1[14], m1[15]);
	RadeonProRender::matrix matrix2 = RadeonProRender::matrix(m2[0], m2[1], m2[2], m2[3], m2[4], m2[5], m2[6], m2[7], m2[8], m2[9], m2[10], m2[11], m2[12], m2[13], m2[14], m2[15]);
	RadeonProRender::matrix matrix3 = matrix1 * matrix2;
	return &matrix3.m00;
}
void PositionCamera(rpr_float posx, rpr_float posy, rpr_float posz, rpr_float atx, rpr_float aty, rpr_float atz, rpr_float upx, rpr_float upy, rpr_float upz, rpr_float flength)
{
	CHECK(rprCameraLookAt(camera, posx, posy, posz, atx, aty, atz, upx, upy, upz));
	CHECK(rprCameraSetFocalLength(camera, flength));
}
int RenderToImage(rpr_char *fileName, rpr_int iterations)
{
	CHECK(rprContextSetParameterByKey1u(context,RPR_CONTEXT_ITERATIONS,iterations));
	CHECK( rprContextRender(context) );
	CHECK(rprContextResolveFrameBuffer(context,frame_buffer,frame_buffer_resolved,true));
	CHECK( rprFrameBufferSaveToFile(frame_buffer_resolved, fileName) );
	return 0;
}
void SetCameraTransform(rpr_float const * transform, rpr_float flength)
{
	CHECK(rprCameraSetTransform(camera, RPR_TRUE, transform));
	CHECK(rprCameraSetFocalLength(camera, flength));
}
void SetShapeMaterial(rpr_shape shape, rpr_material_node materialNode)
{
	CHECK(rprShapeSetMaterial(shape, materialNode));
}
void SetShapeTransform(rpr_shape shape, rpr_float const * worldTransform, rpr_float const * localTransform)
{
	CHECK(rprShapeSetTransform(shape, RPR_TRUE, MultiplyMatrices(worldTransform, localTransform)));
}
void TranslateShape(rpr_shape shape, float x, float y, float z)
{
	RadeonProRender::matrix m = RadeonProRender::translation(RadeonProRender::float3(x, y, z));
	CHECK(rprShapeSetTransform(shape, RPR_TRUE, &m.m00));
}
rpr_light CreateAmbientLight(rpr_float const * transform)
{
	rpr_light light=nullptr;
	{
		CHECK(rprContextCreateSkyLight(context, &light));
		CHECK(rprSkyLightSetTurbidity(light, 0.f));
		CHECK(rprSceneAttachLight(scene, light));
		//CHECK(rprContextCreateDirectionalLight(context, &light));
		//CHECK(rprLightSetTransform(light, RPR_TRUE, transform));
		//CHECK(rprDirectionalLightSetRadiantPower3f(light, 10000000, 10000000, 10000000));
		//CHECK(rprSceneAttachLight(scene, light));
	}
	return light;
}
rpr_light CreateDirectionalLight(rpr_float const * transform, rpr_float size, rpr_float intensity, rpr_float const * color)
{
	rpr_light light=nullptr;
	{
		CHECK(rprContextCreateSkyLight(context, &light));
		CHECK(rprSkyLightSetTurbidity(light, 0.f));
		CHECK(rprSceneAttachLight(scene, light));
	}
	return light;
}