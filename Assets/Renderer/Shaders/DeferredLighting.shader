Shader "Hidden/Mechxel/DeferredLighting"
{
	Properties {}
	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always
		
		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Common.hlsl"
			
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};
			
			CBUFFER_START(UnityPerMaterial);
				sampler2D GBuffer0;
				sampler2D GBuffer1;
			CBUFFER_END;
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			half4 frag(v2f i) : SV_Target
			{
				return 0;
			}
			ENDHLSL
		}
	}
}