Shader "LSky/Deep Space/Galaxy Background"
{

    Properties{}

    CGINCLUDE  

    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"
    //-------------------------------------------------


    //-------------------------------------------------

    uniform samplerCUBE lsky_GalaxyBackgroundCubemap;
    half4 lsky_GalaxyBackgroundCubemap_HDR;
    //-------------------------------------------------

    uniform half3 lsky_GalaxyBackgroundTint;
    uniform half  lsky_GalaxyBackgroundIntensity;
    uniform half  lsky_GalaxyBackgroundContrast;
    //-------------------------------------------------

    struct v2f
    {
        float4 vertex    : SV_POSITION;
        float3 texcoord  : TEXCOORD0;
        half3  col       : TEXCOORD2;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f vert(appdata_base v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = LSky_DomeToClipPos(v.vertex);
        o.texcoord = v.vertex.xyz;
        o.col.rgb = LSKY_HORIZON_FADE(v.vertex) * LSKY_GLOBALEXPOSURE;

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {
        half4 res = half4(0.0, 0.0, 0.0, 1.0);

        #ifdef LSKY_CUBE_HDR
            res.rgb = LSky_CUBE(lsky_GalaxyBackgroundCubemap, lsky_GalaxyBackgroundCubemap_HDR, lsky_GalaxyBackgroundContrast, i.texcoord.xyz);
        #else
            res.rgb = LSky_CUBE(lsky_GalaxyBackgroundCubemap, lsky_GalaxyBackgroundContrast, i.texcoord.xyz);
        #endif

        res.rgb *= lsky_GalaxyBackgroundTint.rgb * i.col.rgb * lsky_GalaxyBackgroundIntensity;

        return res;
    }

    ENDCG

    SubShader
    {
        Tags{ "Queue"="Background+5" "RenderType"="Background" "IgnoreProjector"= "true" }
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