Shader "Rallec/LSky/Simple Clouds"
{

    //Properties{}
    CGINCLUDE

    #include "UnityCG.cginc"
    #include "LSky_Common.hlsl"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f
    {
        float2 texcoord : TEXCOORD0;
        half4  col      : TEXCOORD3;
        float4 vertex   : SV_POSITION;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    // Texture.
    uniform sampler2D lsky_CloudsTex;
    float4 lsky_CloudsTex_ST;

    // Color.
    uniform half4 lsky_CloudsTint;
    uniform half lsky_CloudsIntensity;
    
    // Density.
    uniform half lsky_CloudsDensity;
    uniform half lsky_CloudsCoverage;

    v2f vert(appdata_base v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex   = LSky_DomeToClipPos(v.vertex);
       
        o.texcoord = TRANSFORM_TEX(v.texcoord, lsky_CloudsTex);
             
        o.col.rgb = lsky_CloudsTint.rgb * lsky_CloudsIntensity * LSKY_GLOBALEXPOSURE;
    
        o.col.a  = normalize(v.vertex.xyz - float3(0.0, 0.01e-3, 0.0)).y*2;

        return o;
    }

    half4 frag(v2f i) : SV_TARGET
    {
        half4 col = half4(0.0, 0.0, 0.0, 1.0);

        half noise = tex2D(lsky_CloudsTex, i.texcoord).r;
        half2 coverage = saturate(noise-lsky_CloudsCoverage);

        // Get clouds color.
        col.rgb = i.col.rgb * (1.0 - coverage.r * 0.5);

        // Get alpha.
        col.a = saturate(coverage.g * lsky_CloudsDensity * i.col.a);

        col.a += 1.0 - exp(-col.a);

        return saturate(col);
    }

    // For panorama texture(WIP)
    half4 frag2(v2f i) : SV_TARGET
    {
        half4 col = half4(0.0, 0.0, 0.0, 1.0);

        half2 tex = tex2D(lsky_CloudsTex, i.texcoord).rg;

        half coverage = LSky_Pow2(tex.g, lsky_CloudsCoverage);

        // Get clouds color.
        col.rgb = i.col.rgb * tex.r;

        // Get alpha.
        col.a = saturate(coverage * lsky_CloudsDensity * i.col.a);

        //col.a += 1.0 - exp(-col.a);
        return saturate(col);
    }

    ENDCG

    SubShader
    {
        
        Tags{ "Queue"="Background+1745" "RenderType"="Background" "IgnoreProjector"="true" }

        Pass
        {

            Cull Front
            ZWrite Off
            ZTest Lequal
            //Blend One One
            Blend SrcAlpha OneMinusSrcAlpha
            Fog{ Mode Off }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            ENDCG
        }

    }

}