
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


// Moon
uniform float lsky_MoonMieAnisotropy;
uniform half4 lsky_MoonMieTint;
uniform half  lsky_MoonMieScattering;

// Depth.
uniform half lsky_MoonMiePhaseDepthMultiplier;

uniform float lsky_FogHaziness;
uniform half lsky_FogSunMiePhaseMult;
uniform half lsky_FogMoonMiePhaseMult;

#endif // LSKY: ATMOSPHERIC SCATTERING VARIABLES INCLUDED.
