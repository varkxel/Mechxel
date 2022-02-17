using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public partial class MechxelRenderer : RenderPipeline
	{
		public MechxelRenderer()
		{
			DeferredRendererSetup();
		}
		
		protected override void Render(ScriptableRenderContext context, Camera[] cameras)
		{
			BeginFrameRendering(context, cameras);
			
			// Render each camera
			for(int i = 0; i < cameras.Length; i++)
			{
				BeginCameraRendering(context, cameras[i]);
				
				// Get culling results
				if(!cameras[i].TryGetCullingParameters(out ScriptableCullingParameters cullParameters))
				{
					Debug.LogWarning("Failed to get culling parameters.");
					continue;
				}
				CullingResults cullResult = context.Cull(ref cullParameters);
				
				// Setup camera builtin variables
				context.SetupCameraProperties(cameras[i]);
				
				bool drawSkybox = (cameras[i].clearFlags == CameraClearFlags.Skybox);
				
				PrepareSceneView(cameras[i]);
				
				DrawUnsupportedShaders(ref context, cameras[i], ref cullResult);
				RenderDeferred(ref context, cameras[i], ref cullResult, drawSkybox);
				
				//DrawGizmos(context, cameras[i]);
				
				EndCameraRendering(context, cameras[i]);
			}
			
			EndFrameRendering(context, cameras);
		}
	}
}