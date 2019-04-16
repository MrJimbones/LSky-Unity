/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Lighting
///----------------------------------------------
/// Description: Lighting
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;
using UnityEngine.Rendering;

namespace LSky
{

    /// <summary></summary>
    [Serializable] public class LSky_DirLightParams
    {

        public float intensity;
        public Gradient color;
    }

    /// <summary></summary>
    [Serializable] public class LSky_AmbientParams
    {

        public UnityEngine.Rendering.AmbientMode ambientMode;

        public float updateTime;
        public Gradient skyColor, equatorColor, groundColor;
        public Gradient skyMoonColor, equatorMoonColor, groundMoonColor;
    }

    /// <summary></summary>
    [Serializable] public class LSky_Ambient
    {

        public LSky_AmbientParams ambientParams = new LSky_AmbientParams
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
            RenderSettings.ambientMode = ambientParams.ambientMode;

            m_AmbientRefreshTimer += Time.deltaTime;
            if(ambientParams.updateTime >= m_AmbientRefreshTimer)
            {

                switch(ambientParams.ambientMode)
                {
                    case UnityEngine.Rendering.AmbientMode.Flat:

                    RenderSettings.ambientSkyColor = ambientParams.skyColor.Evaluate(evaluateTime);

                    // Add moon contribution.
                    if(evaluateMoon)
                        RenderSettings.ambientSkyColor += ambientParams.skyMoonColor.Evaluate(moonEvaluateTime);

                    break;

                    case UnityEngine.Rendering.AmbientMode.Trilight:

                    RenderSettings.ambientSkyColor     = ambientParams.skyColor.Evaluate(evaluateTime);
                    RenderSettings.ambientEquatorColor = ambientParams.equatorColor.Evaluate(evaluateTime);
                    RenderSettings.ambientGroundColor  = ambientParams.groundColor.Evaluate(evaluateTime);

                    // Add moon contribution.
                    if(evaluateMoon)
                    {
                        RenderSettings.ambientSkyColor     += ambientParams.skyMoonColor.Evaluate(moonEvaluateTime);
                        RenderSettings.ambientEquatorColor += ambientParams.equatorMoonColor.Evaluate(moonEvaluateTime);
                        RenderSettings.ambientGroundColor  += ambientParams.groundMoonColor.Evaluate(moonEvaluateTime);
                    }

                    break;

                    case UnityEngine.Rendering.AmbientMode.Skybox:

                    if(Application.isPlaying)
                        DynamicGI.UpdateEnvironment(); // Is slow.

                    break;

                }
                m_AmbientRefreshTimer = 0.0f;
            }
        }

    }

}