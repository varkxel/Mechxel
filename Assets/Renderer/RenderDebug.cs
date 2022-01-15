using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	partial struct CameraRenderer
	{
		partial void DrawGizmos();
		
		partial void DrawUnsupportedShaders();

		partial void PrepareSceneWindow();
		
		#region Editor
		#if UNITY_EDITOR

		partial void PrepareSceneWindow()
		{
			if(camera.cameraType == CameraType.SceneView)
			{
				ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
			}
		}
		
		partial void DrawGizmos()
		{
			if(Handles.ShouldRenderGizmos())
			{
				context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
				context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
			}
		}
		
		#endif
		#endregion
		
		#region Development
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		
		private static readonly ShaderTagId[] LegacyTagIDs =
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
					_errorMaterial = new Material(Shader.Find("Mechxel/Internal/Error"));
					_errorMaterialSet = true;
				}
				return _errorMaterial;
			}
		}
		
		partial void DrawUnsupportedShaders()
		{
			// Set unsupported shaders to the error material
			DrawingSettings drawSettings = new DrawingSettings(LegacyTagIDs[0], new SortingSettings(camera))
			{
				overrideMaterial = errorMaterial
			};
			for(int i = 1; i < LegacyTagIDs.Length; i++)
			{
				drawSettings.SetShaderPassName(i, LegacyTagIDs[i]);
			}
			
			// Draw error shader
			FilteringSettings filterSettings = FilteringSettings.defaultValue;
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
		}
		
		#endif
		#endregion
	}
}