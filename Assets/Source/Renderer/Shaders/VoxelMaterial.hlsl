#ifndef MECHXEL_VOXELMATERIAL_INCLUDED
#define MECHXEL_VOXELMATERIAL_INCLUDED

struct VoxelMaterial
{
	float3 colourA;
	float roughness;
	
	float3 colourB;
	float metallic;
};

StructuredBuffer<VoxelMaterial> voxelMaterials;

#endif