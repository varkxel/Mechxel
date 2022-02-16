using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Mechxel.Renderer
{
	public class MechxelRenderer : RenderPipeline
	{
		public MechxelRendererSettings settings;
		
		private Material lightingMaterial;
		
		public GraphicsFormat graphicsFormat { get; private set; }
		private void UpdateGraphicsFormat()
			=> graphicsFormat = graphicsFormat = SystemInfo.GetGraphicsFormat(settings.graphicsFormat);
		
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
		
		public MechxelRenderer(MechxelRendererSettings settings)
		{
			this.settings = settings;
			
			UpdateGraphicsFormat();
			settings.OnUpdated.AddListener(UpdateGraphicsFormat);
			
			lightingMaterial = new Material(Shader.Find("Hidden/Mechxel/DeferredLighting"));
		}
		
		protected override void Dispose(bool disposing)
		{
			if(disposing == false) return;
			
			settings.OnUpdated.RemoveListener(UpdateGraphicsFormat);
		}
		
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
				
				RenderTextureDescriptor colourBuffer = new RenderTextureDescriptor(camera.pixelWidth, camera.pixelHeight)
				{
					graphicsFormat = this.graphicsFormat,
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
					tempRTBuffer.GetTemporaryRT(GBuffer0_ID, colourBuffer, FilterMode.Point);
					tempRTBuffer.GetTemporaryRT(GBuffer1_ID, colourBuffer, FilterMode.Point);
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
				
				EndCameraRendering(context, camera);
			}
			
			EndFrameRendering(context, cameras);
		}
	}
}