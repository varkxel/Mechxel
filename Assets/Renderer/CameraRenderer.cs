using UnityEngine;
using UnityEngine.Rendering;

namespace Mechxel.Renderer
{
	public struct CameraRenderer
	{
		// Parameters
		public ScriptableRenderContext context { get; private set; }
		public Camera camera { get; private set; }
		
		// Constants
		private static readonly ShaderTagId UnlitTagID = new ShaderTagId("SRPDefaultUnlit");
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
		
		// Context variables
		public CommandBuffer commandBuffer { get; private set; }
		public CullingResults cullingResults { get; private set; }
		
		private void ExecuteCommandBuffer()
		{
			context.ExecuteCommandBuffer(commandBuffer);
			commandBuffer.Clear();
		}
		
		public CameraRenderer(ScriptableRenderContext context, Camera camera)
		{
			this.context = context;
			this.camera = camera;
			
			commandBuffer = new CommandBuffer
			{
				name = $"Render Camera \"{camera.name}\""
			};
			cullingResults = default;
		}
		
		public void Render()
		{
			if(!Cull()) return;
			
			Setup();
			
			DrawVisibleGeometry();
			
			// Draw after geometry step to show above transparent materials
			DrawUnsupportedShaders();
			
			Submit();
		}
		
		private bool Cull()
		{
			if(camera.TryGetCullingParameters(out ScriptableCullingParameters cullParameters))
			{
				cullingResults = context.Cull(ref cullParameters);
				return true;
			}
			else return false;
		}
		
		private void Setup()
		{
			context.SetupCameraProperties(camera);
			
			// Clear depth & colour
			commandBuffer.ClearRenderTarget(true, true, Color.clear);
			
			commandBuffer.BeginSample(commandBuffer.name);
			ExecuteCommandBuffer();
		}
		
		private void DrawVisibleGeometry()
		{
			// Set to draw opaque
			SortingSettings sortSettings = new SortingSettings(camera)
			{
				criteria = SortingCriteria.CommonOpaque
			};
			DrawingSettings   drawSettings   = new DrawingSettings(UnlitTagID, sortSettings);
			FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.opaque);
			
			// Draw opaque
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
			context.DrawSkybox(camera);
			
			// Set to draw transparent
			sortSettings.criteria = SortingCriteria.CommonTransparent;
			drawSettings.sortingSettings = sortSettings;
			filterSettings.renderQueueRange = RenderQueueRange.transparent;
			
			// Draw transparent
			context.DrawRenderers(cullingResults, ref drawSettings, ref filterSettings);
		}
		
		private void DrawUnsupportedShaders()
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
		
		private void Submit()
		{
			commandBuffer.EndSample(commandBuffer.name);
			ExecuteCommandBuffer();
			context.Submit();
		}
	}
}