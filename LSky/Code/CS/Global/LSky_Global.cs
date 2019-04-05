/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Global
///----------------------------------------------
/// Description: Global for sky
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;

namespace Rallec.LSky
{

    /// <summary></summary>
    [Serializable] public class LSky_Global
    {

        #region [Fields|General]

        public float globalExposure = 1.0f;

        #endregion

        #region [PropertyIDs]

        private int m_GlobalExposurePropertyID;
        private int m_WorldSunDirectionID, m_WorldMoonDirectionID, m_LocalSunDirectionID, m_LocalMoonDirectionID;
        private int m_ObjectToWorldID, m_WorldToObjectID;

        /// <summary></summary>
        public int GlobalExposurePropertyID{ get{ return m_GlobalExposurePropertyID; } }

        /// <summary></summary>
        public int WorldSunDirectionID{ get{ return m_WorldSunDirectionID; } }

        /// <summary></summary>
        public int WorldMoonDirectionID{ get{ return m_WorldMoonDirectionID; } }

        /// <summary></summary>
        public int LocalSunDirectionID{ get{ return m_LocalSunDirectionID; } }

        /// <summary></summary>
        public int LocalMoonDirectionID{ get{ return m_LocalMoonDirectionID; } }

        /// <summary></summary>
        public int ObjectToWorldID{ get{ return m_ObjectToWorldID; } }

        /// <summary></summary>
        public int WorldToObjectID{ get{ return m_WorldToObjectID; } }

        #endregion


        #region [Initialize]

        public void InitializePropertyIDs()
        {
            m_GlobalExposurePropertyID = Shader.PropertyToID("lsky_GlobalExposure");

            m_WorldSunDirectionID  = Shader.PropertyToID("lsky_WorldSunDirection");
            m_WorldMoonDirectionID = Shader.PropertyToID("lsky_WorldMoonDirection");
            m_LocalSunDirectionID  = Shader.PropertyToID("lsky_LocalSunDirection");
            m_LocalMoonDirectionID = Shader.PropertyToID("lsky_LocalMoonDirection");

            m_ObjectToWorldID = Shader.PropertyToID("lsky_ObjectToWorld");
            m_WorldToObjectID = Shader.PropertyToID("lsky_WorldToObject");

        }

        #endregion

        #region [Set Params]

        /// <summary></summary>
        public void SetParams(Transform transform)
        {
            // Set global exposure.
            Shader.SetGlobalFloat(m_GlobalExposurePropertyID, globalExposure);

            // Set matrices.
            Shader.SetGlobalMatrix(m_ObjectToWorldID, transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(m_WorldToObjectID, transform.worldToLocalMatrix);

        }

        /// <summary></summary>
        public void SetParams(Transform transform, Vector3 sunDir, Vector3 moonDir)
        {
            // Set global exposure.
            Shader.SetGlobalFloat(m_GlobalExposurePropertyID, globalExposure);

            // Set matrices.
            Shader.SetGlobalMatrix(m_ObjectToWorldID, transform.localToWorldMatrix);
            Shader.SetGlobalMatrix(m_WorldToObjectID, transform.worldToLocalMatrix);

            // Set sun direction.
            Shader.SetGlobalVector(m_WorldSunDirectionID, sunDir);
            Shader.SetGlobalVector(m_LocalSunDirectionID, transform.InverseTransformDirection(sunDir));


            // Set moon direction-
            Shader.SetGlobalVector(m_WorldMoonDirectionID, moonDir);
            Shader.SetGlobalVector(m_LocalMoonDirectionID, transform.InverseTransformDirection(moonDir));

        }

        #endregion

    }
}