/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Stars Field.
///----------------------------------------------
/// Description: Stars Field.
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace LSky
{
    [Serializable] public class LSky_StarsFieldParams
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
        
        public LSky_StarsFieldParams parameters = new LSky_StarsFieldParams
        {
            cubemap            = null,
            noiseCubemap       = null,
            tint               = Color.white,
            intensity          = 1.0f,
            scintillation      = 1.0f,
            scintillationSpeed = 25f
        };

        private float m_StarsFieldNoiseXAngle;

        internal readonly int m_CubemapID      = Shader.PropertyToID("lsky_StarsFieldCubemap");
        internal readonly int m_NoiseCubemapID = Shader.PropertyToID("lsky_StarsFieldNoiseCubemap");
        internal readonly int m_NoiseMatrixID  = Shader.PropertyToID("lsky_StarsFieldNoiseMatrix");
        internal readonly int m_TintID         = Shader.PropertyToID("lsky_StarsFieldTint");
        internal readonly int m_IntensityID    = Shader.PropertyToID("lsky_StarsFieldIntensity");
        internal readonly int m_ContrastID     = Shader.PropertyToID("lsky_StarsFieldContrast");
        internal readonly int m_ScintillationID = Shader.PropertyToID("lsky_StarsFieldScintillation");
        internal readonly int m_ScintillationSpeedID = Shader.PropertyToID("lsky_StarsFieldScintillationSpeed");
        
        /// <summary></summary>
        public void SetParams(Material material, float intensity = 1.0f)
        {

            material.SetTexture(m_CubemapID, parameters.cubemap);
            material.SetTexture(m_NoiseCubemapID, parameters.noiseCubemap);
            material.SetColor(m_TintID, parameters.tint);
            material.SetFloat(m_IntensityID, parameters.intensity*intensity);
            material.SetFloat(m_ScintillationID, parameters.scintillation);
            material.SetFloat(m_ScintillationSpeedID, parameters.scintillationSpeed);

            // Scroll the x Axis of the noise cubemap.
            m_StarsFieldNoiseXAngle += Time.deltaTime * parameters.scintillationSpeed;
            Matrix4x4 starsFieldNoiseMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(m_StarsFieldNoiseXAngle, 0.0f, 0.0f), Vector3.one);
            material.SetMatrix(m_NoiseMatrixID, starsFieldNoiseMatrix);
        }
    }
}