using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	partial class MechxelRenderer
	{
		#region Scene View Setup
		
		partial void PrepareSceneView(Camera camera);
		
		#if UNITY_EDITOR
		partial void PrepareSceneView(Camera camera)
		{
			if(camera.cameraType == CameraType.SceneView)
			{
				ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
			}
		}
		#endif
		
		#endregion
		
		#region Gizmo Drawing
		
		partial void DrawGizmos(ScriptableRenderContext context, Camera camera);
		
		#if UNITY_EDITOR
		partial void DrawGizmos(ScriptableRenderContext context, Camera camera)
		{
			if(Handles.ShouldRenderGizmos())
			{
				context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
				context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
			}
		}
		#endif
		
		#endregion
		
		#region Invalid Shader Rendering
		
		partial void DrawUnsupportedShaders
		(
			ref ScriptableRenderContext context, Camera camera,
			ref CullingResults cullResults
		);
		
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		
		private static readonly ShaderTagId[] LegacyTagIDs = new ShaderTagId[]
		{
			new ShaderTagId("Always"),
			new ShaderTagId("ForwardBase"),
			new ShaderTagId("PrepassBase"),
			new ShaderTagId("Vertex"),
			new ShaderTagId("VertexLMRGBM"),
			new ShaderTagId("VertexLM")
		};
		
		// Error material
		private static Material _errorMaterial = null;
		private static bool _errorMaterialSet = false;
		
		public static Material errorMaterial
		{
			get
			{
				if(!_errorMaterialSet)
				{
					_errorMaterial = new Material(Shader.Find("Hidden/Mechxel/Error"));
					_errorMaterialSet = true;
				}
				return _errorMaterial;
			}
		}
		
		partial void DrawUnsupportedShaders
		(
			ref ScriptableRenderContext context, Camera camera,
			ref CullingResults cullResults
		)
		{
			DrawingSettings drawSettings = new DrawingSettings(LegacyTagIDs[0], new SortingSettings(camera))
			{
				overrideMaterial = errorMaterial
			};
			for(int i = 1; i < LegacyTagIDs.Length; i++)
			{
				drawSettings.SetShaderPassName(i, LegacyTagIDs[i]);
			}
			
			// Draw the error shader on the unsupported objects.
			FilteringSettings filterSettings = FilteringSettings.defaultValue;
			context.DrawRenderers(cullResults, ref drawSettings, ref filterSettings);
		}
		
		#endif
		
		#endregion
	}
}