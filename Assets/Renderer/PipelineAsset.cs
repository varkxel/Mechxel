using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Rendering/Mechxel Render Pipeline")]
	public class PipelineAsset : RenderPipelineAsset
	{
		protected override RenderPipeline CreatePipeline()
		{
			return new Pipeline();
		}
	}
}
