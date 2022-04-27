using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class PipelineSettings : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		public VoxelMaterialAsset[] voxelMaterials;
		public Pipeline pipeline { get; private set; }
		
		protected override RenderPipeline CreatePipeline()
		{
			pipeline = new Pipeline(this);
			return pipeline;
		}
	}
}