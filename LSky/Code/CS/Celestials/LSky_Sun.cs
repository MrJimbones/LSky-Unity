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
using Rallec.LSky.Utility;

namespace Rallec.LSky
{

    [Serializable] public struct LSky_SunParams
    {
        // Coordinates.
        public LSky_CelestialsCoords coords;
        public float size;

        // Textures.
        public Texture2D tex;

        // Color.
        public Color tint;
        public float intensity;
        
        public LSky_SunParams(LSky_CelestialsCoords _coords, float _size, Texture2D _tex, Color _tint, float _intensity)
        {
            this.coords    = _coords;
            this.size      = _size;
            this.tex       = _tex;
            this.tint      = _tint;
            this.intensity = _intensity;
        }

        /// <summary></summary>
        public void Lerp(LSky_SunParams b, float time)
        {
            this.size = Mathf.Lerp(this.size, b.size, time);
            this.tint = Color.Lerp(this.tint, b.tint, time);
            this.intensity = Mathf.Lerp(this.intensity, b.intensity, time);
        }
        
    }

    [Serializable] public class LSky_Sun
    {

        [SerializeField] private LSky_SunParams m_Parameters = new LSky_SunParams
        {
            coords = LSky_CelestialsCoords.Zero,
            size = 0.1f,
            tex = null,
            tint = Color.white,
            intensity = 1.0f
        };

        /// <summary></summary>
        public LSky_SunParams Parameters
        {
            get{ return m_Parameters; }
            set{ m_Parameters = value; }
        }

        public Vector3 SunPosition{ get{ return LSky_Mathf.SphericalToCartesian(m_Parameters.coords.altitude, m_Parameters.coords.azimuth); } }

        #region [PropertyIDs]

        private int m_SunTexID;
        private int m_SunTintID;
        private int m_SunIntensityID;

        /// <summary></summary>
        public int SunTexID{ get{ return m_SunTexID; } }

        /// <summary></summary>
        public int SunTintID{ get{ return m_SunTintID; } }

        /// <summary></summary>
        public int SunIntensityID{ get{ return m_SunIntensityID; } }

        /// <summary></summary>
        public void InitProperyIDs()
        {
            m_SunTexID       = Shader.PropertyToID("lsky_StarTex");
            m_SunTintID      = Shader.PropertyToID("lsky_StarTint");
            m_SunIntensityID = Shader.PropertyToID("lsky_StarIntensity");
        }

        #endregion

        #region [SetParams]

        public void SetParams(Material material)
        {
            material.SetTexture(SunTexID, m_Parameters.tex);
            material.SetColor(SunTintID, m_Parameters.tint);
            material.SetFloat(SunIntensityID, m_Parameters.intensity);
        }

        #endregion
    }

}