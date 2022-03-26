using UnityEngine;
using static Unity.Mathematics.math;

namespace Mechxel.Renderer
{
	/// <summary>
	/// <see cref="ScriptableObject"/> wrapper for <see cref="VoxelMaterial"/>.
	/// </summary>
	[CreateAssetMenu]
	public class VoxelMaterialAsset : ScriptableObject
	{
		[Header("Colour")]
		public Color colourA = Color.white;
		public Color colourB = Color.white;
		
		[Header("Material Values")]
		[Range(0.0f, 1.0f)] public float roughness = 0.5f;
		[Range(0.0f, 1.0f)] public float metallic = 0.0f;
		
		public VoxelMaterial Material => new VoxelMaterial
		{
			colourA = float3(this.colourA.r, this.colourA.g, this.colourA.b),
			colourB = float3(this.colourB.r, this.colourB.g, this.colourB.b),
			roughness = this.roughness,
			metallic = this.metallic
		};
	}
}