Shader "Mechxel/DeferredLit"
{
	Properties
	{
		// RGB = Albedo, A = Emissivity
		[MainTexture]
		_Albedo("Albedo", 2D) = "white" {}
		
		[MainColor]
		_Colour("Colour", Color) = (1, 1, 1, 1)
		
		[NoScaleOffset] [Normal]
		_Normal("Normal", 2D) = "bump" {}
		
		// | Metalllic | Roughness | Emissive | /// | 
		[NoScaleOffset]
		_Mask("Mask", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Tags { "LightMode" = "MechxelDeferred" }
			
			HLSLPROGRAM
			
			#pragma vertex Geometry_Vertex
			#pragma fragment Geometry_Fragment
			
			#include "Common.hlsl"
			
			TEXTURE2D(_Albedo);
			TEXTURE2D(_Normal);
			TEXTURE2D(_Mask);
			
			SAMPLER(sampler_Albedo);
			SAMPLER(sampler_Normal);
			SAMPLER(sampler_Mask);
			
			CBUFFER_START(UnityPerMaterial);
				float4 _Albedo_ST;
				float4 _Colour;
			CBUFFER_END;
			
			struct Geometry_VertexInfo
			{
				float3 position_OS : POSITION;
				float3 normal_OS   : NORMAL;
				float2 uv          : TEXCOORD0;
			};
			
			struct Geometry_FragmentInfo
			{
				float4 position_HCS : SV_POSITION;
				float3 position_WS  : TEXCOORD0;
				
				float2 uv           : VAR_BASE_UV;
				float3 normal_WS    : VAR_NORMAL;
			};
			
			struct Geometry_GBuffers
			{
				// | Diffuse R | Diffuse G | Diffuse B | Roughness |
				float4 GBuffer0 : SV_Target0;
				// |  Normal X |  Normal Y | Normal Z  | Metallic  |
				float4 GBuffer1 : SV_Target1;
				
				// Reserved
				// | Motion Vector X | Motion Vector Y |    ///    |     ///    |
				// float4 GBuffer2;
			};
			
			Geometry_FragmentInfo Geometry_Vertex(Geometry_VertexInfo info)
			{
				Geometry_FragmentInfo output;
				
				float3 position_WS = TransformObjectToWorld(info.normal_OS);
				float4 position_HCS = TransformWorldToHClip(position_WS);
				
				output.position_WS = position_WS;
				output.position_HCS = position_HCS;
				
				// Normals
				output.normal_WS = TransformObjectToWorldNormal(info.normal_OS);
				
				// Scale UV and pass to fragment
				float4 albedoST = _Albedo_ST;
				output.uv = info.uv * albedoST.xy + albedoST.zw;
				
				return output;
			}
			
			Geometry_GBuffers Geometry_Fragment(Geometry_FragmentInfo info)
			{
				Geometry_GBuffers gbuffers;
				
				float4 albedoMap = SAMPLE_TEXTURE2D(_Albedo, sampler_Albedo, info.uv);
				float4 normalMap = SAMPLE_TEXTURE2D(_Normal, sampler_Normal, info.uv);
				float4 maskMap   = SAMPLE_TEXTURE2D(_Mask,   sampler_Mask,   info.uv);
				
				float3 albedo = albedoMap.rgb * _Colour;
				float3 normal = normalMap.rgb;
				
				float emissive = maskMap.r;
				float metallic = maskMap.g;
				float roughness = maskMap.b;
				
				albedo *= emissive;
				
				gbuffers.GBuffer0 = float4(albedo, roughness);
				gbuffers.GBuffer1 = float4(normal, metallic);
				
				return gbuffers;
			}
			
			ENDHLSL
		}
	}
}