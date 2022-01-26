using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public partial class CameraRenderer
	{
		private static readonly ShaderTagId UnlitTagID = new ShaderTagId("SRPDefaultUnlit");
		
		// Parameters
		public ScriptableRenderContext context { get; private set; }
		public Camera camera { get; private set; }
		
		// Context variables
		private const string commandBufferName = "Render Camera";
		public CommandBuffer commandBuffer { get; private set; } = new CommandBuffer();
		
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
		}
		
		public void Render()
		{
			PrepareBuffer();
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
			
			// Camera layers
			CameraClearFlags flags = camera.clearFlags;
			
			// Clear depth & colour (dependent on camera flags)
			commandBuffer.ClearRenderTarget
			(
				flags <= CameraClearFlags.Depth,
				flags == CameraClearFlags.Color,
				flags == CameraClearFlags.Color ?
					camera.backgroundColor.linear : Color.clear
			);
			
			commandBuffer.BeginSample(sampleName);
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
			commandBuffer.EndSample(sampleName);
			ExecuteCommandBuffer();
			context.Submit();
		}
	}
}