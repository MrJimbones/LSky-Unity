/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Atmospheric Scattering.
///----------------------------------------------
/// Description:
/// Atmospheric Scattering based on
/// Naty Hoffman and Arcot. J. Preetham papers.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;


namespace Rallec.LSky
{

   
    /// <summary> 
    /// Atmospheric scattering wavelength parameters.
    /// </summary>
    [Serializable] public struct LSky_WavelengthParams
    {
        #region [Fields]

        [Range(0,2000)] public float red, green, blue;

        #endregion

        #region [Constructor]

        public LSky_WavelengthParams(float _red, float _green, float _blue) 
        {
            this.red = _red; this.green = _green; this.blue = _blue;
        }

        #endregion

        #region [Defautl Values]

        // Wavelenght eart values.
        static readonly LSky_WavelengthParams earthWavelenghValues  = new LSky_WavelengthParams(650f, 570f, 475f);
        static readonly LSky_WavelengthParams earthWavelenghValues2 = new LSky_WavelengthParams(680f, 550f, 440f);
        static readonly LSky_WavelengthParams earthWavelenghValues3 = new LSky_WavelengthParams(650f, 550f, 475f);
       
        /// <summary> Defautl Wavelength Eart Values #1. </summary>
        public static LSky_WavelengthParams EarthValues{ get{ return earthWavelenghValues; } }

        /// <summary> Defautl Wavelength Eart Values #2. </summary>
        public static LSky_WavelengthParams EarthValues2{ get{ return earthWavelenghValues2; } }

        /// <summary> wavelength * 1e-9 </summary>
        public Vector3 Lambda
        {
            get
            {
                const float m = 1e-9f;
                Vector3 res;
                {
                    res.x = red   * m;
                    res.y = green * m;
                    res.z = blue  * m;
                }

                return res; 
            }
        }

        #endregion   
    }

    /// <summary> Common values for mie phase. </summary>
    [Serializable] public struct LSky_MiePhaseValues
    {
        #region [Fields]

        public Color tint;
        [Range(0, 0.999f)] public float anisotropy;
        public float scattering;

        #endregion

        #region [Constructor]
        public LSky_MiePhaseValues(Color _tint, float _anisotropy, float _scattering)
        {
            this.tint       = _tint;
            this.anisotropy = _anisotropy;
            this.scattering = _scattering;
        }
        #endregion

        // Default values.
        static readonly LSky_MiePhaseValues m_Defautl = new LSky_MiePhaseValues(Color.white, 0.80f, 1.0f);

        /// <summary> Default values. </summary>
        public static LSky_MiePhaseValues Default{ get{ return m_Defautl; } }
    }

    /// <summary></summary>
    [Serializable] public class LSky_AtmosphericScatteringParams
    {

        #region [Fields|General]

        public bool applyFastTonemaping;

        // Use for exponent fade.
        [Range(0.0f, 1.0f)]public float contrast;

        public Color groundColor;

        #endregion

        #region [Fields|Rayleigh]

        // Wavelegths.
        public LSky_WavelengthParams wavelength;

        // General.
        [Range(0.0f, 25f)] public float scattering;

        // Optical Depth.
        [Range(0.0f, 1.0f)] public float atmosphereHaziness;
        [Range(0.0f, 0.1f)] public float atmosphereZenith;
        [Range(0.0f, 8.4e3f)] public float rayleighZenithLength;

        // Sun/Day.
        [Range(0.0f, 150f)] public float sunBrightness;    
        public Gradient sunAtmosphereTint;

        // Moon/NIght.
        [Range(0.0f, 1.0f)] public float moonContribution;
        public Color moonAtmosphereTint;

        #endregion

        #region [Fields|Mie]

        // General.
        [Range(0,0.5f)] public float turbidity;

        [Range(0.0f, 1.25e3f)] public float mieZenithLength;

        // Sun mie values.
        public LSky_MiePhaseValues sunMie;

        // Moon mie values.
        public LSky_MiePhaseValues moonMie;

        #endregion
    }


