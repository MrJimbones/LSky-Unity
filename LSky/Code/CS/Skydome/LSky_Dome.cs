/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Dome.
///----------------------------------------------
/// Description: Skydome Manager.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;

namespace Rallec.LSky
{
    [ExecuteInEditMode]
    public class LSky_Dome : MonoBehaviour
    {

        #region [Resources]

        [Header("Resources")]
        [SerializeField] private LSky_DomeResources m_Resources = null;

        [SerializeField] private bool m_IsReady = false;
        public bool CheckResources
        {
            get
            {
                #region [Mesh]

                // Check all meshes.
                if(m_Resources == null) return false;
                
                if(m_Resources.sphereLOD0 == null) return false;
                if(m_Resources.sphereLOD1 == null) return false;
                if(m_Resources.sphereLOD2 == null) return false;
                if(m_Resources.sphereLOD3 == null) return false;

                if(m_Resources.hemisphereLOD0 == null) return false;
                if(m_Resources.hemisphereLOD1 == null) return false;
                if(m_Resources.hemisphereLOD2 == null) return false;
                if(m_Resources.hemisphereLOD3 == null) return false;

                if(m_Resources.atmosphereLOD0 == null) return false;
                if(m_Resources.atmosphereLOD1 == null) return false;
                if(m_Resources.atmosphereLOD2 == null) return false;
                if(m_Resources.atmosphereLOD3 == null) return false;

                if(m_Resources.QuadMesh == null) return false;

                #endregion

                #region [Shaders]

                // Check all shaders.
                if(m_Resources.atmosphereShader == null)       return false;
                if(m_Resources.galaxyBackgroundShader == null) return false;
                if(m_Resources.starsFieldShader == null)       return false;
                if(m_Resources.sunShader == null)              return false;
                if(m_Resources.moonShader == null)             return false;
                if(m_Resources.cloudsShader == null)           return false;
                if(m_Resources.ambientSkyboxShader == null)    return false;

                #endregion

                #region [Material]

                // Check all shaders.
                if(m_Resources.galaxyBackgroundMaterial == null) return false;
                if(m_Resources.starsFieldMaterial == null)       return false;
                if(m_Resources.sunMaterial == null)              return false;
                if(m_Resources.moonMaterial == null)             return false;
                if(m_Resources.atmosphereMaterial == null)       return false;
                if(m_Resources.cloudsMaterial == null)           return false;
                if(m_Resources.ambientSkyboxMaterial == null)    return false;

                #endregion

                return true;
            }
        }

        #endregion

        #region [Fields]

        [Header("Global")]

        // Skydome.
        [SerializeField] private float m_DomeRadius = 10000f;
        private float m_OldDomeRadius;

        // Global.
        public LSky_Global global = new LSky_Global();

        [Header("Deep Space")]
        [LSky_AnimationCurveRange(0.0f, 0.0f, 1.0f, 1.0f, 6)]
        [SerializeField] private AnimationCurve m_DeepSpaceExposure = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

        // Galaxy background.
        [SerializeField] private bool m_RenderGalaxyBackground = false;
        [SerializeField] private int m_GalaxyBackgroundLayerIndex = 0;
        public LSky_GalaxyBackground galaxyBackground = new LSky_GalaxyBackground();

        // Stars field.
        [SerializeField] private bool m_RenderStarsField = true;
        [SerializeField] private int m_StarsFieldLayerIndex = 0;
        public LSky_StarsFieldCubemap starsField = new LSky_StarsFieldCubemap();

        [Header("Near Space")]
        // Sun.
        [SerializeField] private bool m_RenderSun = true;
        [SerializeField] private int m_SunLayerIndex = 0;
        public LSky_Sun sun = new LSky_Sun();

        // Moon.
        [SerializeField] private bool m_RenderMoon = true;
        [SerializeField] private int m_MoonLayerIndex = 0;
        public LSky_Moon moon = new LSky_Moon();

        private Vector3 m_OldSunPos, m_OldMoonPos;

        [Header("Atmosphere")]
        [SerializeField] private bool m_RenderAtmosphere = true;
        [SerializeField] private LSky_Quality4 m_AtmosphereMeshQuality = LSky_Quality4.High;
        [SerializeField] private int m_AtmosphereLayerIndex = 0;
        [SerializeField] private bool m_SetGlobalAtmosphereParams = true;
        public LSky_AtmosphericScattering atmosphere = new LSky_AtmosphericScattering();

