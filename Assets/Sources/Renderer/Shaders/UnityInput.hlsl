#ifndef MECHXEL_UNITY_INPUT_INCLUDED
#define MECHXEL_UNITY_INPUT_INCLUDED

// Include Core RP Library Common.hlsl if not already included.
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

CBUFFER_START(UnityPerDraw)
	// Object position to world position transform matrix.
	float4x4 unity_ObjectToWorld;
	
	// World position to Object position transform matrix.
	// Inverse of unity_ObjectToWorld.
	float4x4 unity_WorldToObject;
	
	// X is the fade value (0 to 1),
	// Y is x quantized into 16 levels.
	float4 unity_LODFade;
	
	// W is 1.0, or -1.0 for odd-negative scale transforms.
	real4 unity_WorldTransformParams;
CBUFFER_END

// Homogenous Clip Space (HClipSpace) transform matrix.
// Transforms world space into a square of everything in front of the camera,
// distorted into the trapezoid that the view frustum is.
float4x4 unity_MatrixVP;

float4x4 unity_MatrixV;

// The 3D projection matrix.
float4x4 glstate_matrix_projection;

#endif