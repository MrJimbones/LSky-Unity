/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Near Space.
///----------------------------------------------
/// Sun.
///----------------------------------------------
/// Description: Sun.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{

    [Serializable] public class LSky_SunParams
    {

        public Texture2D tex;
        public Color tint;
        public float intensity;
        
        /// <summary></summary>
        public void Lerp(LSky_SunParams b, float time)
        {
            this.tint = Color.Lerp(this.tint, b.tint, time);
            this.intensity = Mathf.Lerp(this.intensity, b.intensity, time);
        }    
    }

    [Serializable] public class LSky_Sun
    {

        public LSky_SunParams parameters = new LSky_SunParams
        {
            tex = null,
            tint = Color.white,
            intensity = 1.0f
        };

        // Shader property ids.
        internal readonly int m_SunTexID       = Shader.PropertyToID("lsky_StarTex");
        internal readonly int m_SunTintID      = Shader.PropertyToID("lsky_StarTint");
        internal readonly int m_SunIntensityID = Shader.PropertyToID("lsky_StarIntensity");

        public void SetParams(Material material)
        {
            material.SetTexture(m_SunTexID, parameters.tex);
            material.SetColor(m_SunTintID, parameters.tint);
            material.SetFloat(m_SunIntensityID, parameters.intensity);
        }
    }

}