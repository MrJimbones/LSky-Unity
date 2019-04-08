/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Lighting
///----------------------------------------------
/// Description: Lighting
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;
using UnityEngine.Rendering;

namespace Rallec.LSky
{

    /// <summary></summary>
    [Serializable] public struct LSky_DirLightParams
    {

        public float intensity;
        public Gradient color;

        public LSky_DirLightParams(float _intensity, Gradient _color)
        {
            this.intensity = _intensity;
            this.color = _color;
        }
    }

    /// <summary></summary>
    [Serializable] public struct LSky_AmbientParams
    {

        public UnityEngine.Rendering.AmbientMode ambientMode;

        public float updateTime;
        public Gradient skyColor, equatorColor, groundColor;
        public Gradient skyMoonColor, equatorMoonColor, groundMoonColor;

        public LSky_AmbientParams(
            UnityEngine.Rendering.AmbientMode _ambientMode, float _updateTime,
            Gradient _skyColor, Gradient _equatorColor, Gradient _groundColor,
            Gradient _skyMoonColor, Gradient _equatorMoonColor, Gradient _groundMoonColor
        )
        {
            this.ambientMode  = _ambientMode;
            this.updateTime   = _updateTime;
            this.skyColor     = _skyColor;
            this.equatorColor = _equatorColor;
            this.groundColor  = _groundColor;
            this.skyMoonColor = _skyMoonColor;
            this.equatorMoonColor = _equatorMoonColor;
            this.groundMoonColor  = _groundMoonColor;
        }

    }

    /// <summary></summary>
    [Serializable] public class LSky_Ambient
    {

        [SerializeField] private LSky_AmbientParams m_AmbientParams = new LSky_AmbientParams
        {
            ambientMode  = UnityEngine.Rendering.AmbientMode.Flat,
            updateTime   = 0.5f,
            skyColor     = new Gradient(),
            equatorColor = new Gradient(),
            groundColor  = new Gradient(),
            skyMoonColor     = new Gradient(),
            equatorMoonColor = new Gradient(),
            groundMoonColor  = new Gradient()
        };

        private float m_AmbientRefreshTimer;

        /// <summary></summary>
        public void UpdateAmbient(float evaluateTime, bool evaluateMoon, float moonEvaluateTime = 1.0f)
        {
            RenderSettings.ambientMode = m_AmbientParams.ambientMode;

            m_AmbientRefreshTimer += Time.deltaTime;
            if(m_AmbientParams.updateTime >= m_AmbientRefreshTimer)
            {

                switch(m_AmbientParams.ambientMode)
                {
                    case UnityEngine.Rendering.AmbientMode.Flat:

                    RenderSettings.ambientSkyColor = m_AmbientParams.skyColor.Evaluate(evaluateTime);

                    // Add moon contribution.
                    if(evaluateMoon)
                        RenderSettings.ambientSkyColor += m_AmbientParams.skyMoonColor.Evaluate(moonEvaluateTime);

                    break;

                    case UnityEngine.Rendering.AmbientMode.Trilight:

                    RenderSettings.ambientSkyColor     = m_AmbientParams.skyColor.Evaluate(evaluateTime);
                    RenderSettings.ambientEquatorColor = m_AmbientParams.equatorColor.Evaluate(evaluateTime);
                    RenderSettings.ambientGroundColor  = m_AmbientParams.groundColor.Evaluate(evaluateTime);

                    // Add moon contribution.
                    if(evaluateMoon)
                    {
                        RenderSettings.ambientSkyColor     += m_AmbientParams.skyMoonColor.Evaluate(moonEvaluateTime);
                        RenderSettings.ambientEquatorColor += m_AmbientParams.equatorMoonColor.Evaluate(moonEvaluateTime);
                        RenderSettings.ambientGroundColor  += m_AmbientParams.groundMoonColor.Evaluate(moonEvaluateTime);
                    }

                    break;

                    case UnityEngine.Rendering.AmbientMode.Skybox:

                    DynamicGI.UpdateEnvironment(); // Is low.

                    break;

                }
                m_AmbientRefreshTimer = 0.0f;
            }
        }

        /// <summary></summary>
        public LSky_AmbientParams AmbientParams
        {
            get
            {
                return m_AmbientParams;
            }
            set
            {
                m_AmbientParams.ambientMode  = value.ambientMode;
                m_AmbientParams.updateTime   = value.updateTime;
                m_AmbientParams.skyColor     = value.skyColor;
                m_AmbientParams.equatorColor = value.equatorColor;
                m_AmbientParams.groundColor  = value.groundColor;

            }
        }

    }

}