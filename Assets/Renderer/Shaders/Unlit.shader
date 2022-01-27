Shader "Mechxel/Unlit"
{
	Properties
	{
		// Texturing
		_BaseTexture("Texture", 2D) = "white" {}
		_BaseColour("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		
		// Transparency
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend Mode", Float) = 0
		
		// Write to Z-Buffer
		[Enum(Off, 0, On, 1)] _ZWrite("Write to Z-Buffer", Float) = 1
	}
	SubShader
	{
		Pass
		{
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			
			HLSLPROGRAM
			
			#pragma multi_compile_instancing
			
			#pragma vertex UnlitVertex
			#pragma fragment UnlitFragment
			
			#include "../Pipeline/Unlit.hlsl"
			
			ENDHLSL
		}
	}
}