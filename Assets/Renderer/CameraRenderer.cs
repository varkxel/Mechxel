using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel
{
	public struct CameraRenderer
	{
		// Parameters
		public ScriptableRenderContext context { get; private set; }
		public Camera camera { get; private set; }
		
		// Context variables
		public CommandBuffer commandBuffer { get; private set; }

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
		}
		
		public void Render()
		{
			Setup();
			DrawVisibleGeometry();
			Submit();
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
			context.DrawSkybox(camera);
		}
		
		private void Submit()
		{
			commandBuffer.EndSample(commandBuffer.name);
			ExecuteCommandBuffer();
			context.Submit();
		}
	}
}