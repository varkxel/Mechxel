Shader "Mechxel/Unlit"
{
	Properties
	{
		// Texturing
		_BaseTexture("Texture", 2D) = "white" {}
		_BaseColour("Colour", Color) = (1.0, 1.0, 1.0, 1.0)
		
		// Alpha Clip
		[Toggle(_CLIPPING)] _Clipping("Alpha Clip", Float) = 0
		_Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
		
		// Transparency
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("Source Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("Destination Blend Mode", Float) = 10
		
		// Write to Z-Buffer
		[Enum(Off, 0, On, 1)] _ZWrite("Write to Z-Buffer", Float) = 1
	}
	SubShader
	{
		Pass
		{
			Blend  [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			
			HLSLPROGRAM
			
			// Shader variants
			#pragma shader_feature _CLIPPING
			
			// Instancing support
			#pragma multi_compile_instancing
			
			// Define shader functions
			#pragma vertex UnlitVertex
			#pragma fragment UnlitFragment
			
			// Include Unlit runtime
			#include "../Pipeline/Unlit.hlsl"
			
			ENDHLSL
		}
	}
}