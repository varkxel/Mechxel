using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using static Mechxel.Renderer.Context;

namespace Mechxel.Renderer
{
	public class TerrainRenderer : RenderPass
	{
		public TerrainRenderer() : base("MechxelTerrain") {}
		
		private static Material BlitMaterial;
		private const string BlitShader = "Hidden/Mechxel/TerrainBlit";
		
		private static bool BlitMaterial_Initialised = false;
		
		public static readonly int materialsProperty = Shader.PropertyToID("voxelMaterials");
		private ComputeBuffer materialsBuffer = null;
		
		public static readonly GBuffer GBuffer0 = new GBuffer("GBuffer0", GraphicsFormat.R16G16B16A16_SFloat);
		
		private static void BlitMaterial_Initialise()
		{
			Shader blitShader = Shader.Find(BlitShader);
			
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			if(blitShader == null)
			{
				Debug.LogError($"Lighting Shader \"{BlitShader}\" was not found.");
				return;
			}
			#endif
			
			BlitMaterial = new Material(blitShader);
		}
		
		public override void Initialise(ref Context context)
		{
			if(!BlitMaterial_Initialised)
			{
				BlitMaterial_Initialise();
				BlitMaterial_Initialised = true;
			}
			
			VoxelMaterialAsset[] materialAssets = context.settings.voxelMaterials;
			int materialCount = materialAssets.Length;
			
			VoxelMaterial[] materials = new VoxelMaterial[materialCount];
			for(int i = 0; i < materialCount; i++)
			{
				materials[i] = materialAssets[i].Material;
			}
			
			materialsBuffer = new ComputeBuffer
			(
				materialCount, VoxelMaterial.Size,
				ComputeBufferType.Structured, ComputeBufferMode.Immutable
			);
			Shader.SetGlobalBuffer(materialsProperty, materialsBuffer);
		}
		
		public override void Render(ref Context context)
		{
			Geometry(ref context);
			ScreenSpace(ref context);
			Finalize(ref context);
		}
		
		private void Geometry(ref Context context)
		{
			RenderTextureDescriptor GBuffer0_Desc = GBuffer0.Descriptor(context.renderSize);
			
			context.StartBuffer("Create Terrain GBuffers");
				buffer.GetTemporaryRT(GBuffer0.ID, GBuffer0_Desc, FilterMode.Point);
			context.EndBuffer();
			
			context.StartBuffer("Bind Terrain GBuffers");
				buffer.SetRenderTarget(GBuffer0.RT, DepthBuffer.RT);
				buffer.ClearRenderTarget(false, true, Color.black);
			context.EndBuffer();
			
			// Render
			SortingSettings sortSettings = new SortingSettings(context.camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings drawSettings = new DrawingSettings(LightingPassTag, sortSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);
			context.SRPContext.DrawRenderers(context.culling, ref drawSettings, ref filterSettings);
			
			context.SRPContext.Submit();
		}
		
		private void ScreenSpace(ref Context context)
		{
			context.StartBuffer("Terrain Blit");
				buffer.Blit
				(
					BuiltinRenderTextureType.CameraTarget,
					BuiltinRenderTextureType.CameraTarget,
					BlitMaterial
				);
			context.EndBuffer();
			context.SRPContext.Submit();
		}
		
		private void Finalize(ref Context context)
		{
			context.StartBuffer("Destroy Terrain GBuffers");
				buffer.ReleaseTemporaryRT(GBuffer0.ID);
			context.EndBuffer();
			context.SRPContext.Submit();
		}
		
		public override void Dispose(ref Context context)
		{
			materialsBuffer.Dispose();
			materialsBuffer = null;
		}
	}
}