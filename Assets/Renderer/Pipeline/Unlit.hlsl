#ifndef MECHXEL_UNLIT_PASS_INCLUDED
#define MECHXEL_UNLIT_PASS_INCLUDED

#include "../Library/Common.hlsl"

TEXTURE2D(_BaseTexture);
SAMPLER(sampler_BaseTexture);

UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseTexture_ST)
	UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColour)
UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

struct UnlitVertexInput
{
	float3 position_OS : POSITION;
	float2 uv : TEXCOORD0;
	
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct UnlitFragmentInput
{
	float4 position_HCS : SV_POSITION;
	float2 uv : VAR_BASE_UV;
	
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

UnlitFragmentInput UnlitVertex(UnlitVertexInput input)
{
	UnlitFragmentInput output;
	
	// Instancing
	UNITY_SETUP_INSTANCE_ID(input);
	UNITY_TRANSFER_INSTANCE_ID(input, output);
	
	// Transform
	float3 position_WS = TransformObjectToWorld(input.position_OS.xyz);
	float4 position_HCS = TransformWorldToHClip(position_WS);
	output.position_HCS = position_HCS;
	
	// Scale UV and pass to fragment
	float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseTexture_ST);
	output.uv = input.uv * baseST.xy + baseST.zw;
	
	return output;
}

float4 UnlitFragment(UnlitFragmentInput input) : SV_TARGET
{
	UNITY_SETUP_INSTANCE_ID(input);
	float4 tex = SAMPLE_TEXTURE2D(_BaseTexture, sampler_BaseTexture, input.uv);
	float4 colour = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColour);
	return tex * colour;
}

#endif