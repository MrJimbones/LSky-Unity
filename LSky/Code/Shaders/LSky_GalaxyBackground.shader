Shader "Rallec/LSky/Deep Space/Galaxy Background"
{

    Properties{}

    CGINCLUDE  

    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"
    #include "LSky_DeepSpaceCommon.hlsl"

    #define LSKY_CUBE_HDR 0

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

            #pragma vertex galaxy_background_vert
            #pragma fragment galaxy_background_frag
            #pragma target 2.0

            ENDCG
        }
    }

}