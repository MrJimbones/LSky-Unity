Shader "Rallec/LSky/Deep Space/Stars Field"
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
        Tags{ "Queue"="Background+10" "RenderType"="Background" "IgnoreProjector"= "true" }

        Pass
        {
            Cull Front
            ZWrite Off
            ZTest Lequal
            Blend One One
            Fog{ Mode Off }

            CGPROGRAM

            #pragma vertex stars_field_vert
            #pragma fragment stars_field_frag
            #pragma target 2.0

            ENDCG
        }
    }

}