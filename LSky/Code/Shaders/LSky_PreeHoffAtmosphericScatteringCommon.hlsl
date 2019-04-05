#ifndef LSKY_PREEHTAM_HOFFMAN_ATMOSPHERIC_SCATTERING
#define LSKY_PREEHTAM_HOFFMAN_ATMOSPHERIC_SCATTERING

//#include "UnityCG.cginc"
#include "LSky_AtmosphericScatteringVariables.hlsl"
#include "LSky_AtmosphericScatteringCommon.hlsl"

//////////////////////////////////////////////////
/// Description: Atmospheric Scattering based on 
/// Naty Hoffman and Arcot. J. Preetham papers.
//////////////////////////////////////////////////

//------------------------------------------- Def Const --------------------------------------------
//==================================================================================================

#ifndef LSKY_RAYLEIGH_ZENITH_LENGTH
    #define LSKY_RAYLEIGH_ZENITH_LENGTH lsky_RayleighZenithLength
#endif

#ifndef LSKY_MIE_ZENITH_LENGTH
    #define LSKY_MIE_ZENITH_LENGTH lsky_MieZenithLength
#endif


//--------------------------------------------- Params ---------------------------------------------
//==================================================================================================

uniform float lsky_AtmosphereHaziness;
uniform float lsky_AtmosphereZenith;

uniform float lsky_RayleighZenithLength;
uniform float lsky_MieZenithLength;

uniform float3 lsky_BetaRay;
uniform float3 lsky_BetaMie;

uniform float lsky_SunsetDawnHorizon;
uniform half lsky_DayIntensity;
uniform half lsky_NightIntensity;


//------------------------------------------- Scattering -------------------------------------------
//==================================================================================================

// Defautl optical depth.
inline void OpticalDepth(float y, inout float2 srm)
{

    y = saturate(y); 
    float zenith = acos(y);
    zenith = cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180) / UNITY_PI), -1.253);
    zenith = 1.0/zenith;

    srm.x = zenith * LSKY_RAYLEIGH_ZENITH_LENGTH;
    srm.y = zenith * LSKY_MIE_ZENITH_LENGTH;
}

// Optical depth with small changes for more customization.
inline void CustomOpticalDepth(float y, inout float2 srm)
{

    y = saturate(y * lsky_AtmosphereHaziness); 

    float zenith = acos(y);
    zenith = cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180) / UNITY_PI), -1.253);
    zenith = 1.0/(zenith + lsky_AtmosphereZenith);

    srm.x = zenith * LSKY_RAYLEIGH_ZENITH_LENGTH;
    srm.y = zenith * LSKY_MIE_ZENITH_LENGTH;
}

// Optimized for mobile devices.
inline void OptimizedOpticalDepth(float y, inout float2 srm)
{
    y = saturate(y * lsky_AtmosphereHaziness);
    y = 1.0 / (y + lsky_AtmosphereZenith);
    srm.x = y * LSKY_RAYLEIGH_ZENITH_LENGTH;
    srm.y = y * LSKY_MIE_ZENITH_LENGTH;
}

/*
// Based in Nielsen paper
inline void NielsenOpticalDepth(float y, inout float2 srm)
{
    y = saturate(y);
    float f = pow(y, lsky_AtmosphereHaziness); // h 1 / 5.0 0.25
    float t = (1.05 - f) * 190000;
	
    srm.x = t + f * (LSKY_RAYLEIGH_ZENITH_LENGTH - t);
    srm.y = t + f * (LSKY_MIE_ZENITH_LENGTH - t);
}
*/

inline half3 AtmosphericScattering(float2 srm, float sunCosTheta, float3 sunMiePhase, float3 moonMiePhase)
{
    // Combined Extinction Factor.
    float3 fex  = exp(-(lsky_BetaRay * srm.x + lsky_BetaMie * srm.y));

    // Combined extinction factor with horizon sunset/dawn color.
    float3 ffex = saturate(lerp(1.0-fex, (1.0-fex) * fex, lsky_SunsetDawnHorizon));

    // Compute rayleigh phase for sun.
    float sunRayleighPhase = LSky_RayleighPhase(sunCosTheta);

    // Sun/Day.
    //-------------------------------------------------------------------------------------
    float3 sunBRT  = lsky_BetaRay * sunRayleighPhase;
    float3 sunBMT  = sunMiePhase * lsky_BetaMie;
    float3 sunBRMT = (sunBRT + sunBMT) / (lsky_BetaRay + lsky_BetaMie);

    half3 sunScatter = lsky_DayIntensity * (sunBRMT * ffex) * lsky_SunAtmosphereTint;

    // Moon/Night.
    //------------------------------------------------------------------------------------
    #ifdef LSKY_ENABLE_MOON_RAYLEIGH
        half3 moonScatter = lsky_NightIntensity.x * (1.0-fex) * lsky_MoonAtmosphereTint; // Simple night rayleigh.
        moonScatter += moonMiePhase;
        return sunScatter + moonScatter; // Return day scattering + nigth scattering.
    #else
        return sunScatter + moonMiePhase;
    #endif
}

inline half3 LSky_ComputeAtmosphere(float3 pos, out float3 sunMiePhase, out float3 moonMiePhase)
{
    // Result color.
    half3 col = half3(0.0, 0.0, 0.0);

    // sR, sM
    float2 srm;

    // Get dot product of the sun and moon.
    float2 cosTheta = float2(dot(pos.xyz, lsky_LocalSunDirection.xyz), dot(pos.xyz, lsky_LocalMoonDirection.xyz)); 

    // Get uo side mask.
    fixed skyMask = 1.0-LSky_GroundMask(pos.y);

    // Compute sun mie phase in up side.
    sunMiePhase = LSky_PartialMiePhase(cosTheta.x, lsky_PartialSunMiePhase, lsky_SunMieScattering) * lsky_SunMieTint.rgb * skyMask;

    // Compute moon mie phase in up side.
    moonMiePhase = LSky_PartialMiePhase(cosTheta.y, lsky_PartialMoonMiePhase, lsky_MoonMieScattering) * lsky_MoonMieTint.rgb * skyMask; 

    // Get optical depth.
    #ifdef SHADER_API_MOBILE
        OptimizedOpticalDepth(pos.y, srm.xy); 
    #else
        CustomOpticalDepth(pos.y, srm.xy);
    #endif

    // Compute atmospheric scattering.
    col.rgb = AtmosphericScattering(srm.xy, cosTheta.x, sunMiePhase, moonMiePhase);

    // Apply color correction.
    AtmosphereColorCorrection(col.rgb, lsky_GroundColor.rgb, LSKY_GLOBALEXPOSURE, lsky_AtmosphereContrast);
    
    // Apply ground.
    col.rgb = applyGroundColor(pos.y, col.rgb);

    // return final color.
    return col;
}

#endif // LSKY: PREETHAM AND HOFFMAN ATMOSPHERIC SCATTERING INCLUDED.
