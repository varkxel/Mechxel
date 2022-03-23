using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

namespace Mechxel.Renderer
{
	public struct Context
	{
		public ScriptableRenderContext SRPContext;
		public Camera camera;
		
		internal CullingResults culling;
		
		public int2 renderSize;
		
		public static readonly GBuffer DepthBuffer = new GBuffer
		(
			"DepthBuffer",
			GraphicsFormat.None, DepthFormat.Bits32
		);
		
		public static CommandBuffer buffer { get; private set; } = new CommandBuffer()
		{
			#if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
			name = "Render Camera"
			#endif
		};
		
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		private string cameraName;
		#endif
		
		internal void BufferInit()
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Profiler.BeginSample("Editor Only");
			
			cameraName = camera.name;
			buffer.name = $"Render {cameraName}";
			
			Profiler.EndSample();
			#endif
		}
		
		#if UNITY_EDITOR || DEVELOPMENT_BUILD
		private string sampleName;
		#endif
		
		internal void StartBuffer(string description)
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Profiler.BeginSample("Editor Only");
			
			sampleName = $"({cameraName}) {description}";
			buffer.BeginSample(sampleName);
			
			Profiler.EndSample();
			#endif
		}
		
		internal void EndBuffer()
		{
			#if UNITY_EDITOR || DEVELOPMENT_BUILD
			Profiler.BeginSample("Editor Only");
			
			buffer.EndSample(sampleName);
			sampleName = null;
			
			Profiler.EndSample();
			#endif
			
			SRPContext.ExecuteCommandBuffer(buffer);
			buffer.Clear();
		}
		
		internal void CameraInit() => SRPContext.SetupCameraProperties(camera);
		
		internal bool GetCullingResults()
		{
			// Get culling parameters
			if(!camera.TryGetCullingParameters(out ScriptableCullingParameters cullParameters))
			{
				Debug.LogWarning("Failed to get culling parameters.");
				return false;
			}
			
			// Get culling results
			culling = SRPContext.Cull(ref cullParameters);
			return true;
		}
	}
}