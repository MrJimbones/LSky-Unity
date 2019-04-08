Shader "Rallec/LSky/Near Space/Sun"
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
        float3 worldPos : TEXCOORD1;
        float4 vertex   : SV_POSITION;
        half3 col       : TEXCOORD3;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    // Texture.
    uniform sampler2D lsky_StarTex;

    // Color.
    uniform half4 lsky_StarTint;
    uniform half lsky_StarIntensity;

    v2f vert(appdata v)
    {
        v2f o;
        UNITY_INITIALIZE_OUTPUT(v2f, o);

        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = UnityObjectToClipPos(v.vertex);
        o.worldPos = LSKY_WORLD_POS(v.vertex);
        o.texcoord = v.texcoord;
             
        o.col.rgb = lsky_StarTint.rgb * lsky_StarIntensity * LSKY_GLOBALEXPOSURE;

        return o;
    }

    half4 frag(v2f i) : SV_TARGET
    {
        half4 col = half4(0.0, 0.0, 0.0, 1.0);

        col.rgb = tex2D(lsky_StarTex, i.texcoord).rgb;
        col.rgb *= i.col.rgb * LSKY_WORLD_HORIZON_FADE(i.worldPos);

        return col;
    }

    ENDCG

    SubShader
    {
        
        Tags{ "Queue"="Background+1545" "RenderType"="Background" "IgnoreProjector"="true" }

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

            ENDCG
        }
    }

}