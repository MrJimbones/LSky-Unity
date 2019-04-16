/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Dome.
///----------------------------------------------
/// Description: Skydome Manager Main.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    public partial class LSky_Dome : MonoBehaviour
    {

        [Header("SKY SETTINGS")]
        
        // Global.
        public LSky_Global global = new LSky_Global();

        [Header("Deep Space")]

        [LSky_AnimationCurveRange(0.0f, 0.0f, 1.0f, 1.0f, 6)]
        [SerializeField] private AnimationCurve m_DeepSpaceExposure = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

        public LSky_GalaxyBackground galaxyBackground = new LSky_GalaxyBackground();
        public LSky_StarsFieldCubemap starsField = new LSky_StarsFieldCubemap();

        [Header("Near Space")]

        public LSky_Sun sun = new LSky_Sun();
        public LSky_Moon moon = new LSky_Moon();


        [Header("Atmosphere")]

        [SerializeField] private bool m_SetGlobalAtmosphereParams = true;
        public LSky_AtmosphericScattering atmosphere = new LSky_AtmosphericScattering();

        [Header("Clouds")]

        public LSky_Clouds clouds = new LSky_Clouds();

        [Header("Lighting")]
        [SerializeField] private bool m_SendSkybox = false;
        [SerializeField] private LSky_DirLightParams m_SunLightParams = new LSky_DirLightParams();
        [SerializeField] private LSky_DirLightParams m_MoonLightParams = new LSky_DirLightParams();

        [LSky_AnimationCurveRange(0.0f, 0.0f, 1.0f, 1.0f, 6)]
        [SerializeField] private AnimationCurve m_SunMoonLightFade = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

        [SerializeField] private float m_DirLightRefreshTime = 0.5f;
        private float m_DirLightRefreshTimer = 0.0f;

        private float SunEvaluateTime => (1.0f -SunDirection.y) * 0.5f;
        private float MoonEvaluteTime => (1.0f -MoonDirection.y) * 0.5f;

        public LSky_Ambient ambient = new LSky_Ambient();

        /// <summary></summary>
        public float DeepSpaceExposure => m_DeepSpaceExposure.Evaluate(SunEvaluateTime);


        #region [Initialize]

        private void Awake()
        {
            Initialize();
        }

        #endregion


        #region [Update]

        private void LateUpdate()
        {

            UpdateDome();

        }


        private void UpdateDome()
        {

            if(!m_IsReady) return;

            if(m_SendSkybox)
            {
                UnityEngine.RenderSettings.skybox = m_Resources.ambientSkyboxMaterial;
                m_SendSkybox = false;
            }

            UpdateCelestialsTransform();
            UpdateCloudsTransform();
            SetParams();
            RenderDome();
            UpdateLight();
            UpdateAmbient();

        }

        private void SetParams()
        {

            global.SetParams(m_Transform, SunDirection, MoonDirection);

            if(m_RenderGalaxyBackground)         
                galaxyBackground.SetParams(m_Resources.galaxyBackgroundMaterial, DeepSpaceExposure);
            
            if(m_RenderStarsField)
                starsField.SetParams(m_Resources.starsFieldMaterial, DeepSpaceExposure);
               
            
            sun.SetParams(m_Resources.sunMaterial);
            moon.SetParams(m_Resources.moonMaterial);

            if(m_RenderAtmosphere)
            {

                atmosphere.sunDir  = SunDirection;
                atmosphere.moonDir = MoonDirection;

                if(m_SetGlobalAtmosphereParams)
                    atmosphere.SetGlobalParams(SunEvaluateTime);
                else
                    atmosphere.SetParams(m_Resources.atmosphereMaterial, SunEvaluateTime);

            }

            if(m_RenderClouds)
            {
                
                if(atmosphere.moonRayleighMode == LSky_CelestialRayleighMode.CelestialContribution)
                    clouds.SetParams(m_Resources.cloudsMaterial, SunEvaluateTime, true, MoonEvaluteTime);
                else
                    clouds.SetParams(m_Resources.cloudsMaterial, SunEvaluateTime, false);

            }

        }

        private void UpdateLight()
        {
            m_DirLightRefreshTimer += Time.deltaTime;
            if(m_DirLightRefreshTime >= m_DirLightRefreshTimer)
            {
                if(IsDay)
                {
                    m_DirLightRef.transform.localPosition = SunPosition;
                    m_DirLightRef.transform.LookAt(m_Transform);
                    m_DirLightRef.light.color = m_SunLightParams.color.Evaluate(SunEvaluateTime);
                    m_DirLightRef.light.intensity = m_SunLightParams.intensity;
                }
                else
                {
                    m_DirLightRef.transform.localPosition = MoonPosition;
                    m_DirLightRef.transform.LookAt(m_Transform);
                    m_DirLightRef.light.color = m_MoonLightParams.color.Evaluate(MoonEvaluteTime);
                    m_DirLightRef.light.intensity = m_MoonLightParams.intensity * m_SunMoonLightFade.Evaluate(SunEvaluateTime);
                }
                m_DirLightRefreshTimer = 0.0f;
            }
            m_DirLightRef.light.enabled = DirLightEnbled;
        }

        private void UpdateAmbient()
        {
            if(atmosphere.moonRayleighMode == LSky_CelestialRayleighMode.CelestialContribution)
                ambient.UpdateAmbient(SunEvaluateTime, true, MoonEvaluteTime);
            else
                ambient.UpdateAmbient(SunEvaluateTime, false);
        }

        #endregion

    }
}