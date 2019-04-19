Shader "LSky/Deep Space/Stars Field"
{

    //Properties{}
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"

    uniform samplerCUBE lsky_StarsFieldCubemap;
    half4 lsky_StarsFieldCubemap_HDR;

    uniform half3 lsky_StarsFieldTint;
    uniform half  lsky_StarsFieldIntensity;
    uniform half  lsky_StarsFieldScintillation;
    uniform half  lsky_StarsFieldScintillationSpeed;

    uniform samplerCUBE lsky_StarsFieldNoiseCubemap;
    uniform float4x4 lsky_StarsFieldNoiseMatrix;

    #define LSKY_STARS_FIELD_NOISE_COORDS(vertex) mul((float3x3) lsky_StarsFieldNoiseMatrix, vertex.xyz)

    struct v2f
    {
        float4 vertex    : SV_POSITION;
        float3 texcoord  : TEXCOORD0;
        half3  col       : TEXCOORD2;
        float3 texcoord2 : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata_base v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex    = LSky_DomeToClipPos(v.vertex);
        o.texcoord  = v.vertex.xyz;
        o.texcoord2 = LSKY_STARS_FIELD_NOISE_COORDS(v.vertex);
        o.col.rgb   = LSKY_HORIZON_FADE(v.vertex) * LSKY_GLOBALEXPOSURE;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        half4 res = half4(0.0, 0.0, 0.0, 1.0);

        #ifdef LSKY_CUBE_HDR
            res.rgb = LSky_CUBE(lsky_StarsFieldCubemap, lsky_StarsFieldCubemap_HDR, 0, i.texcoord);
        #else
            res.rgb = LSky_CUBE(lsky_StarsFieldCubemap, 0, i.texcoord);
        #endif

        // Stars field scintillation.
        half noise = texCUBE(lsky_StarsFieldNoiseCubemap, i.texcoord2).r;
        res.rgb = lerp(res.rgb, 2.0*res.rgb*noise, lsky_StarsFieldScintillation);

        // Intensity.
        res.rgb *= lsky_StarsFieldTint.rgb * i.col.rgb * lsky_StarsFieldIntensity;

        return res;
    }

    ENDCG

    SubShader
    {
        Tags{ "Queue"="Background+10" "RenderType"="Background" "IgnoreProjector"= "true" }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest Lequal
            Blend One One
            Fog{ Mode Off }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #define LSKY_CUBE_HDR 0

            ENDCG
        }
    }

}