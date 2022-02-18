using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public class Pipeline : RenderPipeline
	{
		internal Context context;
		
		public Pipeline()
		{
			Deferred.Initialise();
			UnsupportedShaderRenderer.Initialise();
		}
		
		protected override void Render(ScriptableRenderContext _SRPContext, Camera[] cameras)
		{
			context = new Context { SRPContext = _SRPContext };
			
			// Render each camera
			for(int i = 0; i < cameras.Length; i++)
			{
				context.camera = cameras[i];
				
				// Initialise Command Buffer
				context.BufferInit();
				
				// Occlusion/Frustum culling
				if(!context.GetCullingResults()) continue;
				
				// Initialise camera
				context.CameraInit();
				
				UnsupportedShaderRenderer.DrawUnsupportedShaders(ref context);
				Deferred.RenderDeferred(ref context);
			}
		}
	}
}