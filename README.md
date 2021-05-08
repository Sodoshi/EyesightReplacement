# EyesightReplacement
Replacing the Eyesight.exe in Lego Studio with something that works on my GPU


Parsing the eyesight .dae means:

1. Open with an xml reader and add the following namespaces:

"C", "http://www.collada.org/2005/11/COLLADASchema"
"RoundingEdgeNormal", "http://www.w3.org/2001/XMLSchema-instance"
"RoughSurface", "http://www.w3.org/2001/XMLSchema-instance"
"PearlGroupPrincipledBSDF", "http://www.w3.org/2001/XMLSchema-instance"

2. Select single node: //C:scene/C:instance_visual_scene
	The url tag will be the name of your scene

3. Select single node: $"//C:visual_scene[@id='{visualSceneUrl}']"

	This will contain your camera, lighting, world matrix and id of the first node in your scene.

	The matrix elements are made up of 16 floats, m11,m12,m13,m14m21...m44
	(See: https://docs.microsoft.com/en-us/dotnet/api/system.numerics.matrix4x4?view=net-5.0 )

4. Select single node: $"//C:library_nodes//C:node[@id='{nodeName}']"
	If it has a child of "C:matrix"
	Multiply the World Matrix by this matrix and pass down:
		See: https://github.com/mono/opentk/blob/main/Source/Compatibility/Math/Matrix4.cs
		Line: 867

	For each child:
		If it's of type "instance_node", get url attribute, repeat #4

	If it's of type "node":
		If it has an "id" attribute, get it and repeat #4
		Else if it has a child of "C:instance_geometry":
			Step #5

5. Multiply the Parent matrix with the "C:matrix" child element for the local transform
	Get the "instance_material" child and extract "target" attribute

6. Get "//C:material" with an id matching target. Get the url of the child "C:instance_effect"

7. Get "//C:effect" with an id matching url, the "C:phong" child is the material data

8. Pass everything to a rendering engine.
