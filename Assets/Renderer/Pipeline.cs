using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public class Pipeline : RenderPipeline
	{
		public Pipeline()
		{
			GraphicsSettings.useScriptableRenderPipelineBatching = true;
		}
		
		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			for(int i = 0; i < cameras.Length; i++)
			{
				CameraRenderer renderer = new CameraRenderer(context, cameras[i]);
				renderer.Render();
			}
		}
	}
}