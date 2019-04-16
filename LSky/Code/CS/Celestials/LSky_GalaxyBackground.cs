/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Deep Space
///----------------------------------------------
/// Galaxy background.
///----------------------------------------------
/// Description: Galaxy background(milky way,etc).
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace LSky
{
    /// <summary></summary>
    [Serializable] public class LSky_GalaxyBackgroundParams
    {

        public Cubemap cubemap;
        public Color tint;
        public float intensity;
        [Range(0.0f, 1.0f)] public float contrast;

        /// <summary></summary>
        public void Lerp(LSky_GalaxyBackgroundParams b, float time)
        {
           this.tint      = Color.Lerp(tint, b.tint, time);
           this.intensity = Mathf.Lerp(intensity, b.intensity, time);
           this.contrast  = Mathf.Lerp(contrast, b.intensity, time);
        }
    }
    /// <summary></summary>
    [Serializable] public class LSky_GalaxyBackground
    {

        public LSky_GalaxyBackgroundParams parameters = new LSky_GalaxyBackgroundParams
        {
            cubemap   = null,
            tint      = Color.white,
            intensity = 1.0f,
            contrast  = 0.3f
        };

        internal readonly int  m_CubemapID   = Shader.PropertyToID("lsky_GalaxyBackgroundCubemap");
        internal readonly int  m_TintID      = Shader.PropertyToID("lsky_GalaxyBackgroundTint");
        internal readonly int  m_IntensityID = Shader.PropertyToID("lsky_GalaxyBackgroundIntensity");
        internal readonly int  m_ContrastID  = Shader.PropertyToID("lsky_GalaxyBackgroundContrast");
     
        /// <summary> Set parameters to material. </summary>
        public void SetParams(Material material, float intensity = 1f)
        {
            material.SetTexture(m_CubemapID, parameters.cubemap);
            material.SetColor(m_TintID, parameters.tint);
            material.SetFloat(m_IntensityID, parameters.intensity * intensity);
            material.SetFloat(m_ContrastID, parameters.contrast);
        }

    }

}

