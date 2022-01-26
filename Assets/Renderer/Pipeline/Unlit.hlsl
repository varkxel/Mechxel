#ifndef MECHXEL_UNLIT_PASS_INCLUDED
#define MECHXEL_UNLIT_PASS_INCLUDED

#include "../Library/Common.hlsl"

CBUFFER_START(UnityPerMaterial)
//{
	float4 _BaseColour;
//}
CBUFFER_END

float4 UnlitVertex(float3 position_OS : POSITION) : SV_POSITION
{
	float3 position_WS = TransformObjectToWorld(position_OS.xyz);
	float4 position_HCS = TransformWorldToHClip(position_WS);
	
	return position_HCS;
}

float4 UnlitFragment(float4 position_HCS : SV_POSITION) : SV_TARGET
{
	return _BaseColour;
}

#endif