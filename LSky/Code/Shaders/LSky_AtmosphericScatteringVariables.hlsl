
#ifndef LSKY_ATMOSPHERIC_SCATTERING_VARIABLES
#define LSKY_ATMOSPHERIC_SCATTERING_VARIABLES

// General.
//-------------------------------------------------
uniform half lsky_AtmosphereContrast;
uniform half4 lsky_GroundColor;

// Sun.
uniform half  lsky_SunBrightness;
uniform half3 lsky_SunAtmosphereTint;

// Moon.
uniform half  lsky_MoonContribution;
uniform half3 lsky_MoonAtmosphereTint;

// Depth
uniform half lsky_RayleighDepthMultiplier;

#ifndef LSKY_RAYLEIGHDEPTHMULTIPLIER 
#       define LSKY_RAYLEIGHDEPTHMULTIPLIER lsky_RayleighDepthMultiplier
#endif


// Mie.
//-------------------------------------------------
// Sun
uniform float lsky_SunMieAnisotropy;
uniform half4 lsky_SunMieTint;
uniform half  lsky_SunMieScattering;

// Partial Mie Phase.
uniform float3 lsky_PartialSunMiePhase;

// Partial Mie Phase.
uniform float3 lsky_PartialMoonMiePhase;

// Depth
uniform half lsky_SunMiePhaseDepthMultiplier;

#ifndef LSKY_SUNMIEPHASEDEPTHMULTIPLIER 
#   define LSKY_SUNMIEPHASEDEPTHMULTIPLIER lsky_SunMiePhaseDepthMultiplier
#endif

// Moon
uniform float lsky_MoonMieAnisotropy;
uniform half4 lsky_MoonMieTint;
uniform half  lsky_MoonMieScattering;

// Depth.
uniform half lsky_MoonMiePhaseDepthMultiplier;

#ifndef LSKY_MOONMIEPHASEDEPTHMULTIPLIER 
#   define LSKY_MOONMIEPHASEDEPTHMULTIPLIER lsky_MoonMiePhaseDepthMultiplier
#endif

uniform float lsky_FogHaziness;

#endif // LSKY: ATMOSPHERIC SCATTERING VARIABLES INCLUDED.
