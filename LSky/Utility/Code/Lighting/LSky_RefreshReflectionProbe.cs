/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Refresh Reflection Probe
///----------------------------------------------
/// Description: Refresh reflection probe.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rallec.LSky.Utility
{
    [ExecuteInEditMode]
    public class LSky_RefreshReflectionProbe : LSky_Refresh
    {

        // Set Custom Render Texture.
        //--------------------------------------------------------------
        [SerializeField] protected bool m_SetRenderTexture = false;
        [SerializeField] protected RenderTexture m_RenderTexture = null;

        // Target.
        //--------------------------------------------------------------
        protected ReflectionProbe m_Probe = null;

        protected void Awake()
        {
            m_Probe             = GetComponent<ReflectionProbe>();
            m_Probe.mode        = ReflectionProbeMode.Realtime;
            m_Probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
        }

        protected override void Refresh()
        {
            if(m_SetRenderTexture)
                m_Probe.RenderProbe(m_RenderTexture);
            else
                m_Probe.RenderProbe(null);
        }  

        /// <summary></summary>
        public bool SetRenderTexture
        {
            get{ return m_SetRenderTexture; }
            set{ m_SetRenderTexture = value;}
        }

        /// <summary></summary>
        public RenderTexture RenderTexture
        {
            get{ return m_RenderTexture; }
            set{ m_RenderTexture = value;}
        }
        
    }
}
