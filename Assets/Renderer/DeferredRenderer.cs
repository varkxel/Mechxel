using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public partial class CameraRenderer
	{
		private static readonly ShaderTagId DeferredLitID = new ShaderTagId("DeferredLit");
		
		public void RenderDeferred
		(
			ref DrawingSettings drawSettings,
			ref FilteringSettings filterSettings
		)
		{
			drawSettings.SetShaderPassName(0, DeferredLitID);
			
			// Render Targets
			RenderTexture albedoSpecular = RenderTexture.GetTemporary(1634, 890);
			RenderTexture positions = RenderTexture.GetTemporary(1634, 890);
			RenderTexture normals = RenderTexture.GetTemporary(1634, 890);
			
			RenderTargetIdentifier[] targets = new RenderTargetIdentifier[] { albedoSpecular, positions, normals };
			
			commandBuffer.SetRenderTarget(targets, new RenderTargetIdentifier(BuiltinRenderTextureType.None));
			ExecuteCommandBuffer();
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
		}
	}
}