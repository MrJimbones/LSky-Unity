#ifndef LSKY_COMMON
#define LSKY_COMMON

//------------------------------------------- Const Def --------------------------------------------
//==================================================================================================

// PI
//-------------------------------------------------
#define LSKY_TAU 6.283185307f       // PI*2
#define LSKY_INVTAU 0.159154943f    // 1/(PI*2)
#define LSKY_HALFPI 1.570796326f    // PI/2
#define LSKY_INVHALFPI 0.636619772f // 1/(PI/2)
#define LSKY_PI4 12.56637061f       // PI*4
#define LSKY_INVPI4 0.079577472f    // 1/(PI*4)
#define LSKY_3PIE 0.119366207f      // 3/(PI*8)
#define LSKY_3PI16 0.059683104f     // 3/(PI*16)

// Horizon Fade
#define LSKY_HORIZON_FADE(vertex) saturate(2 * normalize(mul((float3x3)unity_ObjectToWorld, vertex.xyz)).y)
#define LSKY_WORLD_HORIZON_FADE(pos) saturate(normalize(pos).y)


//-------------------------------------------- Position --------------------------------------------
//==================================================================================================

// 4x4 Matrices.
//-------------------------------------
uniform float4x4 lsky_WorldToObject;
uniform float4x4 lsky_ObjectToWorld;

// Celestials Directions.
//-------------------------------------
uniform float3 lsky_LocalSunDirection;
uniform float3 lsky_LocalMoonDirection;
uniform float3 lsky_WorldSunDirection;
uniform float3 lsky_WorldMoonDirection;

// Dome Position
//----------------------------------------------------
inline float4 LSky_DomeToClipPos(in float3 position)
{
    float4 pos = UnityObjectToClipPos(position);

    #ifdef UNITY_REVERSED_Z
        pos.z = 1e-5f;
    #else
        pos.z = pos.w - 1e-5f;
    #endif

    return pos;
}

// WorldPos.
#define LSKY_WORLD_POS(vertex) mul(lsky_WorldToObject, mul(unity_ObjectToWorld, vertex)).xyz;

//---------------------------------------- Color Correction ----------------------------------------
//==================================================================================================

// Color Space
//----------------------------
#define LSKY_LINEAR_TO_GAMMA(color) pow(color, 0.45454545f)
#define LSKY_LINEAR_TO_GAMMA_SIMPLE(color) sqrt(color)

// HDR
//-------------------------------------------------------------------------
uniform half lsky_GlobalExposure; 

#ifndef LSKY_GLOBALEXPOSURE
    #define LSKY_GLOBALEXPOSURE lsky_GlobalExposure
#endif

// Simple and fast tonemaping.

inline half LSky_FastTonemaping(half x, half exposure)
{
    return 1.0 - exp(exposure * - x);
}

inline half3 LSky_FastTonemaping(half3 col, half exposure)
{
    return 1.0 - exp(exposure * -col.rgb);
}

inline half4 LSky_FastTonemaping(half4 col, half exposure)
{
    return 1.0 - exp(exposure * -col);
}

// Exponent.
//--------------------------------------------------------------------------
inline half LSky_Pow2(half x, in half fade)
{ 
    return lerp(x, x*x, fade);
}

inline half3 LSky_Pow2(half3 x, in half fade)
{ 
    return lerp(x, x*x, fade);
}

inline half LSky_Pow3(half x, in half fade)
{ 
    return lerp(x, x*x*x, fade);
}

inline half3 LSky_Pow3(half3 x, in half fade)
{ 
    return lerp(x, x*x*x, fade);
}

//------------------------------------------- Cubemap -- -------------------------------------------
//==================================================================================================

#ifdef LSKY_CUBE_HDR
inline half3 LSky_CUBE(samplerCUBE cubemap, inout half4 cubemapHDR, float contrast, float3 coords)
#else
inline half3 LSky_CUBE(samplerCUBE cubemap, float contrast, float3 coords)
#endif
{
    half3 col = half3(0.0, 0.0, 0.0);
    half4 cube = texCUBE(cubemap, coords);

    #ifdef LSKY_CUBE_HDR
        col = DecodeHDR(cube, cubemapHDR);
        col *= unity_ColorSpaceDouble.rgb;
    #else
        col = saturate(cube.rgb);
    #endif

    return LSky_Pow3(col.rgb, contrast);
}


#endif // LSKY COMMON INCLUDED.
