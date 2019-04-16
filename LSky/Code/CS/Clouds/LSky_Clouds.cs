/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Clouds
///----------------------------------------------
/// Description: Clouds
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    /// <summary></summary>
    [Serializable] public class LSky_CloudsParams
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

        internal readonly int m_TexID       = Shader.PropertyToID("lsky_CloudsTex");
        internal readonly int m_TintID      = Shader.PropertyToID("lsky_CloudsTint");
        internal readonly int m_IntensityID = Shader.PropertyToID("lsky_CloudsIntensity");
        internal readonly int m_DensityID   = Shader.PropertyToID("lsky_CloudsDensity");
        internal readonly int m_CoverageID  = Shader.PropertyToID("lsky_CloudsCoverage");
        internal readonly int m_SpeedID     = Shader.PropertyToID("lsky_CloudsSpeed");
        internal readonly int m_Speed2ID    = Shader.PropertyToID("lsky_CloudsSpeed2");
        
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
    } 
}