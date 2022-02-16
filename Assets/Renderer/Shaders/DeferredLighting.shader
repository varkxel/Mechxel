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
			
			// | Diffuse R | Diffuse G | Diffuse B |  Emissive  |
			TEXTURE2D(GBuffer0);
			SAMPLER(GBuffer0_sampler);
			
			// | Normal X  | Normal Y |  Specular  | Smoothness |
			TEXTURE2D(GBuffer1);
			SAMPLER(GBuffer1_sampler);
			
			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = TransformObjectToHClip(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			float4 frag(v2f i) : SV_Target
			{
				float4 gbuffer0 = SAMPLE_TEXTURE2D(GBuffer0, GBuffer0_sampler, i.uv);
				float4 gbuffer1 = SAMPLE_TEXTURE2D(GBuffer1, GBuffer1_sampler, i.uv);
				
				return float4(gbuffer0.rgb, 1);
			}
			ENDHLSL
		}
	}
}