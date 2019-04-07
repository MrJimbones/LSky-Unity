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

namespace Rallec.LSky
{
    /// <summary></summary>
    [Serializable] public struct LSky_GalaxyBackgroundParams
    {

        public Cubemap cubemap;
        public Color tint;
        public float intensity;

        [Range(0.0f, 1.0f)] public float contrast;

        public LSky_GalaxyBackgroundParams(Cubemap _cubemap, Color _tint, float _intensity, float _contrast)
        {
            this.cubemap   = _cubemap;
            this.tint      = _tint;
            this.intensity = _intensity;
            this.contrast  = _contrast;
        }

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

        [SerializeField] LSky_GalaxyBackgroundParams m_Parameters = new LSky_GalaxyBackgroundParams
        {
            cubemap   = null,
            tint      = Color.white,
            intensity = 1.0f,
            contrast  = 0.3f
        };

        #region [PropertyIDs]

        private int m_CubemapID;
        private int m_TintID;
        private int m_IntensityID;
        private int m_ContrastID;

        /// <summary></summary>
        public int CubemapID{ get{ return m_CubemapID; } }

        /// <summary></summary>
        public int TintID{ get{ return m_TintID; } }

        /// <summary></summary>
        public int IntensityID{ get{ return m_IntensityID; } }

        /// <summary></summary>
        public int ContrastID{ get{ return m_ContrastID; } }

        #endregion

        /// <summary></summary>
        public void InitializePropertyIDs()
        {
            m_CubemapID   = Shader.PropertyToID("lsky_GalaxyBackgroundCubemap");
            m_TintID      = Shader.PropertyToID("lsky_GalaxyBackgroundTint");
            m_IntensityID = Shader.PropertyToID("lsky_GalaxyBackgroundIntensity");
            m_ContrastID  = Shader.PropertyToID("lsky_GalaxyBackgroundContrast");
        }

        /// <summary> Set parameters to material. </summary>
        public void SetParams(Material material, float intensity = 1f)
        {
            material.SetTexture(m_CubemapID, m_Parameters.cubemap);
            material.SetColor(m_TintID, m_Parameters.tint);
            material.SetFloat(m_IntensityID, m_Parameters.intensity * intensity);
            material.SetFloat(m_ContrastID, m_Parameters.contrast);
        }

        /// <summary></summary>
        public LSky_GalaxyBackgroundParams Parameters
        {
            get
            {
                return m_Parameters;
            }
            set
            {
                m_Parameters.cubemap   = value.cubemap;
                m_Parameters.tint      = value.tint;
                m_Parameters.intensity = value.intensity;
                m_Parameters.contrast  = value.contrast;
            }
        }

    }

}

