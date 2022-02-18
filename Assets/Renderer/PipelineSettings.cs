using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Mechxel.Renderer
{
	[CreateAssetMenu(menuName = "Mechxel/Renderer Asset", fileName = Name)]
	public class PipelineSettings : RenderPipelineAsset
	{
		private const string Name = "Mechxel Renderer";
		
		[Header("Draw Call Management")]
		public bool useSRPBatcher = true;
		public bool useInstancing = false;
		public bool useDynamicBatching = false;
		
		protected override RenderPipeline CreatePipeline()
		{
			return new Pipeline();
		}
	}
}