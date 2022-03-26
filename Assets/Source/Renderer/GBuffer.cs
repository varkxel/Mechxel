using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Unity.Mathematics;

namespace Mechxel.Renderer
{
	public struct GBuffer
	{
		public readonly string Name;
		public readonly int ID;
		public readonly RenderTargetIdentifier RT;
		
		public GraphicsFormat graphicsFormat;
		public DepthFormat depthFormat;
		
		public GBuffer
		(
			string name,
			GraphicsFormat format = GraphicsFormat.None,
			DepthFormat depthFormat = DepthFormat.None
		)
		{
			Name = name;
			ID = Shader.PropertyToID(name);
			RT = new RenderTargetIdentifier(ID);
			
			graphicsFormat = format;
			this.depthFormat = depthFormat;
		}
		
		public readonly RenderTextureDescriptor Descriptor(in int2 size)
		{
			RenderTextureDescriptor output = new RenderTextureDescriptor(size.x, size.y)
			{
				depthBufferBits = depthFormat.Bits(),

				enableRandomWrite = false,
				msaaSamples = 1
			};
			
			bool isDepthBuffer = 
				depthFormat != DepthFormat.None &&
				graphicsFormat == GraphicsFormat.None;
			
			if(isDepthBuffer)
			{
				output.colorFormat = RenderTextureFormat.Depth;
			}
			else
			{
				output.graphicsFormat = graphicsFormat;
				output.sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
			
			return output;
		}
		
		public static RenderTargetIdentifier[] GetRenderTargets(params GBuffer[] gBuffers)
		{
			int length = gBuffers.Length;
			
			RenderTargetIdentifier[] identifiers = new RenderTargetIdentifier[length];
			for(int i = 0; i < length; i++) identifiers[i] = gBuffers[i].RT;
			
			return identifiers;
		}
	}
}