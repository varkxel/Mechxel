using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using static Unity.Mathematics.math;

using Mechxel.Renderer.Development;
using static Mechxel.Renderer.Context;

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
				
				// Initialise size property
				context.renderSize = int2
				(
					context.camera.pixelWidth,
					context.camera.pixelHeight
				);
				
				// Initialise depth buffer
				RenderTextureDescriptor DepthBuffer_Desc = DepthDescriptor(context.renderSize);
				context.StartBuffer("Setup Depth Buffer");
					buffer.GetTemporaryRT(DepthBuffer_ID, DepthBuffer_Desc, FilterMode.Point);
				context.EndBuffer();
				context.SRPContext.Submit();
				
				// Deferred rendering
				Deferred.Render(ref context);
				Deferred.Finalise(ref context);
				
				// Terrain rendering
				
				
				// Debug/Editor rendering
				UnsupportedShaderRenderer.DrawUnsupportedShaders(ref context);
				GizmoRenderer.DrawGizmos(ref context);
				
				// Destroy depth buffer
				context.StartBuffer("Destroy Depth Buffer");
					buffer.ReleaseTemporaryRT(DepthBuffer_ID);
				context.EndBuffer();
				context.SRPContext.Submit();
			}
			
			Profiler.EndSample();
		}
	}
}