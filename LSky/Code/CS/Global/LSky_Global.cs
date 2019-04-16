/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Global
///----------------------------------------------
/// Description: Global for sky
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{

    /// <summary></summary>
    [Serializable] public class LSky_Global
    {

        [SerializeField] private float m_GlobalExposure = 1.0f;

        public float GlobalExposure => m_GlobalExposure;

        internal readonly int m_GlobalExposurePropertyID = Shader.PropertyToID("lsky_GlobalExposure");
        internal readonly int m_WorldSunDirectionID  = Shader.PropertyToID("lsky_WorldSunDirection");
        internal readonly int m_WorldMoonDirectionID = Shader.PropertyToID("lsky_WorldMoonDirection");
        internal readonly int m_LocalSunDirectionID  = Shader.PropertyToID("lsky_LocalSunDirection");
        internal readonly int m_LocalMoonDirectionID = Shader.PropertyToID("lsky_LocalMoonDirection");
        internal readonly int m_ObjectToWorldID = Shader.PropertyToID("lsky_ObjectToWorld");
        internal readonly int m_WorldToObjectID = Shader.PropertyToID("lsky_WorldToObject");
        

        /// <summary> Set parameters to material. </summary>
        /// <param="transform"> Dome transform.  </param>
        public void SetParams(Transform transform)
        {
            Shader.SetGlobalFloat(m_GlobalExposurePropertyID, m_GlobalExposure);
            Shader.SetGlobalMatrix(m_ObjectToWorldID, transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(m_WorldToObjectID, transform.worldToLocalMatrix);
        }

        /// <summary> Set parameters to material. </summary>
        public void SetParams(Transform transform, Vector3 sunDir, Vector3 moonDir)
        {
            Shader.SetGlobalFloat(m_GlobalExposurePropertyID, m_GlobalExposure);
            Shader.SetGlobalMatrix(m_ObjectToWorldID, transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(m_WorldToObjectID, transform.worldToLocalMatrix);
            Shader.SetGlobalVector(m_WorldSunDirectionID, sunDir);
            Shader.SetGlobalVector(m_LocalSunDirectionID, transform.InverseTransformDirection(sunDir));
            Shader.SetGlobalVector(m_WorldMoonDirectionID, moonDir);
            Shader.SetGlobalVector(m_LocalMoonDirectionID, transform.InverseTransformDirection(moonDir));
        }
    }
}