#ifndef MECHXEL_DEFERRED_INCLUDED
#define MECHXEL_DEFERRED_INCLUDED

struct DeferredGBuffers
{
	// |    Diffuse R    |    Diffuse G    |      Diffuse B     |  Specular  |
	float4 GBuffer_0;
	// |     Normal X    |     Normal Y    | Reflectivity (SSR) | Smoothness |
	float4 GBuffer_1;
	
	// Reserved
	// | Motion Vector X | Motion Vector Y |         ///        |     ///    |
	// float4 GBuffer_2;
};

#endif