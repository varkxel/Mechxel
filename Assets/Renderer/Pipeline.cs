using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public class Pipeline : RenderPipeline
	{
		private bool useSRPBatcher = true;
		private bool useInstancing = true;
		private bool useDynamicBatching = true;
		
		public Pipeline(bool useSRPBatcher, bool useInstancing, bool useDynamicBatching)
		{
			this.useSRPBatcher = useSRPBatcher;
			GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatcher;
			
			this.useInstancing = useInstancing;
			this.useDynamicBatching = useDynamicBatching;
		}
		
		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			for(int i = 0; i < cameras.Length; i++)
			{
				CameraRenderer renderer = new CameraRenderer(context, cameras[i]);
				renderer.Render(useDynamicBatching, useInstancing);
			}
		}
	}
}