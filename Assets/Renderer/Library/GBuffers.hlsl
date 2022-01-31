#ifndef MECHXEL_GBUFFERS_INCLUDED
#define MECHXEL_GBUFFERS_INCLUDED

struct DeferredLitGBuffers
{
    float4 albedoSpecular : SV_Target0;
    float3 positions_WS   : SV_Target1;
    float3 normals_WS     : SV_Target2;
};

#endif