using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public partial struct CameraRenderer
	{
		// Parameters
		public ScriptableRenderContext context { get; private set; }
		public Camera camera { get; private set; }
		
		// Constants
		private static readonly ShaderTagId UnlitTagID = new ShaderTagId("SRPDefaultUnlit");
		
		// Context variables
		public CommandBuffer commandBuffer { get; private set; }
		public CullingResults cullingResults { get; private set; }
		
		private void ExecuteCommandBuffer()
		{
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}
		
		public CameraRenderer(ScriptableRenderContext context, Camera camera)
		{
			this.context = context;
			this.camera = camera;
			
			commandBuffer = new CommandBuffer
			{
				name = $"Render Camera \"{camera.name}\""
			};
			cullingResults = default;
		}
		
		public void Render()
		{
			PrepareSceneWindow();
			if(!Cull()) return;
			
			Setup();
			
			DrawVisibleGeometry();
			
			// Draw after geometry step to show above transparent materials.
			DrawUnsupportedShaders();
			
			// Draw Gizmos last, on top of everything else
			DrawGizmos();
			
			Submit();
		}
		
		private bool Cull()
		{
			if(camera.TryGetCullingParameters(out ScriptableCullingParameters cullParameters))
			{
				cullingResults = context.Cull(ref cullParameters);
				return true;
			}
			else return false;
		}
		
		private void Setup()
		{
			context.SetupCameraProperties(camera);
			
			// Clear depth & colour
			commandBuffer.ClearRenderTarget(true, true, Color.clear);
			
			commandBuffer.BeginSample(commandBuffer.name);
			ExecuteCommandBuffer();
		}
		
		private void DrawVisibleGeometry()
		{
			// Set to draw opaque
			SortingSettings sortSettings = new SortingSettings(camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings   drawSettings   = new DrawingSettings(UnlitTagID, sortSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);
			
			// Draw opaque
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
			context.DrawSkybox(camera);
			
			// Set to draw transparent
			sortSettings.criteria = SortingCriteria.CommonTransparent;
			drawSettings.sortingSettings = sortSettings;
			filterSettings.renderQueueRange = RenderQueueRange.transparent;
			
			// Draw transparent
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
		}
		
		private void Submit()
		{
			commandBuffer.EndSample(commandBuffer.name);
			ExecuteCommandBuffer();
			context.Submit();
		}
	}
}