using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public class Pipeline : RenderPipeline
	{
		public PipelineSettings settings;
		
		internal Context context;
		
		public Pipeline(PipelineSettings settings)
		{
			this.settings = settings;
			
			Deferred.Initialise();
			UnsupportedShaderRenderer.Initialise();
		}
		
		protected override void Render(ScriptableRenderContext _SRPContext, Camera[] cameras)
		{
			Profiler.BeginSample("Mechxel Render Pipeline");
			
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
				
				Deferred.Render(ref context);
				Deferred.Finalise(ref context);
				
				UnsupportedShaderRenderer.DrawUnsupportedShaders(ref context);
			}
			
			Profiler.EndSample();
		}
	}
}