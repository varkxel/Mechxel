#ifndef MECHXEL_UNITY_INPUT_INCLUDED
#define MECHXEL_UNITY_INPUT_INCLUDED

// Include Core RP Library Common.hlsl if not already included.
#ifndef UNITY_COMMON_INCLUDED
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#endif

CBUFFER_START(UnityPerDraw)
//{
	// Object position to world position transform matrix.
	float4x4 unity_ObjectToWorld;
	
	// World position to Object position transform matrix.
	// Inverse of unity_ObjectToWorld.
	float4x4 unity_WorldToObject;
	
	float4 unity_LODFade;
	
	real4 unity_WorldTransformParams;
//}
CBUFFER_END

// Homogenous Clip Space (HClipSpace) transform matrix.
// Transforms world space into a square of everything in front of the camera,
// distorted into the trapezoid that the view frustum is.
float4x4 unity_MatrixVP;

float4x4 unity_MatrixV;

// The 3D projection matrix.
float4x4 glstate_matrix_projection;

#endif