        [Header("Clouds")]
        [SerializeField] private bool m_RenderClouds = true;
        [SerializeField] private int m_CloudsLayerIndex = 0;
        [SerializeField, Range(0.0f, 1.0f)] private float m_CloudsDomeHeight = 1.0f;
        [SerializeField] private float m_CloudsDomeYPos = 0.0f;
        [SerializeField] private float m_CloudsOrientation = 3f;
        public LSky_Clouds clouds = new LSky_Clouds();
        
        [Header("Lighting")]
        [SerializeField] private bool m_SendSkybox = false;
        [SerializeField] private LSky_DirLightParams m_SunLightParams = new LSky_DirLightParams();
        [SerializeField] private LSky_DirLightParams m_MoonLightParams = new LSky_DirLightParams();
        [LSky_AnimationCurveRange(0.0f, 0.0f, 1.0f, 1.0f, 6)]
        [SerializeField] private AnimationCurve m_SunMoonLightFade = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);
        private float SunEvaluateTime{ get{ return (1.0f -SunDirection.y) * 0.5f; } }
        private float MoonEvaluteTime{ get { return (1.0f -MoonDirection.y) * 0.5f; } }
        public LSky_Ambient ambient = new LSky_Ambient();

        #endregion

        #region [References|Transform]

        private Transform m_Transform = null;
        private LSky_EmptyObjectInstantiate m_GalaxyBackgroundRef = new LSky_EmptyObjectInstantiate();
        private LSky_EmptyObjectInstantiate m_StarsFieldRef = new LSky_EmptyObjectInstantiate();
        private LSky_EmptyObjectInstantiate m_SunRef = new LSky_EmptyObjectInstantiate();
        private LSky_EmptyObjectInstantiate m_MoonRef = new LSky_EmptyObjectInstantiate();
        private LSky_EmptyObjectInstantiate m_AtmosphereRef = new LSky_EmptyObjectInstantiate();
        private LSky_EmptyObjectInstantiate m_CloudsRef = new LSky_EmptyObjectInstantiate();
        private LSky_DirLightIntantiate m_DirLightRef = new LSky_DirLightIntantiate();

        /// <summary></summary>
        public bool CheckRef
        {
            get
            {
                if(!m_GalaxyBackgroundRef.CheckComponents) return false;
                if(!m_StarsFieldRef.CheckComponents)       return false;
                if(!m_SunRef.CheckComponents)              return false;
                if(!m_MoonRef.CheckComponents)             return false;
                if(!m_AtmosphereRef.CheckComponents)       return false;
                if(!m_CloudsRef.CheckComponents)           return false;
                if(!m_DirLightRef.CheckComponents)         return false;

                return true;
            }
        }

        #endregion

        #region [Properties]

        /// <summary></summary>
        public Vector3 DomeRadius3D{ get{ return Vector3.one * m_DomeRadius; } }

        /// <summary></summary>
        public Vector3 SunDirection
        {
            get
            { 
                // -(SunMatrix.rotation * Vector3.forward)
                return -m_SunRef.transform.forward;
            }
        }

        /// <summary></summary>
        public Vector3 MoonDirection
        {
            get
            {
                // - MoonMatrix.rotation * Vector3.forward.
                return -m_MoonRef.transform.forward;
            }
        }

        /// <summary></summary>
        public bool IsDay
        { 
            get
            {
                if(Mathf.Abs(sun.Parameters.coords.altitude) > 1.7f)
                    return false;

                return true;
            }
        }

        /// <summary></summary>
        public bool DirLightEnbled
        {
            get
            {
                if(!IsDay && (Mathf.Abs(moon.Parameters.coords.altitude) > 1.7f))
                    return false;

                return true;
            }
        }

        /// <summary></summary>
        public float DeepSpaceExposure{ get{ return m_DeepSpaceExposure.Evaluate(SunEvaluateTime); } }

        #endregion

        #region [Methods|Initialize]

        private void Awake()
        {

            m_Transform = this.transform;

            if(!CheckResources)
            {
                enabled = false;
                m_IsReady = false;
                return;
            }
            m_IsReady = CheckResources;

            SetShadersToMaterials();

            if(!CheckRef)
            {
                BuildDome();
            }
        }

        private void Start()
        {

            ScaleDome();
            atmosphere.Initialize();

            // Initialize property ID's.
            global.InitializePropertyIDs();
            galaxyBackground.InitializePropertyIDs();
            starsField.InitializeṔropertyIDs();
            sun.InitProperyIDs();
            moon.InitPropertyIDs();
            atmosphere.InitPropertyIDs();
            clouds.InitPropertyIDs();
        }

