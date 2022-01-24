Shader "Mechxel/Unlit"
{
	Properties
	{
		
	}
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#pragma vertex UnlitVertex
			#pragma fragment UnlitFragment
			#include "Unlit.hlsl"

			
			
			ENDHLSL
		}
	}
}