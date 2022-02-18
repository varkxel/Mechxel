using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	internal static class UnsupportedShaderRenderer
	{
		#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
		
		internal static void Initialise() {}
		
		internal static void DrawUnsupportedShaders(ref Context context) {}
		
		#else
		
		private static Material errorMaterial = null;
		
		internal static void Initialise()
		{
			errorMaterial = new Material(Shader.Find("Hidden/Mechxel/Error"));
		}
		
		private static readonly ShaderTagId[] LegacyTagIDs = new ShaderTagId[]
		{
			new ShaderTagId("Always"),
			new ShaderTagId("ForwardBase"),
			new ShaderTagId("PrepassBase"),
			new ShaderTagId("Vertex"),
			new ShaderTagId("VertexLMRGBM"),
			new ShaderTagId("VertexLM")
		};
		
		internal static void DrawUnsupportedShaders(ref Context context)
		{
			DrawingSettings drawSettings = new DrawingSettings(LegacyTagIDs[0], new SortingSettings(context.camera))
			{
				overrideMaterial = errorMaterial
			};
			for(int i = 1; i < LegacyTagIDs.Length; i++)
			{
				drawSettings.SetShaderPassName(i, LegacyTagIDs[i]);
			}
			
			// Draw the error shader on the unsupported objects.
			FilteringSettings filterSettings = FilteringSettings.defaultValue;
			context.SRPContext.DrawRenderers(context.culling, ref drawSettings, ref filterSettings);
			context.SRPContext.Submit();
		}
		
		#endif
	}
}