        private void SetShadersToMaterials()
        {
            m_Resources.galaxyBackgroundMaterial.shader = m_Resources.galaxyBackgroundShader;
            m_Resources.starsFieldMaterial.shader       = m_Resources.starsFieldShader;
            m_Resources.sunMaterial.shader              = m_Resources.sunShader;
            m_Resources.moonMaterial.shader             = m_Resources.moonShader;
            m_Resources.atmosphereMaterial.shader       = m_Resources.atmosphereShader; 
            m_Resources.cloudsMaterial.shader           = m_Resources.cloudsShader;
            m_Resources.ambientSkyboxMaterial.shader    = m_Resources.ambientSkyboxShader;
        }

        private void BuildDome()
        {
           
            m_GalaxyBackgroundRef.Instantiate(this.name, "Galaxy Background Tr");
            m_GalaxyBackgroundRef.InitTransform(m_Transform, Vector3.zero);

            m_StarsFieldRef.Instantiate(this.name, "Stars Field Tr");
            m_StarsFieldRef.InitTransform(m_Transform, Vector3.zero);

            m_SunRef.Instantiate(this.name, "Sun Tr");
            m_SunRef.InitTransform(m_Transform, Vector3.zero);

            m_MoonRef.Instantiate(this.name, "Moon Tr");
            m_MoonRef.InitTransform(m_Transform, Vector3.zero);

            m_AtmosphereRef.Instantiate(this.name, "Atmosphere Tr");
            m_AtmosphereRef.InitTransform(m_Transform, Vector3.zero);

            m_CloudsRef.Instantiate(this.name, "Clouds Tr");
            m_CloudsRef.InitTransform(m_Transform, Vector3.zero);

            m_DirLightRef.InstantiateLight(this.name, "Dir Light");
            m_DirLightRef.InitTransform(m_Transform, Vector3.zero);
        }
        #endregion

        #region [Methods|Render]

        private void LateUpdate()
        { 
            UpdateDome();
        }

        public void UpdateDome()
        {
            if(!m_IsReady) return;

            if(m_SendSkybox)
            {
                UnityEngine.RenderSettings.skybox = m_Resources.ambientSkyboxMaterial;
                m_SendSkybox = false;
            }

            UpdateCelestialsTransform();
            UpdateCloudsTransform();
            RenderDome();
            UpdateLight();
            UpdateAmbient();
        }

        public void RenderDome()
        {

            if(m_OldDomeRadius != m_DomeRadius)
            {
                ScaleDome();
                m_OldDomeRadius = m_DomeRadius;
            }

            global.SetParams(m_Transform, SunDirection, MoonDirection);

            if(m_RenderGalaxyBackground)
            {
                galaxyBackground.SetParams(m_Resources.galaxyBackgroundMaterial, DeepSpaceExposure);
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_GalaxyBackgroundRef.transform.localToWorldMatrix,
                    m_Resources.galaxyBackgroundMaterial, m_GalaxyBackgroundLayerIndex
                );
            }

