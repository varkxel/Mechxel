using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	partial class MechxelRenderer
	{
		private static readonly ShaderTagId PassName = new ShaderTagId("MechxelDeferred");
		
		private static readonly int GBuffer0_ID = Shader.PropertyToID("GBuffer0");
		private static readonly int GBuffer1_ID = Shader.PropertyToID("GBuffer1");
		private static readonly int Depth_ID = Shader.PropertyToID("_CameraDepthTexture");
		
		private static readonly RenderTargetIdentifier GBuffer0_RT = new RenderTargetIdentifier(GBuffer0_ID);
		private static readonly RenderTargetIdentifier GBuffer1_RT = new RenderTargetIdentifier(GBuffer1_ID);
		private static readonly RenderTargetIdentifier Depth_RT = new RenderTargetIdentifier(Depth_ID);
		
		private static readonly RenderTargetIdentifier[] GBuffers = new RenderTargetIdentifier[]
		{
			GBuffer0_RT,
			GBuffer1_RT
		};
		
		private Material lightingMaterial;
		
		private void DeferredRendererSetup()
		{
			lightingMaterial = new Material(Shader.Find("Hidden/Mechxel/DeferredLighting"));
		}
		
		private void RenderDeferred
		(
			ref ScriptableRenderContext context, Camera camera,
			ref CullingResults cullResult,
			bool drawSkybox = true
		)
		{
			RenderTextureDescriptor GBuffer0_Desc = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight)
			{
				graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat,
				sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear,
				enableRandomWrite = false,
				msaaSamples = 1,
				depthBufferBits = 0
			};
			RenderTextureDescriptor GBuffer1_Desc = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight)
			{
				graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB,
				sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear,
				enableRandomWrite = false,
				msaaSamples = 1,
				depthBufferBits = 0
			};
			RenderTextureDescriptor depthBuffer = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight)
			{
				colorFormat = RenderTextureFormat.Depth,
				depthBufferBits = 24,
				enableRandomWrite = false,
				msaaSamples = 1
			};
			using(CommandBuffer tempRTBuffer = new CommandBuffer()
			{
				#if UNITY_EDITOR || DEVELOPMENT_BUILD
				name = $"({camera.name}) Setup TempRTs"
				#endif
			})
			{
				tempRTBuffer.GetTemporaryRT(GBuffer0_ID, GBuffer0_Desc, FilterMode.Point);
				tempRTBuffer.GetTemporaryRT(GBuffer1_ID, GBuffer1_Desc, FilterMode.Point);
				tempRTBuffer.GetTemporaryRT(Depth_ID, depthBuffer, FilterMode.Point);
				context.ExecuteCommandBuffer(tempRTBuffer);
			}
			
			SortingSettings sortSettings = new SortingSettings(camera);
			DrawingSettings drawSettings = new DrawingSettings(PassName, sortSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.all);
			
			using(CommandBuffer geometryPassBuffer = new CommandBuffer())
			{
				geometryPassBuffer.SetRenderTarget(GBuffers, Depth_RT);
				geometryPassBuffer.ClearRenderTarget(true, true, Color.black);
				context.ExecuteCommandBuffer(geometryPassBuffer);
			}
			
			if(drawSkybox) context.DrawSkybox(camera);
			
			// Opaque
			sortSettings.criteria = SortingCriteria.CommonOpaque;
			drawSettings.sortingSettings = sortSettings;
			filterSettings.renderQueueRange = RenderQueueRange.opaque;
			context.DrawRenderers(cullResult, ref drawSettings, ref filterSettings);
			
			// Transparent
			sortSettings.criteria = SortingCriteria.CommonTransparent;
			drawSettings.sortingSettings = sortSettings;
			filterSettings.renderQueueRange = RenderQueueRange.transparent;
			context.DrawRenderers(cullResult, ref drawSettings, ref filterSettings);
			
			// Screen Space Combine Step
			using(CommandBuffer blitBuffer = new CommandBuffer()
			{
				#if UNITY_EDITOR || DEVELOPMENT_BUILD
				name = $"({camera.name}) Lighting Step"
				#endif
			})
			{
				blitBuffer.Blit
				(
					BuiltinRenderTextureType.CameraTarget,
					BuiltinRenderTextureType.CameraTarget,
					lightingMaterial
				);
				context.ExecuteCommandBuffer(blitBuffer);
			}

			using(CommandBuffer cleanupBuffer = new CommandBuffer()
			{
				#if UNITY_EDITOR || DEVELOPMENT_BUILD
				name = $"({camera.name}) Cleanup"
				#endif
			})
			{
				cleanupBuffer.ReleaseTemporaryRT(GBuffer0_ID);
				cleanupBuffer.ReleaseTemporaryRT(GBuffer1_ID);
				cleanupBuffer.ReleaseTemporaryRT(Depth_ID);
				context.ExecuteCommandBuffer(cleanupBuffer);
			}
			
			context.Submit();
		}
	}
}