/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Clouds
///----------------------------------------------
/// Description: Clouds
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;

namespace Rallec.LSky
{
    /// <summary></summary>
    [Serializable] public struct LSky_CloudsParams
    {
        // Texture.
        public Texture2D tex;
        public Vector2 texSize;
        public Vector2 texOffset;

        // Color.
        public Gradient tint, moonTint;
        public float intensity;

        // Density.
        public float density;
        [Range(0.0f, 1.0f)] public float coverage;

        public float speed, speed2;

        public LSky_CloudsParams(
            Texture2D _tex, Vector2 _texSize, Vector2 _texOffset, Gradient _tint, Gradient _moonTint,
            float _intensity, float _density, float _coverage, float _speed, float _speed2)
        {

            this.tex       = _tex;
            this.texSize   = _texSize;
            this.texOffset = _texOffset;
            this.tint      = _tint;
            this.moonTint  = _moonTint;
            this.intensity = _intensity;
            this.density   = _density;
            this.coverage  = _coverage;
            this.speed     = _speed;
            this.speed2    = _speed2;
            
        }

    }

    /// <summary></summary>
    [Serializable] public class LSky_Clouds
    {

        [SerializeField] private LSky_CloudsParams m_Parameters = new LSky_CloudsParams
        {
            tex       = null,
            texSize   = Vector2.one,
            texOffset = Vector2.zero,
            tint      = new Gradient(),
            moonTint  = new Gradient(),
            intensity = 1.0f,
            density   = 0.3f,
            coverage  = 0.5f,
            speed     = 0.01f,
            speed2    = 0.05f
        };

        #region [PropertyIDs]

        private int m_TexID, m_TintID, m_IntensityID, m_DensityID, m_CoverageID, m_SpeedID, m_Speed2ID;

        /// <summary></summary>
        public int TexID{ get{ return m_TexID; } }

        /// <summary></summary>
        public int TintID{ get{ return m_TintID; } }

        /// <summary></summary>
        public int IntensityID{ get{ return m_IntensityID; } }

        /// <summary></summary>
        public int DensityID{ get{ return m_DensityID; } }

        /// <summary></summary>
        public int CoverageID{ get{ return m_CoverageID; } }

        /// <summary></summary>
        public int SpeedID{ get{ return m_SpeedID; } }

        /// <summary></summary>
        public int Speed2ID{ get{ return m_Speed2ID; } }

        /// <summary></summary>
        public void InitPropertyIDs()
        {
            m_TexID       = Shader.PropertyToID("lsky_CloudsTex");
            m_TintID      = Shader.PropertyToID("lsky_CloudsTint");
            m_IntensityID = Shader.PropertyToID("lsky_CloudsIntensity");
            m_DensityID   = Shader.PropertyToID("lsky_CloudsDensity");
            m_CoverageID  = Shader.PropertyToID("lsky_CloudsCoverage");
            m_SpeedID    = Shader.PropertyToID("lsky_CloudsSpeed");
            m_Speed2ID    = Shader.PropertyToID("lsky_CloudsSpeed2");
        }

        #endregion

        #region [SetParams]

        public void SetParams(Material material, float evaluateTime, bool evaluateMoon, float moonEvaluateTime = 1.0f)
        {
            material.SetTexture(m_TexID, m_Parameters.tex);
            material.SetTextureScale(m_TexID, m_Parameters.texSize);
            material.SetTextureOffset(m_TexID, m_Parameters.texOffset);

            Color col = evaluateMoon ? m_Parameters.tint.Evaluate(evaluateTime) + m_Parameters.moonTint.Evaluate(moonEvaluateTime) : 
                m_Parameters.tint.Evaluate(evaluateTime);

            material.SetColor(m_TintID, col);
            material.SetFloat(m_IntensityID, m_Parameters.intensity);

            material.SetFloat(m_DensityID, m_Parameters.density);
            material.SetFloat(m_CoverageID, m_Parameters.coverage);

            material.SetFloat(m_SpeedID, m_Parameters.speed);
            material.SetFloat(m_Speed2ID, m_Parameters.speed2);
        }

        #endregion
    } 
}