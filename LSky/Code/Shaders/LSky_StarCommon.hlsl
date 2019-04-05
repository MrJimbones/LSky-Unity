#ifndef LSKY_STAR_COMMON
#define LSKY_STAR_COMMON

//--------------------------------------------- Params ---------------------------------------------
//==================================================================================================

// Texture.
uniform sampler2D lsky_StarTex;

// Color.
uniform half4 lsky_StarTint;
uniform half lsky_StarIntensity;


//------------------------------------------- Functions --------------------------------------------
//==================================================================================================

inline half Disc(float2 coords, half size)
{
    half dist = length(coords);
    return 1.0 - step(size, dist);
}

//--------------------------------------------- Shader ---------------------------------------------
//==================================================================================================
 struct appdata_star
{
    float4 vertex : POSITION;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f_star
{
    float2 texcoord : TEXCOORD0;
    float3 worldPos : TEXCOORD1;
    float4 vertex   : TEXCOORD2;
    half3 col       : TEXCOORD3;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f_star starvert(appdata_star v)
{
    v2f_star o;
    UNITY_INITIALIZE_OUTPUT(v2f_star, o);

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.vertex = UnityObjectToClipPos(v.vertex);

    o.worldPos = LSKY_WORLD_POS(v.vertex);

    #ifdef LSKY_PROCEDURAL_SPOT
        o.texcoord = v.texcoord - 0.5;
    #else
        o.texcoord = v.texcoord;
    #endif

    o.col.rgb = lsky_StarTint.rgb * lsky_StarIntensity * LSKY_HORIZON_FADE(o.worldPos) * LSKY_GLOBALEXPOSURE;

    return o;
}

half4 starfrag(v2f_star i) : SV_TARGET
{
    half4 col = half4(1.0, 1.0, 1.0, 1.0);

    #ifdef LSKY_PROCEDURAL_SPOT
        col.rgb = Disc(i.texcoord, lsky_StarTint.a).xxx;
    #else
        col.rgb = tex2D(lsky_StarTex, i.texcoord).rgb;
    #endif

    col.rgb *= i.col.rgb;

    return col;
}


#endif // LSKY: STAR INCLUDED
