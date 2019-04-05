/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Dome.
///----------------------------------------------
/// Description: Skydome
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

        [SerializeField] private LSky_DomeResources m_Resources = null;

        [SerializeField] private bool m_IsReady = false;
        public bool CheckResources
        {
            get
            {
                #region [Mesh]
                // Check this resource.
                if(m_Resources == null) return false;
                
                // Check sphere.
                if(m_Resources.sphereLOD0 == null) return false;
                if(m_Resources.sphereLOD1 == null) return false;
                if(m_Resources.sphereLOD2 == null) return false;
                if(m_Resources.sphereLOD3 == null) return false;

                // Check hemisphere.
                if(m_Resources.hemisphereLOD0 == null) return false;
                if(m_Resources.hemisphereLOD1 == null) return false;
                if(m_Resources.hemisphereLOD2 == null) return false;
                if(m_Resources.hemisphereLOD3 == null) return false;

                // Check atmosphere.
                if(m_Resources.atmosphereLOD0 == null) return false;
                if(m_Resources.atmosphereLOD1 == null) return false;
                if(m_Resources.atmosphereLOD2 == null) return false;
                if(m_Resources.atmosphereLOD3 == null) return false;

                // Check quad
                if(m_Resources.QuadMesh == null) return false;
                #endregion

                #region [Shaders]

                // Check atmosphere
                if(m_Resources.atmosphereShader == null) return false;

                // Check deep space.
                if(m_Resources.galaxyBackgroundShader == null) return false;
                if(m_Resources.starsFieldShader == null) return false;

                // Check sun.
                if(m_Resources.sunShader == null) return false;

                // Check moon.
                if(m_Resources.moonShader == null) return false;

                // Ambient skybox
                //if(m_Resources.ambientSkyboxShader == null) return false;

                #endregion

                #region [Material]

                // Check atmosphere.
                if(m_Resources.atmosphereMaterial == null) return false;
                
                // Check deep space.
                if(m_Resources.galaxyBackgroundMaterial == null) return false;
                if(m_Resources.starsFieldMaterial == null) return false;

                // Check sun.
                if(m_Resources.sunMaterial == null) return false;

                // Check moon.
                if(m_Resources.moonMaterial == null) return false;

                // Ambient skybox.
                //if(m_Resources.ambientSkyboxMaterial == null) return false;

                #endregion


                return true;
            }
        }

        #endregion

        #region [Fields]

        // Skydome.
        [SerializeField] private float m_DomeRadius = 10000f;

        // Global.
        public LSky_Global global = new LSky_Global();

        // Galaxy background.
        [SerializeField] private bool m_RenderGalaxyBackground = false;
        [SerializeField] private int m_GalaxyBackgroundLayerIndex = 0;
        public LSky_GalaxyBackground galaxyBackground = new LSky_GalaxyBackground();

        // Stars field.
        [SerializeField] private bool m_RenderStarsField = true;
        [SerializeField] private int m_StarsFieldLayerIndex = 0;
        public LSky_StarsFieldCubemap starsField = new LSky_StarsFieldCubemap();

        // Sun.
        [SerializeField] private bool m_RenderSun = true;
        [SerializeField] private int m_SunLayerIndex = 0;
        public LSky_Sun sun = new LSky_Sun();

        // Moon.
        [SerializeField] private bool m_RenderMoon = true;
        [SerializeField] private int m_MoonLayerIndex = 0;
        public LSky_Moon moon = new LSky_Moon();

        // Atmosphere.
        [SerializeField] private bool m_RenderAtmosphere = true;
        [SerializeField] private LSky_Quality4 m_AtmosphereMeshQuality = LSky_Quality4.High;
        [SerializeField] private int m_AtmosphereLayerIndex = 0;
        [SerializeField] private bool m_SetGlobalAtmosphereParams = true;
        public LSky_AtmosphericScattering atmosphere = new LSky_AtmosphericScattering();

        private float m_OldDomeRadius;

        #endregion

        #region [References|Transform]

        // Dome global transform.
        private Transform m_Transform = null;

        // Galaxy background.
        private LSky_EmptyObjectInstantiate m_GalaxyBackgroundRef = new LSky_EmptyObjectInstantiate();

        // Stars field.
        private LSky_EmptyObjectInstantiate m_StarsFieldRef = new LSky_EmptyObjectInstantiate();

        // Atmosphere.
        private LSky_EmptyObjectInstantiate m_AtmosphereRef = new LSky_EmptyObjectInstantiate();

        // Sun.
        private LSky_EmptyObjectInstantiate m_SunRef = new LSky_EmptyObjectInstantiate();

        // Moon.
        private LSky_EmptyObjectInstantiate m_MoonRef = new LSky_EmptyObjectInstantiate();


        public bool CheckRef
        {
            get
            {
                if(!m_GalaxyBackgroundRef.CheckComponents) return false;
                if(!m_StarsFieldRef.CheckComponents) return false;
                if(!m_AtmosphereRef.CheckComponents) return false;
                if(!m_SunRef.CheckComponents) return false;
                if(!m_MoonRef.CheckComponents) return false;

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

        #endregion

        #region [Methods|Initialize]

        private void Awake()
        {
            // Cache transform.
            m_Transform = this.transform;

            // Check resources.
            if(!CheckResources)
            {
                enabled = false;
                return;
            }
            m_IsReady = CheckResources;

            // Set shaders to respective materials.
            SetShadersToMaterials();

            // Build dome.
            if(!CheckRef)
            {
                BuildDome();
            }
        }

        private void Start()
        {

            // Init dome size.
            ScaleDome();

            // Initialize
            atmosphere.Initialize();

            // Initialize property ID's.
            global.InitializePropertyIDs();
            galaxyBackground.InitializePropertyIDs();
            starsField.InitializeṔropertyIDs();
            sun.InitProperyIDs();
            moon.InitPropertyIDs();
            atmosphere.InitPropertyIDs();
           

        }

        private void SetShadersToMaterials()
        {
            // if(!CheckResources) return;

            // Set galaxy background.
            m_Resources.galaxyBackgroundMaterial.shader = m_Resources.galaxyBackgroundShader;

            // Set stars field.
            m_Resources.starsFieldMaterial.shader = m_Resources.starsFieldShader;

            // Set sun.
            m_Resources.sunMaterial.shader = m_Resources.sunShader;

            // Set moon.
            m_Resources.moonMaterial.shader = m_Resources.moonShader;

            // Set atmosphere shader.
            m_Resources.atmosphereMaterial.shader = m_Resources.atmosphereShader; 

        }

        private void BuildDome()
        {
            // Galaxy background.
            m_GalaxyBackgroundRef.Instantiate(this.name, "Galaxy Background Tr");
            m_GalaxyBackgroundRef.InitTransform(m_Transform, Vector3.zero);

            // Stars field.
            m_StarsFieldRef.Instantiate(this.name, "Stars Field Tr");
            m_StarsFieldRef.InitTransform(m_Transform, Vector3.zero);

            // Atmosphere.
            m_AtmosphereRef.Instantiate(this.name, "Atmosphere Tr");
            m_AtmosphereRef.InitTransform(m_Transform, Vector3.zero);

            // Sun-
            m_SunRef.Instantiate(this.name, "Sun Tr");
            m_SunRef.InitTransform(m_Transform, Vector3.zero);

            // Moon.
            m_MoonRef.Instantiate(this.name, "Moon Tr");
            m_MoonRef.InitTransform(m_Transform, Vector3.zero);

        }

        #endregion

        #region [Methods|Render]

        void LateUpdate()
        {
            RenderDome();
        }

        public void RenderDome()
        {
            if(!m_IsReady) return;

            // Update Dome Size.
            if(m_OldDomeRadius != m_DomeRadius)
            {
                ScaleDome(); // Debug.Log("Escale");
                m_OldDomeRadius = m_DomeRadius;
            }

            // Global Params.
            global.SetParams(m_Transform, SunDirection, MoonDirection);

            // Render galaxy background.
            if(m_RenderGalaxyBackground)
            {
                // Set params.
                galaxyBackground.SetParams(m_Resources.galaxyBackgroundMaterial);

                // Draw mesh.
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_GalaxyBackgroundRef.transform.localToWorldMatrix,
                    m_Resources.galaxyBackgroundMaterial, m_GalaxyBackgroundLayerIndex
                );
            }

            // Render stars field.
            if(m_RenderStarsField)
            {
                // Set params.
                starsField.SetParams(m_Resources.starsFieldMaterial);

                // Draw mesh.
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_StarsFieldRef.transform.localToWorldMatrix,
                    m_Resources.starsFieldMaterial,
                    m_StarsFieldLayerIndex
                );
            }

            // Sun Position.
            m_SunRef.transform.localPosition = sun.SunPosition;
            m_SunRef.transform.LookAt(m_Transform, Vector3.forward);

            // Render sun.
            if(m_RenderSun)
            {

                m_SunRef.transform.localScale = sun.Size * Vector3.one;

                // Set params.
                sun.SetParams(m_Resources.sunMaterial);

                // Draw Mesh.
                Graphics.DrawMesh(
                    GetQuadMesh(),
                    m_SunRef.transform.localToWorldMatrix,
                    m_Resources.sunMaterial,
                    m_SunLayerIndex
                );
            }

            // Moon position.
            m_MoonRef.transform.localPosition = moon.MoonPosition;
            m_MoonRef.transform.LookAt(m_Transform, Vector3.forward);

            // Render moon.
            if(m_RenderMoon)
            {
                // Set size.
                m_MoonRef.transform.localScale = moon.Size * Vector3.one;

                // Set params.
                moon.SetParams(m_Resources.moonMaterial);

                // Draw mesh.
                Graphics.DrawMesh(
                    GetSphereMesh(LSky_Quality4.Low),
                    m_MoonRef.transform.localToWorldMatrix,
                    m_Resources.moonMaterial,
                    m_MoonLayerIndex
                );

            }

            // Render atmosphere.
            if(m_RenderAtmosphere)
            {

                atmosphere.sunDir = SunDirection;
                atmosphere.moonDir = MoonDirection;

                // Set params.
                if(m_SetGlobalAtmosphereParams)
                    atmosphere.SetGlobalParams();
                else
                    atmosphere.SetParams(m_Resources.atmosphereMaterial);

                // Draw mesh.
                Graphics.DrawMesh(
                    GetAtmosphereMesh(m_AtmosphereMeshQuality),
                    m_AtmosphereRef.transform.localToWorldMatrix,
                    m_Resources.atmosphereMaterial,
                    m_AtmosphereLayerIndex
                );
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

