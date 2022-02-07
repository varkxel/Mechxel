using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class MechxelRendererAsset : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		protected override RenderPipeline CreatePipeline()
		{
			return new MechxelRenderer();
		}
	}
}