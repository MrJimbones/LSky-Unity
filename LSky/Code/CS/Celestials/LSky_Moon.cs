/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Near Space.
///----------------------------------------------
/// Moon
///----------------------------------------------
/// Description: Moon
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{

    /// <summary></summary>
    [Serializable] public class LSky_MoonParams
    {

        public Texture2D tex;
        public Vector2 texOffsets;
        public Color tint;
        public float intensity;
        public float contrast;

        /// <summary></summary>
        public void Lerp(LSky_MoonParams b, float time)
        {
            this.tint = Color.Lerp(this.tint, b.tint, time);
            this.intensity = Mathf.Lerp(this.intensity, b.intensity, time);
            this.contrast = Mathf.Lerp(this.contrast, b.contrast, time);
        }
    }
    
    /// <summary></summary>
    [Serializable] public class LSky_Moon
    {
        
        public LSky_MoonParams parameters = new LSky_MoonParams
        {
            tex        = null,
            texOffsets = Vector2.zero,
            tint       = Color.white,
            intensity  = 1.0f,
            contrast   = 0.3f
        };

        internal readonly int m_TexID       = Shader.PropertyToID("lsky_MoonTex");
        internal readonly int m_TintID      = Shader.PropertyToID("lsky_MoonTint");
        internal readonly int m_IntensityID = Shader.PropertyToID("lsky_MoonIntensity");
        internal readonly int m_ContrastID  = Shader.PropertyToID("lsky_MoonContrast");
        
        /// <summary></summary>
        public void SetParams(Material material)
        {
            material.SetTexture(m_TexID, parameters.tex);
            material.SetTextureOffset(m_TexID, parameters.texOffsets);
            material.SetColor(m_TintID, parameters.tint);
            material.SetFloat(m_IntensityID, parameters.intensity);
            material.SetFloat(m_ContrastID, parameters.contrast);
        }


    }
    
}
