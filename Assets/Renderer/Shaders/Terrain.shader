Shader "Mechxel/Terrain"
{
	Properties
	{
		[MainTexture] [NoScaleOffset]
		_ID("ID Map", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "MechxelTerrain" }
			
			HLSLPROGRAM
			
			/*
				TODO
				Use 16bit integer/floating point types,
				once Unity finally gets off their arses and implements DX12/VK properly.
			*/
//			#pragma use_dxc
//			#pragma target 6.2
			
			#pragma vertex Vertex
			#pragma fragment Fragment
			
			#include "Common.hlsl"
			
			TEXTURE2D(_ID);
			SAMPLER(sampler_ID);
			
			struct VertexInfo
			{
				float3 position_OS : POSITION;
				float3 normal_OS   : NORMAL;
				float2 uv          : TEXCOORD0;
			};
			
			struct FragmentInfo
			{
				float4 position_HCS : SV_POSITION;
				
				float2 uv        : VAR_BASE_UV;
				float3 normal_WS : VAR_NORMAL;
			};
			
			struct GBuffers
			{
				// |    ID    | Normal X | Normal Y | Normal Z |
				min16uint4 GBuffer0 : SV_Target0;
			};
			
			FragmentInfo Vertex(VertexInfo info)
			{
				FragmentInfo output;
				
				float3 position_WS = TransformObjectToWorld(info.position_OS);
				float4 position_HCS = TransformWorldToHClip(position_WS);
				
				output.position_HCS = position_HCS;
				
				// Normals
				output.normal_WS = TransformObjectToWorldNormal(info.normal_OS);
				
				// UV
				output.uv = info.uv;
				
				return output;
			}
			
			GBuffers Fragment(FragmentInfo info)
			{
				GBuffers buffers;
				
				uint id = SAMPLE_TEXTURE2D(_ID, sampler_ID, info.uv).x;
				half3 normal = info.normal_WS;
				uint3 normalInt = asuint(normal);
				
				// Pack ID & Normals into GBuffer0
				buffers.GBuffer0 = min16uint4(id, normalInt);
				
				return buffers;
			}
			
			ENDHLSL
		}
	}
}