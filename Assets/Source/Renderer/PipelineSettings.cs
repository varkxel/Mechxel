using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class PipelineSettings : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		[Header("Voxel Materials")]
		public VoxelMaterialAsset[] voxelMaterials;
		
		protected override RenderPipeline CreatePipeline()
		{
			return new Pipeline(this);
		}
	}
}