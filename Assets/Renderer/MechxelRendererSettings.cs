using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class MechxelRendererSettings : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		protected override RenderPipeline CreatePipeline()
		{
			return new MechxelRenderer();
		}
	}
}