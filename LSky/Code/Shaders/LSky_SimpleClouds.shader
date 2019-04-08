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

    // Speed.
    uniform half lsky_CloudsSpeed, lsky_CloudsSpeed2;

    v2f vert(appdata_base v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex   = LSky_DomeToClipPos(v.vertex);
        o.texcoord = TRANSFORM_TEX(v.texcoord, lsky_CloudsTex);
             
        o.col.rgb = lsky_CloudsTint.rgb * lsky_CloudsIntensity * LSKY_GLOBALEXPOSURE;
        o.col.a   = normalize(v.vertex.xyz-float3(0.0, 0.05, 0.0)).y*2;

        return o;
    }

    half4 frag2(v2f i) : SV_TARGET
    {
        half4 col   = half4(0.0, 0.0, 0.0, 1.0);
        half noise  = tex2D(lsky_CloudsTex, i.texcoord + _Time.x * lsky_CloudsSpeed).r;
        half noise2 = tex2D(lsky_CloudsTex, i.texcoord + _Time.x * lsky_CloudsSpeed2).r;

        half coverage = saturate(( (noise+noise2) * 0.5) - lsky_CloudsCoverage);

        col.rgb = i.col.rgb * (1.0 - coverage * lsky_CloudsTint.a);
        
        col.a = saturate(coverage * lsky_CloudsDensity * i.col.a);
        
        col.a += LSky_FastTonemaping(col.a, 1.0);
        col.a = saturate(col.a);
    
        return col;
    }


    ENDCG

    SubShader
    {
        
        Tags{ "Queue"="Geometry+1745" "RenderType"="Background" "IgnoreProjector"="true" }

        Pass
        {

            Cull Front
            ZWrite Off
            //ZTest Lequal
            //Blend One One
            Blend SrcAlpha OneMinusSrcAlpha
            Fog{ Mode Off }

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag2
            #pragma target 2.0

            ENDCG
        }

    }

}