using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public class MechxelRenderer : RenderPipeline
	{
		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			BeginFrameRendering(context, cameras);
			
			// Render each camera
			for(int i = 0; i < cameras.Length; i++)
			{
				Camera camera = cameras[i];
				BeginCameraRendering(context, camera);
				
				// Get culling results
				if(!camera.TryGetCullingParameters(out ScriptableCullingParameters cullParameters))
				{
					Debug.LogWarning("Failed to get culling parameters.");
					continue;
				}
				CullingResults cullResult = context.Cull(ref cullParameters);
				
				// Setup camera builtin variables
				context.SetupCameraProperties(camera);
				
				bool drawSkybox = (camera.clearFlags == CameraClearFlags.Skybox);
				
				EndCameraRendering(context, camera);
			}
			
			EndFrameRendering(context, cameras);
		}
	}
}