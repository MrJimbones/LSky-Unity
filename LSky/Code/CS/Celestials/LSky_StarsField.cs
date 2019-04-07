/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Stars Field.
///----------------------------------------------
/// Description: Stars Field.
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Rallec.LSky
{
    [Serializable] public struct LSky_StarsFieldParams
    {

        // Cubemaps.
        public Cubemap cubemap;
        public Cubemap noiseCubemap;

        // Color.
        public Color tint;
        public float intensity;

        // Sincillation.
        [Range(0.0f, 1.0f)] public float scintillation;
        public float scintillationSpeed;

        public LSky_StarsFieldParams(Cubemap _cubemap, Cubemap _noiseCubemap, Color _tint, float _intensity, float _scintillation, float _scintillationSpeed)
        {
            this.cubemap       = _cubemap;
            this.noiseCubemap  = _noiseCubemap;
            this.tint          = _tint;
            this.intensity     = _intensity;
            this.scintillation = _scintillation;
            this.scintillationSpeed = _scintillationSpeed;
        }

        /// <summary></summary>
        public void Lerp(LSky_StarsFieldParams b, float time)
        {
            this.tint               = Color.Lerp(this.tint, b.tint, time);
            this.intensity          = Mathf.Lerp(this.intensity, b.intensity, time);
            this.scintillation      = Mathf.Lerp(this.scintillation, b.scintillation, time);
            this.scintillationSpeed = Mathf.Lerp(this.scintillationSpeed, b.scintillationSpeed, time);
        }

    }

    /// <summary></summary>
    [Serializable] public class LSky_StarsFieldCubemap
    {
        
        [SerializeField] private LSky_StarsFieldParams m_Parameters = new LSky_StarsFieldParams
        {
            cubemap            = null,
            noiseCubemap       = null,
            tint               = Color.white,
            intensity          = 1.0f,
            scintillation      = 1.0f,
            scintillationSpeed = 25f
        };
      
        #region [PropertyIDs]

        private int m_TintID;
        private int m_IntensityID;
        private int m_ContrastID;
        private int m_ScintillationID;
        private int m_ScintillationSpeedID;

        private int m_CubemapID;
        private int m_NoiseCubemapID;
        private int m_NoiseMatrixID;

        /// <summary></summary>
        public int CubemapID{ get{ return m_CubemapID; } }

        /// <summary></summary>
        public int NoiseCubemapID{ get{ return m_NoiseCubemapID; } }

        /// <summary></summary>
        public int NoiseMatrixID{ get{ return m_NoiseMatrixID; } }
        
        /// <summary></summary>
        public int TintID{ get{ return m_TintID; } }

        /// <summary></summary>
        public int IntensityID{ get{ return m_IntensityID; } }

        /// <summary></summary>
        public int ContrastID{ get{ return m_ContrastID; } }

        /// <summary></summary>
        public int ScitillationID{ get{ return m_ScintillationID; } }

        /// <summary></summary>
        public int ScintillationSpeedID{ get{ return m_ScintillationSpeedID; } }

        #endregion

        private float m_StarsFieldNoiseXAngle;

        /// <summary></summary>
        public void InitializeṔropertyIDs()
        {
            m_CubemapID      = Shader.PropertyToID("lsky_StarsFieldCubemap");
            m_NoiseCubemapID = Shader.PropertyToID("lsky_StarsFieldNoiseCubemap");
            m_NoiseMatrixID  = Shader.PropertyToID("lsky_StarsFieldNoiseMatrix");
            m_TintID         = Shader.PropertyToID("lsky_StarsFieldTint");
            m_IntensityID    = Shader.PropertyToID("lsky_StarsFieldIntensity");
            m_ContrastID     = Shader.PropertyToID("lsky_StarsFieldContrast");
            m_ScintillationID = Shader.PropertyToID("lsky_StarsFieldScintillation");
            m_ScintillationSpeedID = Shader.PropertyToID("lsky_StarsFieldScintillationSpeed");
        }

        /// <summary></summary>
        public void SetParams(Material material, float intensity = 1.0f)
        {
            // Set cubemap.
            material.SetTexture(m_CubemapID, m_Parameters.cubemap);

            // Set Noise Cubemap.
            material.SetTexture(m_NoiseCubemapID, m_Parameters.noiseCubemap);

            // Set color.
            material.SetColor(m_TintID, m_Parameters.tint);

            // Set intensity
            material.SetFloat(m_IntensityID, m_Parameters.intensity*intensity);

            // Set contrast.
            //material.SetFloat(m_ContrastID, m_Parameters.contrast);

            // Set Scintillation.
            material.SetFloat(m_ScintillationID, m_Parameters.scintillation);

            // Set Scintillation Speed.
            material.SetFloat(m_ScintillationSpeedID, m_Parameters.scintillationSpeed);

            // Scroll the x Axis of the noise cubemap.
            m_StarsFieldNoiseXAngle += Time.deltaTime * m_Parameters.scintillationSpeed;

            // Set noise matrix.
            Matrix4x4 starsFieldNoiseMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(m_StarsFieldNoiseXAngle, 0.0f, 0.0f), Vector3.one);
            material.SetMatrix(m_NoiseMatrixID, starsFieldNoiseMatrix);
        }
    }
}