Shader "Rallec/LSky/Skydome/Preetham And Hoffman Atmosphere"
{
    Properties{}

    CGINCLUDE

    // Includes.
    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"
    #include "LSky_PreeHoffAtmosphericScatteringCommon.hlsl" 

    struct appdata
    {
        float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float3 nvertex : TEXCOORD0; // pos
        #ifndef LSKY_PER_PIXEL_ATMOSPHERE
        half3 scatter: TEXCOORD1;
        #endif
        half3 sunMiePhase : TEXCOORD2;
        half4 moonMiePhase: TEXCOORD3;
        float4 vertex : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    
    v2f vert(appdata v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = LSky_DomeToClipPos(v.vertex);
        o.nvertex = normalize(v.vertex.xyz);

        #ifndef LSKY_PER_PIXEL_ATMOSPHERE
            o.scatter.rgb = LSky_ComputeAtmosphere(o.nvertex.xyz, o.sunMiePhase, o.moonMiePhase.rgb);
        #endif

        return o;
    }

    half4 frag(v2f i) : SV_Target
    {

        half4 col = half4(0.0, 0.0, 0.0, 1.0);

        #ifndef LSKY_PER_PIXEL_ATMOSPHERE
            col.rgb = i.scatter;
        #else
            i.nvertex.xyz = normalize(i.nvertex.xyz);
            col.rgb = LSky_ComputeAtmosphere(i.nvertex.xyz, i.sunMiePhase, i.moonMiePhase.rgb);
        #endif

        return col;
    }
    ENDCG

    SubShader
    {

        Tags{"Queue"="Background+1560" "RenderType"="Background" "IgnoreProjector"="True"}

        Pass
        {
            Cull Front 
            ZWrite Off
            ZTest LEqual
            Blend One One
            Fog{ Mode Off }
            //----------------------

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag 
            #pragma target 2.0
            
            // Keywords
            #pragma multi_compile __ LSKY_APPLY_FAST_TONEMAPING
            #pragma multi_compile __ LSKY_PER_PIXEL_ATMOSPHERE
            #pragma multi_compile __ LSKY_ENABLE_MOON_RAYLEIGH
            
            ENDCG
        }
    }

}