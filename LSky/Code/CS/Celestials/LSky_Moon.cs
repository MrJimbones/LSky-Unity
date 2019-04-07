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
using Rallec.LSky.Utility;

namespace Rallec.LSky
{

    /// <summary></summary>
    [Serializable] public struct LSky_MoonParams
    {

        public LSky_CelestialsCoords coords;
        public float size;

        public Texture2D tex;
        public Vector2 texOffsets;
        public Color tint;
        public float intensity;
        public float contrast;

        public LSky_MoonParams(LSky_CelestialsCoords _coords, float _size, Texture2D _tex, Vector2 _texOffsets, Color _tint, float _intensity, float _contrast)
        {
            this.coords     = _coords;
            this.size       = _size;
            this.tex        = _tex;
            this.texOffsets = _texOffsets;
            this.tint       = _tint;
            this.intensity  = _intensity;
            this.contrast   = _contrast;
        }

        /// <summary></summary>
        public void Lerp(LSky_MoonParams b, float time)
        {
            this.size = Mathf.Lerp(this.size, b.size, time);
            this.tint = Color.Lerp(this.tint, b.tint, time);
            this.intensity = Mathf.Lerp(this.intensity, b.intensity, time);
            this.contrast = Mathf.Lerp(this.contrast, b.contrast, time);
        }
    }
    
    /// <summary></summary>
    [Serializable] public class LSky_Moon
    {
        
        [SerializeField] private LSky_MoonParams m_Parameters = new LSky_MoonParams
        {
            coords = LSky_CelestialsCoords.Zero,
            size   = 0.3f,
            tex    = null,
            texOffsets = Vector2.zero,
            tint       = Color.white,
            intensity  = 1.0f,
            contrast   = 0.3f
        };

        /// <summary></summary>
        public Vector3 MoonPosition
        { 
            get
            { 
                return LSky_Mathf.SphericalToCartesian(m_Parameters.coords.altitude, m_Parameters.coords.azimuth);
            }
        } 

        #region [PropertyIDs]

        private int m_TexID, m_TexOffsetID, m_TintID, m_IntensityID, m_ContrastID;

        /// <summary></summary>
        public int TexID{ get{ return m_TexID; } }

        /// <summary></summary>
        public int TintID{ get{ return m_TintID; } }

        /// <summary></summary>
        public int IntensityID{ get{ return m_IntensityID; } }

        /// <summary></summary>
        public int ContrastID{ get{ return m_ContrastID; } }

        /// <summary></summary>
        public void InitPropertyIDs()
        {
            m_TexID       = Shader.PropertyToID("lsky_MoonTex");
            m_TintID      = Shader.PropertyToID("lsky_MoonTint");
            m_IntensityID = Shader.PropertyToID("lsky_MoonIntensity");
            m_ContrastID  = Shader.PropertyToID("lsky_MoonContrast");
        }

        #endregion

        #region [SetParams]

        public void SetParams(Material material)
        {
            material.SetTexture(m_TexID, m_Parameters.tex);
            material.SetTextureOffset(m_TexID, m_Parameters.texOffsets);
            material.SetColor(m_TintID, m_Parameters.tint);
            material.SetFloat(m_IntensityID, m_Parameters.intensity);
            material.SetFloat(m_ContrastID, m_Parameters.contrast);
        }

        #endregion

        #region [Accessors]

        /// <summary></summary>
        public float Size{ get{ return m_Parameters.size; } }

        /// <summary></summary>
        public LSky_CelestialsCoords Coords{ get{ return m_Parameters.coords; } }

        #endregion

    }


    
}
