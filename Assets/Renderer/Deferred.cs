using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using static Mechxel.Renderer.Context;

namespace Mechxel.Renderer
{
	public static class Deferred
	{
		public const string LightingPassTag_Name = "MechxelDeferred";
		public static readonly ShaderTagId LightingPassTag = new ShaderTagId(LightingPassTag_Name);
		
		public const string GBuffer0_Name = "GBuffer0";
		public const string GBuffer1_Name = "GBuffer1";
		public const string Depth_Name = "_CameraDepthTexture";
		
		public static readonly int GBuffer0_ID = Shader.PropertyToID(GBuffer0_Name);
		public static readonly int GBuffer1_ID = Shader.PropertyToID(GBuffer1_Name);
		public static readonly int Depth_ID = Shader.PropertyToID(Depth_Name);
		
		private static readonly RenderTargetIdentifier GBuffer0_RT = new RenderTargetIdentifier(GBuffer0_ID);
		private static readonly RenderTargetIdentifier GBuffer1_RT = new RenderTargetIdentifier(GBuffer1_ID);
		private static readonly RenderTargetIdentifier Depth_RT = new RenderTargetIdentifier(Depth_ID);
		
		private static readonly RenderTargetIdentifier[] GBuffers = new RenderTargetIdentifier[]
		{
			GBuffer0_RT,
			GBuffer1_RT
		};
		
		private static Material lightingMaterial = null;
		
		private static void SetLightingMaterial()
		{
			// Lighting material isn't set, set it.
			const string lightingShader_path = "Hidden/Mechxel/DeferredLighting";
			Shader lightingShader = Shader.Find(lightingShader_path);
			
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if(lightingShader == null)
			{
				Debug.LogError($"Lighting Shader \"{lightingShader_path}\" not found.");
				return;
			}
			#endif
			
			lightingMaterial = new Material(lightingShader);
		}
		
		internal static void Initialise()
		{
			SetLightingMaterial();
		}
		
		private const GraphicsFormat HDRFormat = GraphicsFormat.R16G16B16A16_SFloat;
		private const GraphicsFormat SDRFormat = GraphicsFormat.R8G8B8A8_SRGB;
		private const DepthFormat DepthBufferFormat = DepthFormat.Bits32;
		
		private static RenderTextureDescriptor GBufferDescriptor(int width, int height, GraphicsFormat format) =>
			new RenderTextureDescriptor(width, height)
			{
				graphicsFormat = format,
				sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear,
				
				enableRandomWrite = false,
				msaaSamples = 1,
				depthBufferBits = DepthFormat.None.Bits()
			};
		
		private static RenderTextureDescriptor DepthDescriptor(int width, int height) =>
			new RenderTextureDescriptor(width, height)
			{
				colorFormat = RenderTextureFormat.Depth,
				depthBufferBits = DepthBufferFormat.Bits(),
				
				enableRandomWrite = false,
				msaaSamples = 1
			};
		
		public delegate void SkyboxDelegate(ref Context context);
		public static SkyboxDelegate OnDrawSkybox = (ref Context context) =>
		{
			context.SRPContext.DrawSkybox(context.camera);
		};
		
		internal static void RenderDeferred(ref Context context)
		{
			int width = context.camera.pixelWidth;
			int height = context.camera.pixelHeight;
			
			RenderTextureDescriptor GBuffer0_Desc = GBufferDescriptor(width, height, HDRFormat);
			RenderTextureDescriptor GBuffer1_Desc = GBufferDescriptor(width, height, SDRFormat);
			RenderTextureDescriptor Depth_Desc = DepthDescriptor(width, height);
			
			context.StartBuffer("Create RenderTargets");
				buffer.GetTemporaryRT(GBuffer0_ID, GBuffer0_Desc, FilterMode.Point);
				buffer.GetTemporaryRT(GBuffer1_ID, GBuffer1_Desc, FilterMode.Point);
				buffer.GetTemporaryRT(Depth_ID, Depth_Desc, FilterMode.Point);
			context.EndBuffer();
			
			context.StartBuffer("Bind RenderTargets");
				buffer.SetRenderTarget(GBuffers, Depth_RT);
				buffer.ClearRenderTarget(true, true, Color.black);
			context.EndBuffer();
			
			// Draw skybox
			OnDrawSkybox.Invoke(ref context);
			
			// Render
			SortingSettings sortSettings = new SortingSettings(context.camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings drawSettings = new DrawingSettings(LightingPassTag, sortSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);
			context.SRPContext.DrawRenderers(context.culling, ref drawSettings, ref filterSettings);
			
			// Screen Space Combine Step
			context.StartBuffer("Lighting Blit");
				buffer.Blit
				(
					BuiltinRenderTextureType.CameraTarget,
					BuiltinRenderTextureType.CameraTarget,
					lightingMaterial
				);
			context.EndBuffer();
			
			context.StartBuffer("Cleanup RenderTargets");
				buffer.ReleaseTemporaryRT(GBuffer0_ID);
				buffer.ReleaseTemporaryRT(GBuffer1_ID);
				buffer.ReleaseTemporaryRT(Depth_ID);
			context.EndBuffer();
			
			context.SRPContext.Submit();
		}
	}
}