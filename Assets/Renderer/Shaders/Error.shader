/*
	This shader will run whenever there is a shader error.
	The output is a pink background with a black-blue cross.
	
	This probably isn't the fastest it could be, but this shouldn't even be running in a proper build.
 */

Shader "Mechxel/Internal/Error"
{
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex Vertex
			#pragma fragment Fragment
			#pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
			
			#include "UnityCG.cginc"
			
			struct VertexInfo
			{
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct FragmentInfo
			{
				float4 vertex : SV_POSITION;
				half2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			FragmentInfo Vertex(VertexInfo info)
			{
				FragmentInfo fragment;
				UNITY_SETUP_INSTANCE_ID(info);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(fragment);
				fragment.vertex = UnityObjectToClipPos(info.vertex);
				fragment.uv = info.uv;
				return fragment;
			}
			
			fixed4 Fragment(FragmentInfo info) : SV_Target
			{
				const half CrossSize = 0.1;
				const half CheckerboardSize = 0.125;
				
				const fixed4 Colour_BG = fixed4(1, 0, 1, 1);
				const fixed4 Colour_Checkerboard = fixed4(0, 0, 0, 1);
				const fixed4 Colour_Cross = fixed4(0, 1, 1, 1);
				
				half2 pos = half2
				(
					abs(info.uv.y - 0.5),
					abs(info.uv.x - 0.5)
				);

				bool background =
					pos.x + (0.5 - pos.y) < (0.5 - CrossSize) ||
					(0.5 - pos.x) + pos.y < (0.5 - CrossSize);
				if(!background) return Colour_Cross;

				bool2 checkerboard = info.uv % CheckerboardSize > (CheckerboardSize * 0.5);
				bool isCheckerboard = checkerboard.x ^ checkerboard.y;
				
				if(isCheckerboard) return Colour_Checkerboard;
				else return Colour_BG;
			}
			
			ENDHLSL
		}
	}
	Fallback Off
}