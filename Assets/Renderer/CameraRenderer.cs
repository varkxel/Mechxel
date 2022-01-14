using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel
{
	public struct CameraRenderer
	{
		public ScriptableRenderContext context;
		public Camera camera;
		
		public void Render()
		{
			Setup();
			DrawVisibleGeometry();
			Submit();
		}
		
		private void Setup()
		{
			context.SetupCameraProperties(camera);
		}
		
		private void DrawVisibleGeometry()
		{
			context.DrawSkybox(camera);
		}
		
		private void Submit()
		{
			context.Submit();
		}
	}
}