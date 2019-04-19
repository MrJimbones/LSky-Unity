#ifndef LSKY_PREEHTAM_HOFFMAN_ATMOSPHERIC_SCATTERING
#define LSKY_PREEHTAM_HOFFMAN_ATMOSPHERIC_SCATTERING

//#include "UnityCG.cginc"
#include "LSky_AtmosphericScatteringVariables.hlsl"
#include "LSky_AtmosphericScatteringCommon.hlsl"

//////////////////////////////////////////////////
/// Description: Atmospheric Scattering based on 
/// Naty Hoffman and Arcot. J. Preetham papers.
//////////////////////////////////////////////////


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

// Optical depth with small changes for more customization.
inline void CustomOpticalDepth(float y, inout float2 srm)
{

    y = saturate(y * lsky_AtmosphereHaziness); 

    float zenith = acos(y);
    zenith = cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180) / UNITY_PI), -1.253);
    zenith = 1.0/(zenith + lsky_AtmosphereZenith);

    srm.x = zenith * lsky_RayleighZenithLength;
    srm.y = zenith * lsky_MieZenithLength;
}

// Optimized for mobile devices.
inline void OptimizedOpticalDepth(float y, inout float2 srm)
{
    y = saturate(y * lsky_AtmosphereHaziness);
    y = 1.0 / (y + lsky_AtmosphereZenith);
    srm.x = y * lsky_RayleighZenithLength;
    srm.y = y * lsky_MieZenithLength;
}


inline half3 AtmosphericScattering(float2 srm, float sunCosTheta, float3 sunMiePhase, float3 moonMiePhase, float depth)
{
    // Combined Extinction Factor.
    float3 fex  = exp(-(lsky_BetaRay * srm.x + lsky_BetaMie * srm.y));

    // Combined extinction factor with horizon sunset/dawn color.
    float3 ffex = saturate(lerp(1.0-fex, (1.0-fex) * fex, lsky_SunsetDawnHorizon));

    // Compute rayleigh phase for sun.
    float sunRayleighPhase = LSky_RayleighPhase(sunCosTheta);

    // Sun/Day.
    //-------------------------------------------------------------------------------------
    float3 sunBRT  = lsky_BetaRay * sunRayleighPhase * (depth * LSKY_RAYLEIGHDEPTHMULTIPLIER);
    float3 sunBMT  = sunMiePhase * lsky_BetaMie;
    float3 sunBRMT = (sunBRT + sunBMT) / (lsky_BetaRay + lsky_BetaMie);

    half3 sunScatter = lsky_DayIntensity * (sunBRMT * ffex) * lsky_SunAtmosphereTint;
   

    // Moon/Night.
    //------------------------------------------------------------------------------------
    #ifdef LSKY_ENABLE_MOON_RAYLEIGH
        half3 moonScatter = lsky_NightIntensity.x * (1.0-fex) * lsky_MoonAtmosphereTint * (depth * LSKY_RAYLEIGHDEPTHMULTIPLIER); // Simple night rayleigh.
        moonScatter += moonMiePhase;
        return sunScatter + moonScatter; // Return day scattering + nigth scattering.
    #else
        return sunScatter + moonMiePhase;
    #endif
}

#ifdef LSKY_COMPUTE_MIE_PHASE
inline half3 LSky_ComputeAtmosphere(float3 pos, out float3 sunMiePhase, out float3 moonMiePhase, float depth, float dist)
#else
inline half3 LSky_ComputeAtmosphere(float3 pos, float depth, float dist)
#endif
{
    // Result color.
    half3 col = half3(0.0, 0.0, 0.0);

    // sR, sM
    float2 srm;


    // x = SunMiePhaseMul, y = MoonMiePhaseMul, z = Upside Mask;
    half3 multParams = half3(1.0, 1.0, 1.0);
    
    #ifndef LSKY_ENABLE_POST_FX
    multParams.x = 1.0;
    multParams.y = 1.0;
    multParams.z = 1.0-LSky_GroundMask(pos.y);
    #else
    multParams.x = lsky_FogSunMiePhaseMult;
    multParams.y = lsky_FogMoonMiePhaseMult;
    multParams.z = 1.0;
    #endif

    // Get dot product of the sun and moon.
    float2 cosTheta = float2(dot(pos.xyz, lsky_LocalSunDirection.xyz), dot(pos.xyz, lsky_LocalMoonDirection.xyz)); 

    #ifdef LSKY_COMPUTE_MIE_PHASE
    // Compute sun mie phase in up side.
    sunMiePhase = (depth*LSKY_SUNMIEPHASEDEPTHMULTIPLIER) * LSky_PartialMiePhase(cosTheta.x, lsky_PartialSunMiePhase, lsky_SunMieScattering);
    sunMiePhase *= multParams.x * lsky_SunMieTint.rgb * multParams.z;

    // Compute moon mie phase in up side.
    moonMiePhase = (depth*LSKY_MOONMIEPHASEDEPTHMULTIPLIER) * LSky_PartialMiePhase(cosTheta.y, lsky_PartialMoonMiePhase, lsky_MoonMieScattering);
    moonMiePhase *= multParams.y * lsky_MoonMieTint.rgb * multParams.z; 
   
    #endif

    #ifdef LSKY_ENABLE_POST_FX
    pos = saturate(pos*lsky_FogHaziness);
    #endif

    // Get optical depth.
    #ifdef SHADER_API_MOBILE
        OptimizedOpticalDepth(pos.y, srm.xy); 
    #else
        CustomOpticalDepth(pos.y, srm.xy);
    #endif

    #ifdef LSKY_COMPUTE_MIE_PHASE
        // Compute atmospheric scattering.
        col.rgb = AtmosphericScattering(srm.xy, cosTheta.x, sunMiePhase, moonMiePhase, dist);
    #else
        fixed3 mp = fixed3(0.0, 0.0, 0.0);
        col.rgb = AtmosphericScattering(srm.xy, cosTheta.x, mp, mp, dist);
    #endif

    // Apply color correction.
    AtmosphereColorCorrection(col.rgb, lsky_GroundColor.rgb, LSKY_GLOBALEXPOSURE, lsky_AtmosphereContrast);
    
    // Apply ground.
    #ifndef LSKY_ENABLE_POST_FX
    col.rgb = applyGroundColor(pos.y, col.rgb);
    #endif

    return col;
}

#endif // LSKY: PREETHAM AND HOFFMAN ATMOSPHERIC SCATTERING INCLUDED.
