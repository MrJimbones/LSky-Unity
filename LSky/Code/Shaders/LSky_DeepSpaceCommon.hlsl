#ifndef LSKY_DEEP_SPACE
#define LSKY_DEEP_SPACE

//#include "LSky_Common.hlsl"

// Galaxy Background.
uniform samplerCUBE lsky_GalaxyBackgroundCubemap;
half4 lsky_GalaxyBackgroundCubemap_HDR;

uniform half3 lsky_GalaxyBackgroundTint;
uniform half lsky_GalaxyBackgroundIntensity;
uniform half lsky_GalaxyBackgroundContrast;

// Stars Field
uniform samplerCUBE lsky_StarsFieldCubemap;
half4 lsky_StarsFieldCubemap_HDR;

uniform half3 lsky_StarsFieldTint;
uniform half lsky_StarsFieldIntensity;
uniform half lsky_StarsFieldScintillation;
uniform half lsky_StarsFieldScintillationSpeed;

uniform samplerCUBE lsky_StarsFieldNoiseCubemap;
uniform float4x4 lsky_StarsFieldNoiseMatrix;


//------------------------------------------- Const Def --------------------------------------------
//==================================================================================================


#define LSKY_STARS_FIELD_NOISE_COORDS(vertex) mul((float3x3) lsky_StarsFieldNoiseMatrix, vertex.xyz)


//------------------------------------------ Functions  --------------------------------------------
//==================================================================================================

inline half3 LSky_StarsFieldCubemap(float3 coords, float3 noiseCoords, half3 col)
{

    half3 res = half3(0.0, 0.0, 0.0);

    #ifdef LSKY_CUBE_HDR
        res.rgb = LSky_CUBE(lsky_StarsFieldCubemap, lsky_StarsFieldCubemap_HDR, 0, coords);
    #else
        res.rgb = LSky_CUBE(lsky_StarsFieldCubemap, 0, coords);
    #endif

    // Stars field scintillation.
    half noise = texCUBE(lsky_StarsFieldNoiseCubemap, noiseCoords).r;
    res.rgb = lerp(res.rgb, 2.0*res.rgb*noise, lsky_StarsFieldScintillation);

    // Intensity.
    res.rgb *= lsky_StarsFieldTint.rgb * col.rgb * lsky_StarsFieldIntensity;

    return res;
}

inline half3 LSky_GalaxyBackground(float3 coords, half3 col)
{
    
    half3 res = half3(0.0, 0.0, 0.0);

    #ifdef LSKY_CUBE_HDR
        res.rgb = LSky_CUBE(lsky_GalaxyBackgroundCubemap, lsky_GalaxyBackgroundCubemap_HDR, lsky_GalaxyBackgroundContrast, coords.xyz);
    #else
        res.rgb = LSky_CUBE(lsky_GalaxyBackgroundCubemap, lsky_GalaxyBackgroundContrast, coords.xyz);
    #endif

    res.rgb *= lsky_GalaxyBackgroundTint.rgb * col.rgb * lsky_GalaxyBackgroundIntensity;
  
    return res;
}

//-------------------------------------------- Helper  ---------------------------------------------
//==================================================================================================

struct appdata_deepspace
{
    float4 vertex : POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f_galaxy_background
{
    float4 vertex    : SV_POSITION;
    float3 texcoord  : TEXCOORD0;
    half3  col       : TEXCOORD2;
    UNITY_VERTEX_OUTPUT_STEREO
};

struct v2f_stars_field
{
    float4 vertex    : SV_POSITION;
    float3 texcoord  : TEXCOORD0;
    half3  col       : TEXCOORD2;
    float3 texcoord2 : TEXCOORD3;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f_galaxy_background galaxy_background_vert(appdata_deepspace v)
{
    v2f_galaxy_background o;
    UNITY_INITIALIZE_OUTPUT(v2f_galaxy_background, o);

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.vertex = LSky_DomeToClipPos(v.vertex);
    o.texcoord = v.vertex.xyz;

    o.col.rgb = LSKY_HORIZON_FADE(v.vertex) * LSKY_GLOBALEXPOSURE;

    return o;
}

v2f_stars_field stars_field_vert(appdata_deepspace v)
{
    v2f_stars_field o;
    UNITY_INITIALIZE_OUTPUT(v2f_stars_field, o);

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    o.vertex = LSky_DomeToClipPos(v.vertex);
    o.texcoord = v.vertex.xyz;
    o.texcoord2 = LSKY_STARS_FIELD_NOISE_COORDS(v.vertex);
    o.col.rgb = LSKY_HORIZON_FADE(v.vertex) * LSKY_GLOBALEXPOSURE;

    return o;
}

half4 galaxy_background_frag(v2f_galaxy_background i) : SV_Target
{
    half4 res = half4(0.0, 0.0, 0.0, 1.0);
    res = half4(LSky_GalaxyBackground(i.texcoord, i.col.rgb).rgb, 1.0);
    return res;
}

half4 stars_field_frag(v2f_stars_field i) : SV_Target
{
    half4 res = half4(0.0, 0.0, 0.0, 1.0);
    res = half4(LSky_StarsFieldCubemap(i.texcoord.xyz, i.texcoord2.xyz, i.col.rgb), 1.0);
    return res;
}

#endif // LSKY DEEP SPACE INCLUDED
