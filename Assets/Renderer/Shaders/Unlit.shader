Shader "Mechxel/Unlit"
{
	Properties
	{
		_BaseColour("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex UnlitVertex
			#pragma fragment UnlitFragment
			
			#include "../Pipeline/Unlit.hlsl"
			
			ENDHLSL
		}
	}
}