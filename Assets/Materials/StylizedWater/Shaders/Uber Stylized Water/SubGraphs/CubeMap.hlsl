#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

// Material Keywords
#pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
#pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
#pragma multi_compile _ _FORWARD_PLUS


void GetCubemap_float(float3 ViewDirWS, float3 PositionWS, float3 NormalWS, float Roughness, out float3 Cubemap)
{
    #ifdef SHADERGRAPH_PREVIEW
    Cubemap = 0;
    #else

    half3 reflectionVector = reflect(-ViewDirWS, NormalWS);
    
    // Compute screen space UV for Screen Space Reflections (SSR)
    float4 clipPos = TransformWorldToHClip(PositionWS);
    float4 screenPos = ComputeScreenPos(clipPos);
    float2 screenUV = screenPos.xy / screenPos.w;

    Cubemap = GlossyEnvironmentReflection(reflectionVector, PositionWS, Roughness, 1.0, screenUV);

    #endif
}