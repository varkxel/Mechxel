/*
	This shader will run whenever there is a shader error.
	The output is a black-pink background with a blue cross.
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
			
			bool Cross(half2 uv, half crossSize)
			{
				half2 pos = half2
				(
					abs(uv.y - 0.5),
					abs(uv.x - 0.5)
				);
				
				bool cross =
					pos.x + (0.5 - pos.y) < (0.5 - crossSize) ||
					(0.5 - pos.x) + pos.y < (0.5 - crossSize);
				return !cross;
			}
			
			bool Border(half2 uv, half borderSize)
			{
				half2 mirrorUV = 1.0 - (abs(uv - 0.5) * 2.0);
				bool2 border = mirrorUV < borderSize;
				return border.x || border.y;
			}
			
			half4 Fragment(FragmentInfo info) : SV_Target
			{
				const half BorderSize = 0.1;
				const half CrossSize = 0.1;
				const half CheckerboardSize = 0.125;
				
				const fixed4 Colour_BG = fixed4(1, 0, 1, 1);
				const fixed4 Colour_Checkerboard = fixed4(0, 0, 0, 1);
				const fixed4 Colour_Cross = fixed4(0, 1, 1, 1);
				
				if(Cross(info.uv, CrossSize) || Border(info.uv, BorderSize)) return Colour_Cross;
				
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