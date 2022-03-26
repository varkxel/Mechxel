using Unity.Mathematics;
using UnityEngine;

namespace Mechxel.Renderer
{
	/// <summary>
	/// Packed struct containing all the parameters for the voxel material in the shader.
	/// </summary>
	[System.Serializable]
	public struct VoxelMaterial
	{
		public const int Size = sizeof(float) * 8;
		
		// vec1 (float4)
		public float3 colourA;
		public float roughness;
		
		// vec2 (float4)
		public float3 colourB;
		public float metallic;
	}
}