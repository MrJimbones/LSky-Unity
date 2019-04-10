﻿Shader "Rallec/LSky/Post FX/Distance Fog"
{
    Properties{}

    CGINCLUDE

    #define LSKY_COMPUTE_MIE_PHASE 1
    #define LSKY_PER_PIXEL_ATMOSPHERE 1
    #define LSKY_ENABLE_POST_FX 1

    // Includes.
    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"
    #include "LSky_PreeHoffAtmosphericScatteringCommon.hlsl" 

    uniform sampler2D _MainTex;
    uniform sampler2D_float _CameraDepthTexture;
    float4 _MainTex_TexelSize;

    float4x4 lsky_FrustumCorners;
    float3 lsky_Camera;

    float lsky_DensityExp;


    inline float3 LSky_PPWorldPos(float3 viewDir)
    {
        return _WorldSpaceCameraPos - viewDir;
    }

    inline half4 LSky_PPSceneColor(sampler2D tex, float2 uv)
    {
        return tex2D(tex, UnityStereoTransformScreenSpaceTex(uv));
    }

    inline float LSky_PPSceneDepth(float2 uv)
    {
        float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv));
        return Linear01Depth(depth);
    }

    inline float LSky_FogDistance(float depth)
    {
        float dist = depth * _ProjectionParams.z;
        return dist - _ProjectionParams.y;
    }

    inline half LSky_FogExpFactor(float depth)
    {
        float res = LSky_FogDistance(depth);
        res = lsky_DensityExp * depth;
        return 1.0 - saturate(exp2(-res * res));
    }

    struct v2f
    {
        float2 uv             : TEXCOORD0;
        float2 uv_depth       : TEXCOORD1;
        float4 interpolatedRay : TEXCOORD2;
        float4 vertex         : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata_img v)
    {
        v2f o; 
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        v.vertex.z = 0.1;
        o.vertex   = UnityObjectToClipPos(v.vertex);
        o.uv       = v.texcoord.xy;
        o.uv_depth = v.texcoord.xy;

        #if UNITY_UV_STARTS_AT_TOP
            if(_MainTex_TexelSize.y < 0)
            o.uv.y = 1-o.uv.y;
        #endif

        int index = v.texcoord.x + (2.0 * o.uv.y);
        o.interpolatedRay = lsky_FrustumCorners[index];
        o.interpolatedRay.xyz = mul((float3x3)lsky_WorldToObject, o.interpolatedRay.xyz);
        o.interpolatedRay.w = index;	

        return o;
    }

    half3 AtmosphereColor(float3 viewDir, float depth)
    {
        half3 res = half3(0.0, 0.0, 0.0);
        float3 pos = normalize(viewDir.xyz);

        half3 SunMiePhase, MoonMiePhase;

        #ifdef LSKY_COMPUTE_MIE_PHASE
            res = LSky_ComputeAtmosphere(pos, SunMiePhase, MoonMiePhase, depth);
        #endif

        return res;
    }


    half4 frag(v2f i) : SV_Target
    {

        half4 col = LSky_PPSceneColor(_MainTex, i.uv);
        float depth = LSky_PPSceneDepth(i.uv_depth);

        float3 viewDir = (depth * i.interpolatedRay.xyz);
        float3 viewPos = _WorldSpaceCameraPos + viewDir;

        float Fog = 0.0;
        Fog = LSky_FogExpFactor(depth) * (depth < 0.9999);
        half4 scatter =  half4(AtmosphereColor(viewDir, Fog), 1.0);

        return lerp(half4(col.rgb,1.0), scatter, Fog);
    }

    ENDCG

    SubShader
    {

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag 
            #pragma target 3.0
            
            // Keywords
            #pragma multi_compile __ LSKY_APPLY_FAST_TONEMAPING
            #pragma multi_compile __ LSKY_ENABLE_MOON_RAYLEIGH
            
            ENDCG
        }
    }

}