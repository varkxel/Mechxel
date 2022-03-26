Shader "Hidden/Mechxel/TerrainBlit"
{
	Properties {}
	SubShader
	{
		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex Vertex
			#pragma fragment Fragment
			
			#include "Common.hlsl"
			
			struct VertexInfo
			{
				float3 position_OS : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct FragmentInfo
			{
				float4 position_HCS : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			TEXTURE2D(GBuffer0);
			SAMPLER(sampler_GBuffer0);
			
			FragmentInfo Vertex(VertexInfo info)
			{
				FragmentInfo output;
				output.position_HCS = TransformObjectToHClip(info.position_OS);
				output.uv = info.uv;
				return output;
			}
			
			half4 Fragment(FragmentInfo info) : SV_Target
			{
				float4 gbuffer0 = SAMPLE_TEXTURE2D(GBuffer0, sampler_GBuffer0, info.uv);
				return abs(gbuffer0.yzwx);
			}
			
			ENDHLSL
		}
	}
}