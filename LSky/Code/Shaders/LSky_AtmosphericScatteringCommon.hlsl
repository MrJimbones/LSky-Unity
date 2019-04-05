
#ifndef LSKY_ATMOSPHERIC_SCATTERING_COMMON
#define LSKY_ATMOSPHERIC_SCATTERING_COMMON

// Include.
//---------------------------
//#include "LSky_Common.hlsl"


//------------------------------------------- Def Const --------------------------------------------
//==================================================================================================

/////////////////////////////////////////////////////////
/// Based on Naty Hoffman and Arcot. J. Preetham papers.
/////////////////////////////////////////////////////////

// Mie Phase
//-------------------------------------------------
#ifndef LSKY_MIE_PHASE_MULTIPLIER
    #define LSKY_MIE_PHASE_MULTIPLIER LSKY_INVPI4 
#endif

// Rayleigh Phase.
//------------------------------------------------
#ifndef LSKY_RAYLEIGH_PHASE_MULTIPLIER
    #define LSKY_RAYLEIGH_PHASE_MULTIPLIER LSKY_3PI16
#endif

//----------------------------------------- Rayleigh Phase -----------------------------------------
//==================================================================================================

inline float LSky_RayleighPhase(float cosTheta)
{
    return LSKY_RAYLEIGH_PHASE_MULTIPLIER * (1.0 + cosTheta * cosTheta);    
}

//-------------------------------------------- Mie Phase -------------------------------------------
//==================================================================================================

inline float3 LSky_PartialMiePhase(float g)
{
    float g2 = g * g;
    return float3((1.0 - g2) / (2.0 + g2), 1.0 + g2, 2.0 * g);
}

inline float LSky_MiePhase(float cosTheta, float g, half scattering)
{
    float3 PHG = LSky_PartialMiePhase(g);
    return(LSKY_MIE_PHASE_MULTIPLIER * PHG.x * ((1.0 + cosTheta * cosTheta) * pow(PHG.y - (PHG.z * cosTheta), -1.5))) * scattering;
}

inline float LSky_PartialMiePhase(float cosTheta, float3 partialMiePhase, half scattering)
{
    return
    (
        LSKY_MIE_PHASE_MULTIPLIER * partialMiePhase.x * ((1.0 + cosTheta * cosTheta) *
        pow(partialMiePhase.y - (partialMiePhase.z * cosTheta), -1.5))
    ) * scattering;
}

//---------------------------------------- Color Correction ----------------------------------------
//==================================================================================================

inline void AtmosphereColorCorrection(inout half3 col, in half exposure, in half exponentFade)
{

    // Fast Tonemaping.
    #ifdef LSKY_APPLY_FAST_TONEMAPING
        col.rgb = LSky_FastTonemaping(col.rgb, exposure);
    #else
        col.rgb *= exposure;
    #endif
  	
    // Color exponent
    col.rgb = LSky_Pow2(col.rgb, exponentFade);

    // Color space
    #ifdef UNITY_COLORSPACE_GAMMA
        #ifndef SHADER_API_MOBILE
            col.rgb = LSKY_LINEAR_TO_GAMMA(col.rgb);
        #else 
            col.rgb = LSKY_LINEAR_TO_GAMMA_SIMPLE(col.rgb);
        #endif
    #endif
}

inline void AtmosphereColorCorrection(inout half3 col, inout half3 groundCol, in half exposure, in half exponentFade)
{

    // HDR.
    #ifdef LSKY_APPLY_FAST_TONEMAPING
        col.rgb = LSky_FastTonemaping(col.rgb, exposure);
    #else
        col.rgb *= exposure;
    #endif

    // Color exponent
    col.rgb = LSky_Pow2(col.rgb, exponentFade);

    // Color space
    #ifdef UNITY_COLORSPACE_GAMMA
        #ifndef SHADER_API_MOBILE
            col.rgb = LSKY_LINEAR_TO_GAMMA(col.rgb);
        #else 
            col.rgb = LSKY_LINEAR_TO_GAMMA_MOBILE(col.rgb);
        #endif
    #else
        groundCol *= groundCol;
    #endif
}

inline half LSky_GroundMask(in float pos)
{
    return saturate(-pos*100);
}

inline half3 applyGroundColor(float pos, half3 skyCol)
{

    fixed mask = LSky_GroundMask(pos);
    half3 skyContribution =  skyCol * smoothstep(-0.42, 4.2, pos + lsky_GroundColor.a) * mask;
    return  lerp(skyCol.rgb, lsky_GroundColor.rgb * min(0.5,skyCol), mask) + skyContribution;
}

#endif // LSKY: ATMOSPHERIC SCATTERING COMMON INCLUDED.