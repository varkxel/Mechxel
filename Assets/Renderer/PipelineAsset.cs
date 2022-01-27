using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Rendering/Mechxel Render Pipeline")]
	public class PipelineAsset : RenderPipelineAsset
	{
		[Header("Draw Call Management")]
		public bool useSRPBatcher = true;
		public bool useInstancing = false;
		public bool useDynamicBatching = false;
		
		protected override RenderPipeline CreatePipeline()
		{
			return new Pipeline(useSRPBatcher, useInstancing, useDynamicBatching);
		}
	}
}