    /// <summary></summary>
    [Serializable] public class LSky_AtmosphericScattering
    {

        // Shader quality.
        public LSky_ShaderQuality shaderQuality = LSky_ShaderQuality.PerVertex;

        // Rayleigh mode.
        public LSky_CelestialRayleighMode moonRayleighMode = LSky_CelestialRayleighMode.OpossiteSun;

        // Precompute.
        public LSky_UpdateType betaRayUpdate = LSky_UpdateType.OnInit;

        // Precompute.
        public LSky_UpdateType betaMieUpdate = LSky_UpdateType.OnInit;

        [SerializeField] LSky_AtmosphericScatteringParams parameters = new LSky_AtmosphericScatteringParams
        {
            applyFastTonemaping = true,
            contrast = 0.5f,
            groundColor = Color.white,
            wavelength = LSky_WavelengthParams.EarthValues,
            scattering = 1.0f,
            atmosphereHaziness = 1.0f,
            atmosphereZenith = 0.0f,
            rayleighZenithLength = 8.4e3f,
            sunBrightness = 30f,
            sunAtmosphereTint = new Gradient(),
            moonContribution = 0.3f,
            moonAtmosphereTint = Color.white,
            turbidity = 0.0001f,
            mieZenithLength = 1.25e3f,
            sunMie = LSky_MiePhaseValues.Default,
            moonMie = LSky_MiePhaseValues.Default
            
        };

        // Beta Ray.
        public Vector3 betaRay;
        public Vector3 betaMie;

        #region [Keywords]

        // General.
        protected readonly string m_ApplyFastTonemapingKeyword = "LSKY_APPLY_FAST_TONEMAPING";
        protected readonly string m_PerPixelAtmosphereKeyword  = "LSKY_PER_PIXEL_ATMOSPHERE";

        // Rayleigh
        protected readonly string m_EnableMoonRayleighKeyword = "LSKY_ENABLE_MOON_RAYLEIGH";

        #endregion

        #region [PropertyIDs]

        // General.
        private int m_ContrastID;
        private int m_GroundColorID;

        /// <summary></summary>
        public int ContrastID{ get{ return m_ContrastID; } }

        /// <summary></summary>
        public int GroundColorID{ get{ return m_GroundColorID; } }

        // Rayleigh.
        //-------------------------------------------------------------------------------
        // Wavelengh.
        private int m_WavelegthRID, m_WavelegthGID, m_WavelegthBID;

        // Tickness.
        private int m_ScatteringID;

        // Oṕtical Depth.
        private int m_AtmosphereHazinessID, m_AtmosphereZenithID, m_RayleighZenithID;
        private int m_RayleighZenithLengthID;

        // Rayleigh.
        private int m_BetaRayID, m_SunsetDawnHorizonID, m_DayIntensityID, m_NightIntensityID;
        private int m_SunBrightnessID, m_SunAtmosphereTintID;
        private int m_MoonContributionID, m_MoonAtmosphereTintID, m_MoonRayleighModeID;

        /// <summary></summary>
        public int WavelegthRID{ get{ return m_WavelegthRID; } }

        /// <summary></summary>
        public int WavelegthGID{ get{ return m_WavelegthGID; } }

        /// <summary></summary>
        public int WavelegthBID{ get{ return m_WavelegthBID; } }

        /// <summary></summary>
        public int ScatteringID{ get{ return m_ScatteringID; } }

        /// <summary></summary>
        public int SunBrightnessID{ get{ return m_SunBrightnessID; } }

        /// <summary></summary>
        public int AtmosphereHazinessID{ get{ return m_AtmosphereHazinessID; } }

        /// <summary></summary>
        public int AtmosphereZenithID{ get{ return m_AtmosphereZenithID; } }

        /// <summary></summary>
        public int RayleighZenithLengthID{ get{ return m_RayleighZenithLengthID; } }

        /// <summary></summary>
        public int BetaRayID{ get{ return m_BetaRayID; } }

        /// <summary></summary>
        public int SunsetDawnHorizonID{ get{ return m_SunsetDawnHorizonID; } }

        /// <summary></summary>
        public int DayIntensityPropertyID{ get{ return m_DayIntensityID; } }

        /// <summary></summary>
        public int NightIntensityPropertyID{ get{ return m_NightIntensityID; } }

        /// <summary></summary>
        public int SunAtmosphereTintID{ get{ return m_SunAtmosphereTintID; } }

        /// <summary></summary>
        public int MoonContributionID{ get{ return m_MoonContributionID; } }

        /// <summary></summary>
        public int MoonAtmosphereTintID{ get{ return m_MoonAtmosphereTintID; } }

        /// <summary></summary>
        public int MoonRayleighModeID{ get{ return m_MoonRayleighModeID; } }

        // Mie.
        private int m_MieID;
        private int m_SunMieTintID, m_SunMieAnisotropyID, m_SunMieScatteringID, m_PartialSunMiePhaseID;
        private int m_MoonMieTintID, m_MoonMieAnisotropyID, m_MoonMieScatteringID, m_PartialMoonMiePhaseID;
        private int m_MieZenithLengthID, m_BetaMieID;

        /// <summary></summary>
        public int MieID{ get{ return m_MieID; } }

        /// <summary></summary>
        public int MieZenithLengthID{ get{ return m_MieZenithLengthID; } }

        /// <summary></summary>
        public int BetaMieID{ get{ return m_BetaMieID; } }

        /// <summary></summary>
        public int SunMieTintID{ get{ return m_SunMieTintID;} }

        /// <summary></summary>
        public int SunMieAnisotropyID{ get{ return m_SunMieAnisotropyID; } }

        /// <summary></summary>
        public int SunMieScatteringID{ get{ return m_SunMieScatteringID; } }

        /// <summary></summary>
        public int PartialSunMiePhaseID{ get{ return m_PartialSunMiePhaseID; } }

        /// <summary></summary>
        public int MoonMieTintID{ get{ return m_MoonMieTintID; } }

        /// <summary></summary>
        public int MoonMieAnisotropyID{ get{ return m_MoonMieAnisotropyID; } }

        /// <summary></summary>
        public int MoonMieScatteringID{ get{ return m_MoonMieScatteringID; } }

        /// <summary></summary>
        public int PartialMoonMiePhaseID{ get{ return m_PartialMoonMiePhaseID; } }

        #endregion

        #region [Constants|Rayleigh]

        /// <summary> Index of air refraction(n). </summary>
        public const float k_n = 1.0003f;

        /// <summary> Molecular density(N). </summary>
        public const float k_N = 2.545e25f;

        /// <summary> Depolatization factor for standart air. </summary>
        public const float k_pn = 0.035f;

        /// <summary> Molecular density exponentially squared. </summary>
        public const float k_n2 = k_n * k_n;

        #endregion

        #region [Properties]

        /// <summary> Simple Day Intensity No physically based. </summary>
        public float DayIntensity
        {
            get{ return (LSky_Mathf.Saturate(sunDir.y + 0.40f) * parameters.sunBrightness); }
        }

        
        /// <summary> Sunset/Dawn atmosphere horizon. </summary>
        public float SunsetDawnHorizon
        {
            get{ return LSky_Mathf.Saturate(Mathf.Clamp(1.0f - (sunDir.y), 0.0f, 1f)); }
        }

        /// <summary></summary>
        public float MoonPhasesIntensityMultiplier
        {
            get{ return Mathf.Clamp01(Vector3.Dot(-sunDir, moonDir) + 0.45f); }
        }

        /// <summary> Night intensity(No physically based) </summary>
        public float NightIntensity
        {
            get
            {
                switch(moonRayleighMode)
                {
                    case LSky_CelestialRayleighMode.OpossiteSun:
                        return LSky_Mathf.Saturate(-sunDir.y + 0.25f);

                    case LSky_CelestialRayleighMode.CelestialContribution:
                        return LSky_Mathf.Saturate(moonDir.y + 0.25f);

                }
                return 0.0f;
            }
        }

        /// <summary></summary>
        public float NightIntensityMultiplier
        {
            get
            {
                switch(moonRayleighMode)
                {
                    case LSky_CelestialRayleighMode.OpossiteSun:
                        return parameters.moonContribution;

                    case LSky_CelestialRayleighMode.CelestialContribution:
                        return parameters.moonContribution * MoonPhasesIntensityMultiplier;
                }

                return 0.0f;  // Off
            }
        }

        /// <summary> One part of Mie Phase </summary>
        public Vector3 PartialSunMiePhase
        {
            get{ return PartialMiePhase(parameters.sunMie.anisotropy); }
        }

        /// <summary> One part of HenyeyGreenstein for moon. </summary>
        public Vector3 PartialMoonMiePhase
        {
            get{ return PartialMiePhase(parameters.moonMie.anisotropy); }
        }

        /// <summary> One part of Low Quality HenyeyGreenstein for moon. </summary>
        public Vector3 PartialMoonMiePhaseSimplifield
        {
            get{ return PartialHenyeyGreenstein(parameters.moonMie.anisotropy); }
        }

        
        /// <summary></summary>
        public LSky_AtmosphericScatteringParams Parameters
        { 
            get{ return parameters; } 
            set{ parameters = value; }
        }

        #endregion

        #region [References]

        public Vector3 sunDir = Vector3.zero;
        public Vector3 moonDir = Vector3.zero;

        #endregion

        #region [Methods|Initialize]

        /// <summary></summary>
        public void InitPropertyIDs()
        {
            // General.
            m_ContrastID = Shader.PropertyToID("lsky_AtmosphereContrast");
            m_GroundColorID = Shader.PropertyToID("lsky_GroundColor");

            // Rayleigh.
            m_WavelegthRID = Shader.PropertyToID("lsky_WavelegthR");
            m_WavelegthGID = Shader.PropertyToID("lsky_WavelegthG");
            m_WavelegthBID = Shader.PropertyToID("lsky_WavelegthB");
            m_ScatteringID = Shader.PropertyToID("lsky_RayleighScattering");
            m_SunBrightnessID  = Shader.PropertyToID("lsky_SunBrightness");
            m_AtmosphereHazinessID    = Shader.PropertyToID("lsky_AtmosphereHaziness");
            m_AtmosphereZenithID      = Shader.PropertyToID("lsky_AtmosphereZenith");
            m_RayleighZenithLengthID  = Shader.PropertyToID("lsky_RayleighZenithLength");
            m_BetaRayID               = Shader.PropertyToID("lsky_BetaRay");
            m_SunsetDawnHorizonID     = Shader.PropertyToID("lsky_SunsetDawnHorizon");
            m_DayIntensityID          = Shader.PropertyToID("lsky_DayIntensity");
            m_NightIntensityID        = Shader.PropertyToID("lsky_NightIntensity");
            m_SunAtmosphereTintID  = Shader.PropertyToID("lsky_SunAtmosphereTint");
            m_MoonContributionID   = Shader.PropertyToID("lsky_MoonContribution");
            m_MoonAtmosphereTintID = Shader.PropertyToID("lsky_MoonAtmosphereTint");
            m_MoonRayleighModeID   = Shader.PropertyToID("lsky_MoonRayleighMode");

            // General.    
            m_MieID = Shader.PropertyToID("lsky_Mie");
            m_MieZenithLengthID = Shader.PropertyToID("lsky_MieZenithLength");
            m_BetaMieID = Shader.PropertyToID("lsky_BetaMie");

            // Sun.
            m_SunMieTintID         = Shader.PropertyToID("lsky_SunMieTint");
            m_SunMieAnisotropyID   = Shader.PropertyToID("lsky_SunMieAnisotropy");
            m_SunMieScatteringID   = Shader.PropertyToID("lsky_SunMieScattering");
            m_PartialSunMiePhaseID = Shader.PropertyToID("lsky_PartialSunMiePhase");

            // Moon.
            m_MoonMieTintID         = Shader.PropertyToID("lsky_MoonMieTint");
            m_MoonMieAnisotropyID   = Shader.PropertyToID("lsky_MoonMieAnisotropy");
            m_MoonMieScatteringID   = Shader.PropertyToID("lsky_MoonMieScattering");
            m_PartialMoonMiePhaseID = Shader.PropertyToID("lsky_PartialMoonMiePhase");  
        }

        /// <summary></summary>
        public void Initialize()
        {
               
            // Get beta ray and beta mie.
            if(betaRayUpdate == LSky_UpdateType.OnInit)
                GetBetaRay();
            
            if(betaMieUpdate == LSky_UpdateType.OnInit)
                GetBetaMie();
        }

        #endregion

        #region [Methods|SetParams]

        /// <summary> Set Glonal Parameters to materials </summary>
        public void SetGlobalParams(float sunEvaluateTime)
        {
            
            // General Settings.
            //------------------------------------------------------------------------------------------------------------------------
            // Apply fast tonemaping.
            if(parameters.applyFastTonemaping)
                Shader.EnableKeyword(m_ApplyFastTonemapingKeyword);
            else
                Shader.DisableKeyword(m_ApplyFastTonemapingKeyword);

            // Set Shader Quality.
            switch(shaderQuality)
            {
                case LSky_ShaderQuality.PerVertex: Shader.DisableKeyword(m_PerPixelAtmosphereKeyword); break;
                case LSky_ShaderQuality.PerPixel: Shader.EnableKeyword(m_PerPixelAtmosphereKeyword); break;
            }

            // Exṕonent Fade(Contrast).
            Shader.SetGlobalFloat(ContrastID, parameters.contrast);
            Shader.SetGlobalColor(GroundColorID, parameters.groundColor);

            // Rayleigh.
            //-------------------------------------------------------------------------------------------------------------------------
            Shader.SetGlobalColor(SunAtmosphereTintID, parameters.sunAtmosphereTint.Evaluate(sunEvaluateTime)); // Set day atmosphere tint.

            switch(moonRayleighMode)
            {
                case LSky_CelestialRayleighMode.OpossiteSun:

                Shader.EnableKeyword(m_EnableMoonRayleighKeyword);
                Shader.SetGlobalInt(m_MoonAtmosphereTintID, 0);

                break;

                case LSky_CelestialRayleighMode.CelestialContribution:

                Shader.EnableKeyword(m_EnableMoonRayleighKeyword);
                Shader.SetGlobalInt(m_MoonAtmosphereTintID, 1);
              
                break;

                case LSky_CelestialRayleighMode.Off: Shader.DisableKeyword(m_EnableMoonRayleighKeyword); break;

            }

            Shader.SetGlobalColor
            (
                MoonAtmosphereTintID, 
                parameters.moonAtmosphereTint * 
                NightIntensityMultiplier
            );

            // Mie Phase.
            //--------------------------------------------------------------------------------------------------
            // Sun
            Shader.SetGlobalColor(SunMieTintID, parameters.sunMie.tint);
            Shader.SetGlobalFloat(SunMieAnisotropyID, parameters.sunMie.anisotropy);
            Shader.SetGlobalFloat(SunMieScatteringID, parameters.sunMie.scattering);
            Shader.SetGlobalVector(PartialSunMiePhaseID, PartialSunMiePhase);

            // Moon
            Shader.SetGlobalColor(MoonMieTintID, parameters.moonMie.tint);
            Shader.SetGlobalFloat(MoonMieAnisotropyID, parameters.moonMie.anisotropy);
            Shader.SetGlobalFloat(MoonMieScatteringID, parameters.moonMie.scattering * MoonPhasesIntensityMultiplier);
            Shader.SetGlobalVector(PartialMoonMiePhaseID, PartialMoonMiePhaseSimplifield);

            // Set optical depth params.
            //------------------------------------------------------------------------------------
            Shader.SetGlobalFloat(AtmosphereHazinessID, parameters.atmosphereHaziness);
            Shader.SetGlobalFloat(AtmosphereZenithID, parameters.atmosphereZenith);
            Shader.SetGlobalFloat(SunsetDawnHorizonID, SunsetDawnHorizon);
            Shader.SetGlobalFloat(RayleighZenithLengthID, parameters.rayleighZenithLength);
            Shader.SetGlobalFloat(MieZenithLengthID, parameters.mieZenithLength);

            if(betaRayUpdate == LSky_UpdateType.Realtime)
                GetBetaRay();

            if(betaMieUpdate == LSky_UpdateType.Realtime)
                GetBetaMie();

            Shader.SetGlobalVector(BetaRayID, betaRay * parameters.scattering);
            Shader.SetGlobalVector(BetaMieID, betaMie);

            Shader.SetGlobalFloat(DayIntensityPropertyID, DayIntensity);
            Shader.SetGlobalFloat(NightIntensityPropertyID, NightIntensity);

        }

        /// <summary> Set Glonal Parameters to materials </summary>
        public void SetParams(Material material, float sunEvaluateTime)
        {
            
            // General Settings.
            //------------------------------------------------------------------------------------------------------------------------
            // Apply fast tonemaping.
            if(parameters.applyFastTonemaping)
                material.EnableKeyword(m_ApplyFastTonemapingKeyword);
            else
                material.DisableKeyword(m_ApplyFastTonemapingKeyword);

            // Set Shader Quality.
            switch(shaderQuality)
            {
                case LSky_ShaderQuality.PerVertex: material.DisableKeyword(m_PerPixelAtmosphereKeyword); break;
                case LSky_ShaderQuality.PerPixel: material.EnableKeyword(m_PerPixelAtmosphereKeyword); break;
            }

            // Exṕonent Fade(Contrast).
            material.SetFloat(ContrastID, parameters.contrast);
            material.SetColor(GroundColorID, parameters.groundColor);

            // Rayleigh.
            //-------------------------------------------------------------------------------------------------------------------------
            material.SetColor(SunAtmosphereTintID, parameters.sunAtmosphereTint.Evaluate(sunEvaluateTime)); // Set day atmosphere tint.

            switch(moonRayleighMode)
            {
                case LSky_CelestialRayleighMode.OpossiteSun:

                material.EnableKeyword(m_EnableMoonRayleighKeyword);
                material.SetInt(m_MoonAtmosphereTintID, 0);

                break;

                case LSky_CelestialRayleighMode.CelestialContribution:

                material.EnableKeyword(m_EnableMoonRayleighKeyword);
                material.SetInt(m_MoonAtmosphereTintID, 1);
              
                break;

                case LSky_CelestialRayleighMode.Off: material.DisableKeyword(m_EnableMoonRayleighKeyword); break;

            }

            material.SetColor
            (
                MoonAtmosphereTintID, 
                parameters.moonAtmosphereTint * 
                NightIntensityMultiplier
            );

            // Mie Phase.
            //--------------------------------------------------------------------------------------------------
            // Sun
            material.SetColor(SunMieTintID, parameters.sunMie.tint);
            material.SetFloat(SunMieAnisotropyID, parameters.sunMie.anisotropy);
            material.SetFloat(SunMieScatteringID, parameters.sunMie.scattering);
            material.SetVector(PartialSunMiePhaseID, PartialSunMiePhase);

            // Moon
            material.SetColor(MoonMieTintID, parameters.moonMie.tint);
            material.SetFloat(MoonMieAnisotropyID, parameters.moonMie.anisotropy);
            material.SetFloat(MoonMieScatteringID, parameters.moonMie.scattering * MoonPhasesIntensityMultiplier);
            material.SetVector(PartialMoonMiePhaseID, PartialMoonMiePhaseSimplifield);

            // Set optical depth params.
            //------------------------------------------------------------------------------------
            material.SetFloat(AtmosphereHazinessID, parameters.atmosphereHaziness);
            material.SetFloat(AtmosphereZenithID, parameters.atmosphereZenith);
            material.SetFloat(SunsetDawnHorizonID, SunsetDawnHorizon);
            material.SetFloat(RayleighZenithLengthID, parameters.rayleighZenithLength);
            material.SetFloat(MieZenithLengthID, parameters.mieZenithLength);

            if(betaRayUpdate == LSky_UpdateType.Realtime)
                GetBetaRay();

            if(betaMieUpdate == LSky_UpdateType.Realtime)
                GetBetaMie();

            material.SetVector(BetaRayID, betaRay * parameters.scattering);
            material.SetVector(BetaMieID,betaMie);

            material.SetFloat(DayIntensityPropertyID, DayIntensity);
            material.SetFloat(NightIntensityPropertyID, NightIntensity);
        }

        public void GetBetaRay()
        {
            betaRay = ComputeBetaRay();
        }

        public void GetBetaMie()
        {
            betaMie = ComputeBetaMie();
        }

        #endregion


        #region [Methdos|Compute]

        /// <summary>
        /// Compute Beta Rayleigh
        /// Based on Preetham and Hoffman papers
        /// </summary>
        /// <param name="lambda"> Wavelength * 1e-9 </param>
        public Vector3 ComputeBetaRay()
        {
            Vector3 result;
           
            // Wavelength.
            Vector3 wl;
            wl.x = Mathf.Pow(parameters.wavelength.Lambda.x, 4.0f);
            wl.y = Mathf.Pow(parameters.wavelength.Lambda.y, 4.0f);
            wl.z = Mathf.Pow(parameters.wavelength.Lambda.z, 4.0f);
            
            // Beta Rayleigh
            float ray = (8.0f * Mathf.Pow(Mathf.PI, 3.0f) * Mathf.Pow(k_n2 - 1.0f, 2.0f) * (6.0f + 3.0f * k_pn));
            Vector3 theta = 3.0f * k_N * wl * (6.0f - 7.0f * k_pn);
            
            result.x = (ray / theta.x);
            result.y = (ray / theta.y);
            result.z = (ray / theta.z);
            
            return result;
        }

        /// <summary> Rayleigh Phase </summary>
        /// <param name="cosTheta"> Cos angle </param>
        public float RayleighPhase(float cosTheta)
        {
            return LSky_Mathf.k_3PI16 * (1.0f + cosTheta * cosTheta);
        } 

        /// <summary> 
        /// Compute one part of the HenyeyGreenstein,
        /// based on Preetham and Hoffman papers.
        /// </summary>
        /// <param name="g"> Anisotropy </param>
        public Vector3 PartialHenyeyGreenstein(float g)
        {

            Vector3 result;
            {
                float g2 = g * g;
                result.x = 1.0f - g2;
                result.y = 1.0f + g2;
                result.z = 2.0f * g;
            }
            return result;
        }

        /// <summary> Compute low quality HenyeyGreenstein. </summary>
        /// <param name="g"> Anisotropy </param>
        /// <param name="cosTheta"> Cos Theta</param>
        public float LQHenyeyGreenstein(float g, float cosTheta)
        {
            float g2 = g * g;
            Vector3 PHG = PartialHenyeyGreenstein(g);
            return LSky_Mathf.k_InvPI4 * (PHG.x / (PHG.y - (PHG.z * cosTheta)));
        }

        /// <summary> Compute one part of the Mie phase </summary>
        /// <param name="g"> Anisotropy </param>
        public Vector3 PartialMiePhase(float g)
        {
            Vector3 result;
            {
                float g2 = g * g;
                result.x = (1.0f - g2) / (2.0f + g2);
                result.y = 1.0f + g2;
                result.z = 2.0f * g;
            }
            return result;
        }

        /// <summary>
        /// Compute Mie Phase.
        /// </summary>
        /// <param name="g"> Anisotropy </param>
        /// <param name="cosTheta"> Cos Theta </param>
        public float MiePhase(float g, float cosTheta)
        {
            float g2 = g * g;
            Vector3 PHG = PartialMiePhase(g);
            return  LSky_Mathf.k_InvPI4 * (PHG.x *  ((1.0f + cosTheta * cosTheta) * Mathf.Pow(PHG.y-(PHG.z*cosTheta),-1.5f)));
        }

        /// <summary>
        /// Compute BetaMie.
        /// Based on Preetham and Hoffman papers.
        /// </summary>
        public Vector3 ComputeBetaMie()
        {
            Vector3 result;

            //float turbidity = m_Mie * 0.05f;
            //Vector3 k = new Vector3(0.685f, 0.679f, 0.670f);
            Vector3 k = new Vector3(0.660f, 0.600f, 0.400f);
            float   c = (0.2f * parameters.turbidity) * 10e-18f;
            float   mieFactor = 0.434f * c * Mathf.PI;
            float   v = 4.0f;

            result.x = (mieFactor * Mathf.Pow((2.0f * Mathf.PI) / parameters.wavelength.Lambda.x, v - 2.0f) * k.x);
            result.y = (mieFactor * Mathf.Pow((2.0f * Mathf.PI) / parameters.wavelength.Lambda.y, v - 2.0f) * k.y);
            result.z = (mieFactor * Mathf.Pow((2.0f * Mathf.PI) / parameters.wavelength.Lambda.z, v - 2.0f) * k.z);
           
            return result;
        }

        /// <summary> 
        /// Compute Custom Optical Depth.
        /// </summary>
        /// <param name="yPos"> Y axis pos. </param>
        /// <param bame="sr"></param>
        /// <param name="sm"></param>
        public void OpticalDepth(float yPos, out float sr, out float sm)
        {

            yPos = LSky_Mathf.Saturate(yPos);

            float zenith = Mathf.Acos(yPos);
            zenith = Mathf.Cos(zenith) + 0.15f * Mathf.Pow(93.885f - ((zenith * 180.0f) / Mathf.PI), -1.253f);
            zenith = 1.0f / zenith;

            sr = parameters.rayleighZenithLength * zenith;
            sm = parameters.mieZenithLength * zenith;

        }

        /// <summary> 
        /// Compute Custom Optical Depth.
        /// Optical depth with small changes for more customization.
        /// </summary>
        /// <param name="yPos"> Y axis pos. </param>
        /// <param bame="sr"></param>
        /// <param name="sm"></param>
        public void CustomOpticalDepth(float yPos, out float sr, out float sm)
        {

            yPos = LSky_Mathf.Saturate(yPos * parameters.atmosphereHaziness);

            float zenith = Mathf.Acos(yPos);
            zenith = Mathf.Cos(zenith) + 0.15f * Mathf.Pow(93.885f - ((zenith * 180.0f) / Mathf.PI), -1.253f);
            zenith = 1.0f / (zenith + (parameters.atmosphereZenith * 0.5f));

            sr = parameters.rayleighZenithLength * zenith;
            sm = parameters.mieZenithLength * zenith;
        }

        /// <summary> Compute Optimized Optical Depth. </summary>
        /// <param name="yPos"> Y axis pos. </param>
        /// <param bame="sr"></param>
        /// <param name="sm"></param>
        public void OptimizedOpticalDepth(float yPos, out float sr, out float sm)
        {

            yPos = LSky_Mathf.Saturate(yPos * parameters.atmosphereHaziness);
            yPos = 1.0f / (yPos + parameters.atmosphereZenith);

            sr = parameters.rayleighZenithLength * yPos;
            sm = parameters.mieZenithLength * yPos;
        }

        /// <summary> Compute Optical Depth Based On Nielsen Paper. </summary>
        /// <param name="yPos"> Y axis pos. </param>
        /// <param bame="sr"></param>
        /// <param name="sm"></param>
        public void NielsenOpticalDepth(float yPos, out float sr, out float sm)
        {

            yPos = LSky_Mathf.Saturate(yPos);

            float f = Mathf.Pow(yPos, parameters.atmosphereHaziness);
            float t = ( 1.05f -f) * 190000;
           
            sr = t + f * (parameters.rayleighZenithLength - t);
            sm = t + f * (parameters.mieZenithLength - t);
        }

        public Color AtmosphericScattering(float sr, float sm, float sunCosTheta, float moonCosTheta, bool enableMiePhase, float sunEvaluateTime)
        {

            // Combined Extinction Factor.
            Vector3 fex;
            fex.x = Mathf.Exp(-(betaRay.x * sr + betaMie.x * sm));
            fex.y = Mathf.Exp(-(betaRay.y * sr + betaMie.y * sm));
            fex.z = Mathf.Exp(-(betaRay.z * sr + betaMie.z * sm));

            Vector3 nfex = Vector3.one - fex;
            Vector3 nfexm;
            nfexm.x = nfex.x * fex.x;
            nfexm.y = nfex.y * fex.y;
            nfexm.z = nfex.z * fex.z;

            Vector3 combExcFac = LSky_Mathf.Saturate(Vector3.Lerp(nfex, nfexm, SunsetDawnHorizon));
            float SunRayleighPhase = RayleighPhase(sunCosTheta);

            // Sun/Day
            Vector3 sunBRT;
            sunBRT.x = betaRay.x * SunRayleighPhase;
            sunBRT.y = betaRay.y * SunRayleighPhase;
            sunBRT.z = betaRay.z * SunRayleighPhase;

            float sunMiePhase = enableMiePhase ? MiePhase(parameters.sunMie.anisotropy, sunCosTheta) * parameters.scattering : 1.0f;

            Vector3 sunBMT;
            sunBMT.x = sunMiePhase * parameters.sunMie.tint.r * betaMie.x;
            sunBMT.y = sunMiePhase * parameters.sunMie.tint.g * betaMie.y;
            sunBMT.z = sunMiePhase * parameters.sunMie.tint.b * betaMie.z;

            Vector3 sunBRMT;
            sunBRMT.x = (sunBRT.x + sunBMT.x) / (betaRay.x + betaMie.x);
            sunBRMT.y = (sunBRT.y + sunBMT.y) / (betaRay.y + betaMie.y);
            sunBRMT.z = (sunBRT.z + sunBMT.z) / (betaRay.z + betaMie.z);

            Vector3 sunScatter; Color sunAtmosphereTint = parameters.sunAtmosphereTint.Evaluate(sunEvaluateTime);
            sunScatter.x = DayIntensity * (sunBRMT.x * combExcFac.x) * sunAtmosphereTint.r; 
            sunScatter.y = DayIntensity * (sunBRMT.y * combExcFac.y) * sunAtmosphereTint.g; 
            sunScatter.z = DayIntensity * (sunBRMT.z * combExcFac.z) * sunAtmosphereTint.b; 

            Color result = new Color(0.0f, 0.0f, 0.0f, 1.0f);
            if(moonRayleighMode != LSky_CelestialRayleighMode.Off)
            {

                Vector3 moonScatter;

                // Simple Moon Scatter.
                moonScatter.x = NightIntensity * nfex.x * parameters.moonAtmosphereTint.r;
                moonScatter.y = NightIntensity * nfex.y * parameters.moonAtmosphereTint.g;
                moonScatter.z = NightIntensity * nfex.x * parameters.moonAtmosphereTint.b;

                float moonMiePhase = enableMiePhase ? LQHenyeyGreenstein(parameters.moonMie.anisotropy, moonCosTheta) * parameters.moonMie.scattering : 1.0f;

                moonScatter.x += moonMiePhase * parameters.moonMie.tint.r;
                moonScatter.y += moonMiePhase * parameters.moonMie.tint.g;
                moonScatter.z += moonMiePhase * parameters.moonMie.tint.b;

                result.r = sunScatter.x + moonScatter.x;
                result.g = sunScatter.y + moonScatter.y;
                result.b = sunScatter.z + moonScatter.z;

            }
            else
            {
                result.r = sunScatter.x;
                result.g = sunScatter.y;
                result.b = sunScatter.z;
            }
    
            return result;
        }

        #endregion
        
    }
}

