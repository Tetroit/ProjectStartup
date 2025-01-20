#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

//#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
//#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, out float Attenuation, out float ShadowAttenuation)
{
#ifdef SHADERGRAPH_PREVIEW
     Direction = normalize(float3(1.0f, 1.0f, 0.0f));
     Color = 1.0f;
     Attenuation = 1.0f;
     ShadowAttenuation = 1.0f;
#else  
#if SHADOWS_SCREEN
    float4 shadowPos = ComputeScreenPos(TransformWorldToHClip(WorldPos));
#else
    float4 shadowPos = TransformWorldToShadowCoord(WorldPos);
#endif
    Light mainLight = GetMainLight(shadowPos, WorldPos, 1);
    Direction = mainLight.direction;
    Color = mainLight.color;
    Attenuation = mainLight.distanceAttenuation;
    ShadowSamplingData shadowSampler = GetMainLightShadowSamplingData();
    ShadowAttenuation = MainLightRealtimeShadow(shadowPos);
#endif
}

void ExtraLight_float(float3 WorldPos, float3 WorldNormal, out float3 Color)
{
#ifdef SHADERGRAPH_PREVIEW
     Color = 0.0f;
#else
    Color = float3(0,0,0);
    int lightCount = GetAdditionalLightsCount();
    for (int i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i, WorldPos);
        float3 color = dot(light.direction, WorldNormal) * light.distanceAttenuation;
        color = clamp(color, 0, 1);
        color *= light.color;
        Color += color;
    }
#endif
}
#endif