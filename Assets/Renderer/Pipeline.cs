using System.Collections.Generic;
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
		
		protected List<RenderPass> renderPasses = new List<RenderPass>(new RenderPass[]
		{
			new TerrainRenderer()
		});
		
		public Pipeline(PipelineSettings settings)
		{
			this.settings = settings;
			
			UnsupportedShaderRenderer.Initialise();
			for(int i = 0; i < renderPasses.Count; i++) renderPasses[i].Initialise();
		}
		
		protected override void Render(ScriptableRenderContext _SRPContext, Camera[] cameras)
		{
			Profiler.BeginSample("Mechxel Render Pipeline");
			
			context = new Context { SRPContext = _SRPContext };
			
			// Render each camera
			for(int cameraIndex = 0; cameraIndex < cameras.Length; cameraIndex++)
			{
				context.camera = cameras[cameraIndex];
				
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
				RenderTextureDescriptor DepthBuffer_Desc = DepthBuffer.Descriptor(context.renderSize);
				context.StartBuffer("Setup Depth Buffer");
					buffer.GetTemporaryRT(DepthBuffer.ID, DepthBuffer_Desc, FilterMode.Point);
					buffer.SetRenderTarget(DepthBuffer.RT);
					buffer.ClearRenderTarget(true, false, Color.black);
				context.EndBuffer();
				context.SRPContext.Submit();
				
				// Debug/Editor rendering
				UnsupportedShaderRenderer.DrawUnsupportedShaders(ref context);
				GizmoRenderer.DrawGizmos(ref context);
				
				//context.SRPContext.DrawSkybox(context.camera);
				
				// Render the passes
				for(int i = 0; i < renderPasses.Count; i++) renderPasses[i].Render(ref context);
				
				// Destroy depth buffer
				context.StartBuffer("Destroy Depth Buffer");
					buffer.ReleaseTemporaryRT(DepthBuffer.ID);
				context.EndBuffer();
				context.SRPContext.Submit();
			}
			
			Profiler.EndSample();
		}
	}
}