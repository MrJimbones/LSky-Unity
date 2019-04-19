/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Dome.
///----------------------------------------------
/// Description: Skydome.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    [ExecuteInEditMode]
    public partial class LSky_Dome : MonoBehaviour
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

        [Header("SKY DOME SETTINGS")]

        #region [General Dome Settings]

        [SerializeField] private float m_DomeRadius = 10000f;
        private float m_OldDomeRadius;

        #endregion

        #region [Deep Space]

        [Header("Dome: Deep Space")]
        [SerializeField] private bool m_RenderGalaxyBackground = false;
        [SerializeField] private int m_GalaxyBackgroundLayerIndex = 0;

        [SerializeField] private bool m_RenderStarsField = true;
        [SerializeField] private int m_StarsFieldLayerIndex = 0;

        #endregion

        #region [Near Space]

        [Header("Dome: Near Space")]
        [SerializeField] private bool m_RenderSun = true;
        [SerializeField] private int m_SunLayerIndex = 0;
        [SerializeField] private LSky_CelestialsCoords m_SunCoords = LSky_CelestialsCoords.Zero;
        [SerializeField] private float m_SunMeshSize = 0.005f;

        [SerializeField] private bool m_RenderMoon = true;
        [SerializeField] private int m_MoonLayerIndex = 0;
        [SerializeField] private LSky_CelestialsCoords m_MoonCoords = LSky_CelestialsCoords.Zero;
        [SerializeField] private float m_MoonMeshSize = 0.05f;

        private Vector3 m_OldSunPos, m_OldMoonPos;

        #endregion

        #region [Atmosphere]

        [Header("Dome: Atmosphere")]
        [SerializeField] private bool m_RenderAtmosphere = true;
        [SerializeField] private LSky_Quality4 m_AtmosphereMeshQuality = LSky_Quality4.High;
        [SerializeField] private int m_AtmosphereLayerIndex = 0;

        #endregion

        #region [Clouds]

        [Header("Dome: Clouds")]
        [SerializeField] private bool m_RenderClouds = true;
        [SerializeField] private int m_CloudsLayerIndex = 0;
        [SerializeField, Range(0.0f, 1.0f)] private float m_CloudsDomeHeight = 1.0f;
        [SerializeField] private float m_CloudsDomeYPos = 0.0f;
        [SerializeField] private float m_CloudsOrientation = 3f;

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
        public Vector3 DomeRadius3D => Vector3.one * m_DomeRadius;

        /// <summary> Get sun direction: SunMatrix * Vector3.forward. </summary>
        public Vector3 SunDirection => -m_SunRef.transform.forward;

        /// <summary> Get moon direction: MoonMatrix * Vector3.forward. </summary>
        public Vector3 MoonDirection => -m_MoonRef.transform.forward;

        /// <summary></summary>
        public Vector3 SunPosition => LSky_Mathf.SphericalToCartesian(m_SunCoords.altitude, m_SunCoords.azimuth);

        /// <summary></summary>
        public Vector3 MoonPosition => LSky_Mathf.SphericalToCartesian(m_MoonCoords.altitude, m_MoonCoords.azimuth);
        
        /// <summary></summary>
        public bool IsDay 
        {
            get
            {
                if(Mathf.Abs(m_SunCoords.altitude) > 1.7f)
                    return false;

                return true;
            }
        }

        /// <summary></summary>
        public bool DirLightEnbled
        {
            get
            {
                if(!IsDay && (Mathf.Abs(m_MoonCoords.altitude) > 1.7f))
                    return false;

                return true;
            }
        }
        #endregion


        #region [Initialize]

        private void Initialize()
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
            ScaleDome();
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


        #region [Transform]

        private void ScaleDome()
        {
            m_Transform.localScale = DomeRadius3D;
        }

        private void UpdateCelestialsTransform()
        {
            if(m_OldSunPos != SunPosition)
            {
                m_SunRef.transform.localPosition = SunPosition;
                m_SunRef.transform.LookAt(m_Transform, Vector3.forward);
                m_OldSunPos = SunPosition;
            }
            m_SunRef.transform.localScale = m_SunMeshSize * Vector3.one;

            if(m_OldMoonPos != MoonPosition)
            {
                m_MoonRef.transform.localPosition = MoonPosition;
                m_MoonRef.transform.LookAt(m_Transform, Vector3.forward);
                m_OldMoonPos = MoonPosition;
            }
            m_MoonRef.transform.localScale = m_MoonMeshSize * Vector3.one;
        }

        /// <summary></summary>
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

        #endregion

        #region [Render]

        private void RenderDome()
        {
            if(m_OldDomeRadius != m_DomeRadius)
            {
                ScaleDome();
                m_OldDomeRadius = m_DomeRadius;
            }

            if(m_RenderGalaxyBackground)
            {
                
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_GalaxyBackgroundRef.transform.localToWorldMatrix,
                    m_Resources.galaxyBackgroundMaterial, m_GalaxyBackgroundLayerIndex
                );
            }

            if(m_RenderStarsField)
            {
                Graphics.DrawMesh(
                    m_Resources.sphereLOD3,
                    m_StarsFieldRef.transform.localToWorldMatrix,
                    m_Resources.starsFieldMaterial,
                    m_StarsFieldLayerIndex
                );
            }

            if(m_RenderSun)
            {
                
                Graphics.DrawMesh(
                    GetQuadMesh(),
                    m_SunRef.transform.localToWorldMatrix,
                    m_Resources.sunMaterial,
                    m_SunLayerIndex
                );
            }

            if(m_RenderMoon)
            {
                
                Graphics.DrawMesh(
                    GetSphereMesh(LSky_Quality4.Low),
                    m_MoonRef.transform.localToWorldMatrix,
                    m_Resources.moonMaterial,
                    m_MoonLayerIndex
                );
            }

            if(m_RenderAtmosphere)
            {

                Graphics.DrawMesh(
                    GetAtmosphereMesh(m_AtmosphereMeshQuality),
                    m_AtmosphereRef.transform.localToWorldMatrix,
                    m_Resources.atmosphereMaterial,
                    m_AtmosphereLayerIndex
                );
            }

            if(m_RenderClouds)
            {
                
                Graphics.DrawMesh(
                    GetHemisphereMesh(LSky_Quality4.Low),
                    m_CloudsRef.transform.localToWorldMatrix,
                    m_Resources.cloudsMaterial,
                    m_CloudsLayerIndex
                );

            }
        }

        #endregion

        #region [Mesh]

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

        private Mesh GetQuadMesh() => m_Resources.QuadMesh;

        #endregion

        #region [Accessors]

        public LSky_CelestialsCoords SunCoords
        {
            get{ return m_SunCoords; }
            set{ m_SunCoords = value; }
        }

        public LSky_CelestialsCoords MoonCoords
        {
            get{ return m_MoonCoords; }
            set{ m_MoonCoords = value; }
        }

        public Transform _Transform
        {
            get => m_Transform;
            set => m_Transform = value;
        }

        #endregion
    }
}