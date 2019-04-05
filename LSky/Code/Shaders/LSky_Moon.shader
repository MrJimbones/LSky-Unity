Shader "Rallec/LSky/Near Space/Moon"
{

    Properties{}

    //CGINCLUDE
    //ENDCG

    SubShader
    {
        Tags{ "Queue"="Background+1555" "RenderType"="Background" "IgnoreProjector"= "true" }

        Pass
        {
            //Cull Back
            ZWrite Off
            ZTest Lequal
            //Blend One One
            Fog{ Mode Off }

            CGPROGRAM

            #include "UnityCG.cginc"
            #include "LSky_Common.hlsl"
 
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            uniform sampler2D lsky_MoonTex;
            float4 lsky_MoonTex_ST;
            uniform half lsky_MoonIntensity;
            uniform half3 lsky_MoonTint;
            uniform half lsky_MoonContrast;

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float3 normal   : TEXCOORD1;
                half3  col      : TEXCOORD2;
                float4 vertex   : SV_POSITION;
			    UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_base v)
            {

                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
	
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                // Position.
                o.vertex    = UnityObjectToClipPos(v.vertex);
                float3 Pos  = LSKY_WORLD_POS(v.vertex);

			    o.normal    = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
			    o.texcoord  = TRANSFORM_TEX(v.texcoord.xy, lsky_MoonTex);

                // Color.
			    o.col.rgb = lsky_MoonTint.rgb * saturate(max(0.0, dot(lsky_WorldSunDirection.xyz, o.normal)) * 2.0) * lsky_MoonIntensity;
                o.col.rgb *= LSKY_WORLD_HORIZON_FADE(Pos) * LSKY_GLOBALEXPOSURE;

			    return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(i.col.rgb * LSky_Pow3(tex2D(lsky_MoonTex, i.texcoord).rgb, lsky_MoonContrast), 1);
            }

            ENDCG
        }
    }

}