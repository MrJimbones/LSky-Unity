Shader "Rallec/LSky/Skybox/Preetham And Hoffman Ambient Skybox"
{
    Properties{}

    CGINCLUDE

    #define LSKY_SUNMIEPHASEDEPTHMULTIPLIER 1
    #define LSKY_RAYLEIGHDEPTHMULTIPLIER 1
    #define LSKY_ENABLE_GROUND 1
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
        half3 scatter: TEXCOORD1;
        float4 vertex : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    
    v2f vert(appdata v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = UnityObjectToClipPos(v.vertex);
        o.nvertex = normalize(v.vertex.xyz);

        fixed3 _one = fixed3(0.0, 0.0, 0.0);

        #ifndef LSKY_COMPUTE_MIE_PHASE
        o.scatter.rgb = LSky_ComputeAtmosphere(o.nvertex.xyz, 1.0, 1.0);
        #endif
        
        return o;
    }

    half4 frag(v2f i) : SV_Target
    {

        return half4(i.scatter.rgb, 1.0);
    }
    ENDCG

    SubShader
    {

        Tags{"Queue"="Background+10" "RenderType"="Background" "PreviewType"="Skybox"}

        Pass
        {
            
            ZWrite Off
            //ZTest LEqual
            //Blend One One
            Fog{ Mode Off }
            //----------------------

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag 
            #pragma target 2.0
            #define LSKY_COMPUTE_MIE_PHASE 0
            
            // Keywords
            #pragma multi_compile __ LSKY_APPLY_FAST_TONEMAPING
            #pragma multi_compile __ LSKY_PER_PIXEL_ATMOSPHERE
            #pragma multi_compile __ LSKY_ENABLE_MOON_RAYLEIGH
            
            ENDCG
        }
    }

}