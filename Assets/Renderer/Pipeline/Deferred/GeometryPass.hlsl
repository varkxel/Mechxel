#ifndef MECHXEL_DEFERRED_GEOMETRY_PASS_INCLUDED
#define MECHXEL_DEFERRED_GEOMETRY_PASS_INCLUDED

#include "../Library/Common.hlsl"

TEXTURE2D(_DiffuseSpecular);
SAMPLER(sampler_DiffuseSpecular);

CBUFFER_START(UnityPerMaterial);
    float4 _DiffuseSpecular_ST;
CBUFFER_END

struct DeferredLitVertexInfo
{
    float3 position_OS : POSITION;
    float3 normal_OS   : NORMAL;
    float2 uv          : TEXCOORD0;
	
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct DeferredLitFragmentInfo
{
    float4 position_HCS : SV_POSITION;
    float3 position_WS : TEXCOORD0;
    
    float2 uv : VAR_BASE_UV;
    float3 normal_WS : VAR_NORMAL;
    
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

#include "../Library/GBuffers.hlsl"

DeferredLitFragmentInfo DeferredFillVertex(DeferredLitVertexInfo input)
{
    DeferredLitFragmentInfo output;
	
    // Instancing
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    
    // Transform
    float3 position_WS = TransformObjectToWorld(input.position_OS.xyz);
    float4 position_HCS = TransformWorldToHClip(position_WS);
    
    output.position_WS = position_WS;
    output.position_HCS = position_HCS;
    
    // Normals
    output.normal_WS = TransformObjectToWorldNormal(input.normal_OS);
    
    // Scale UV and pass to fragment
    float4 baseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _DiffuseSpecular_ST);
    output.uv = input.uv * baseST.xy + baseST.zw;
    
    return output;
}

DeferredLitGBuffers DeferredFillFragment(DeferredLitFragmentInfo input)
{
    UNITY_SETUP_INSTANCE_ID(input);
    
    DeferredLitGBuffers buffers;
    
    buffers.albedoSpecular = SAMPLE_TEXTURE2D(_DiffuseSpecular, sampler_DiffuseSpecular, input.uv);
    buffers.positions_WS = input.position_WS;
    buffers.normals_WS = input.normal_WS;
    
    return buffers;
}

#endif