            if(m_RenderStarsField)
            {
                starsField.SetParams(m_Resources.starsFieldMaterial, DeepSpaceExposure);
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_StarsFieldRef.transform.localToWorldMatrix,
                    m_Resources.starsFieldMaterial,
                    m_StarsFieldLayerIndex
                );
            }

            if(m_RenderSun)
            {
                sun.SetParams(m_Resources.sunMaterial);
                Graphics.DrawMesh(
                    GetQuadMesh(),
                    m_SunRef.transform.localToWorldMatrix,
                    m_Resources.sunMaterial,
                    m_SunLayerIndex
                );
            }

            if(m_RenderMoon)
            {
                moon.SetParams(m_Resources.moonMaterial);
                Graphics.DrawMesh(
                    GetSphereMesh(LSky_Quality4.Low),
                    m_MoonRef.transform.localToWorldMatrix,
                    m_Resources.moonMaterial,
                    m_MoonLayerIndex
                );

            }

            if(m_RenderAtmosphere)
            {

                atmosphere.sunDir  = SunDirection;
                atmosphere.moonDir = MoonDirection;

                if(m_SetGlobalAtmosphereParams)
                    atmosphere.SetGlobalParams(SunEvaluateTime);
                else
                    atmosphere.SetParams(m_Resources.atmosphereMaterial, SunEvaluateTime);

                Graphics.DrawMesh(
                    GetAtmosphereMesh(m_AtmosphereMeshQuality),
                    m_AtmosphereRef.transform.localToWorldMatrix,
                    m_Resources.atmosphereMaterial,
                    m_AtmosphereLayerIndex
                );
            }

            if(m_RenderClouds)
            {
                
                if(atmosphere.moonRayleighMode == LSky_CelestialRayleighMode.CelestialContribution)
                    clouds.SetParams(m_Resources.cloudsMaterial, SunEvaluateTime, true, MoonEvaluteTime);
                else
                    clouds.SetParams(m_Resources.cloudsMaterial, SunEvaluateTime, false);

                Graphics.DrawMesh(
                    GetHemisphereMesh(LSky_Quality4.Low),
                    m_CloudsRef.transform.localToWorldMatrix,
                    m_Resources.cloudsMaterial,
                    m_CloudsLayerIndex
                );

            }
        }

        
        private void UpdateCelestialsTransform()
        {
            if(m_OldSunPos != sun.SunPosition)
            {
                m_SunRef.transform.localPosition = sun.SunPosition;
                m_SunRef.transform.LookAt(m_Transform, Vector3.forward);
                m_OldSunPos = sun.SunPosition;
            }
            m_SunRef.transform.localScale = sun.Parameters.size * Vector3.one;

            if(m_OldMoonPos != moon.MoonPosition)
            {
                m_MoonRef.transform.localPosition = moon.MoonPosition;
                m_MoonRef.transform.LookAt(m_Transform, Vector3.forward);
                m_OldMoonPos = moon.MoonPosition;
            }
            m_MoonRef.transform.localScale = moon.Parameters.size * Vector3.one;
        }

        private void UpdateCloudsTransform()
        {
            
            m_CloudsRef.transform.localPosition = new Vector3(m_CloudsRef.transform.localPosition.x, m_CloudsDomeYPos,  m_CloudsRef.transform.localPosition.z);
            m_CloudsRef.transform.localScale = new Vector3(m_CloudsRef.transform.localScale.x,m_CloudsDomeHeight,  m_CloudsRef.transform.localScale.z);

            m_CloudsRef.transform.localRotation = Quaternion.Euler(
                m_CloudsRef.transform.localEulerAngles.x, 
                m_CloudsOrientation, 
                m_CloudsRef.transform.localEulerAngles.z
            );
        }

        private void UpdateLight()
        {
            if(IsDay)
            {
                m_DirLightRef.transform.localPosition = sun.SunPosition;
                m_DirLightRef.transform.LookAt(m_Transform);
                m_DirLightRef.light.color = m_SunLightParams.color.Evaluate(SunEvaluateTime);
                m_DirLightRef.light.intensity = m_SunLightParams.intensity;
            }
            else
            {
                m_DirLightRef.transform.localPosition = moon.MoonPosition;
                m_DirLightRef.transform.LookAt(m_Transform);
                m_DirLightRef.light.color = m_MoonLightParams.color.Evaluate(MoonEvaluteTime);
                m_DirLightRef.light.intensity = m_MoonLightParams.intensity * m_SunMoonLightFade.Evaluate(SunEvaluateTime);
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

        public void SetOuterSpaceRotation(Quaternion rotation)
        {
            if(m_RenderStarsField)
            {
                m_StarsFieldRef.transform.localRotation = rotation;
            }

            if(m_RenderGalaxyBackground)
            {
                m_GalaxyBackgroundRef.transform.localRotation = rotation;
            }
        }

        #endregion

        #region [Methods|Mesh]

        private Mesh GetSphereMesh(LSky_Quality4 quality)
        {
            switch(quality)
            {
                case LSky_Quality4.Low: 
                    return m_Resources.sphereLOD3;

                case LSky_Quality4.Medium:
                    return m_Resources.sphereLOD2; 

                case LSky_Quality4.High: 
                    return m_Resources.sphereLOD1;

                case LSky_Quality4.Ultra: 
                    return m_Resources.sphereLOD0;
            }
            return null;
        }

        private Mesh GetHemisphereMesh(LSky_Quality4 quality)
        {
            switch(quality)
            {
                case LSky_Quality4.Low:
                    return m_Resources.hemisphereLOD3; 

                case LSky_Quality4.Medium: 
                    return m_Resources.hemisphereLOD2; 

                case LSky_Quality4.High: 
                    return m_Resources.hemisphereLOD1; 

                case LSky_Quality4.Ultra: 
                    return m_Resources.hemisphereLOD0; 
            }
            return null;      
        }

        private Mesh GetAtmosphereMesh(LSky_Quality4 quality)
        {
            switch(quality)
            {
                case LSky_Quality4.Low: 
                    return m_Resources.atmosphereLOD3; 

                case LSky_Quality4.Medium: 
                    return m_Resources.atmosphereLOD2; 

                case LSky_Quality4.High: 
                    return m_Resources.atmosphereLOD1;

                case LSky_Quality4.Ultra:
                    return m_Resources.atmosphereLOD0; 
            }
            return null;
        }

        private Mesh GetQuadMesh()
        {
            return m_Resources.QuadMesh;
        }

        private void ScaleDome()
        {
            m_Transform.localScale = DomeRadius3D;
        }

        #endregion
    }
}

