Shader "Mechxel/Standard"
{
	Properties
	{
		// Texturing
		_DiffuseSpecular("Diffuse + Specular", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Tags
			{
				"LightMode" = "DeferredLit"
			}
			
			HLSLPROGRAM
			
			// Define shader functions to deferred pipeline
			#pragma vertex DeferredFillVertex
			#pragma fragment DeferredFillFragment
			
			// Include Deferred runtimes
			#include "../Pipeline/Deferred/GeometryPass.hlsl"
			
			ENDHLSL
		}
	}
}