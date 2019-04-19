/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Fog Post FX
///----------------------------------------------
/// Description: Fog
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class LSky_DistanceFogPFX : LSky_PostProcessingBase
    {

        [SerializeField] private FogMode m_FogMode = FogMode.Exponential;

        [SerializeField] private float m_Density = 0.01f;
        [SerializeField] private float m_StartDistance = 0.0f, m_EndDistance = 300f;
        [SerializeField] private float m_RayleighDepthMultiplier = 1.0f, m_SunMiePhaseDepthMultiplier = 1.0f, m_MoonMiePhaseDepthMultiplier = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_Haziness = 0.75f;
        [SerializeField] private float m_SunMiePhaseMult = 5.0f, m_MoonMiePhaseMult = 5.0f;

        internal readonly int m_FrustumCornersID = Shader.PropertyToID("lsky_FrustumCorners");
        internal readonly int m_CameraPositionID = Shader.PropertyToID("lsky_CameraPosition");
        internal readonly int m_SunMiePhaseDepthMultiplierID = Shader.PropertyToID("lsky_SunMiePhaseDepthMultiplier");
        internal readonly int m_MoonMiePhaseDepthMultiplierID = Shader.PropertyToID("lsky_MoonMiePhaseDepthMultiplier");
        internal readonly int m_RayleighDepthMultiplierID = Shader.PropertyToID("lsky_RayleighDepthMultiplier");
        internal readonly int m_FogHazinessID = Shader.PropertyToID("lsky_FogHaziness");
        internal readonly int m_FogSunMiePhaseMultID = Shader.PropertyToID("lsky_FogSunMiePhaseMult");
        internal readonly int m_FogMoonMiePhaseMultID = Shader.PropertyToID("lsky_FogMoonMiePhaseMult");

        protected override void Start()
        {
            base.Start();
            // Depth mode.
            m_Camera.depthTextureMode |= DepthTextureMode.Depth;
        }

        [ImageEffectOpaque]
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
          
            if(!m_IsReady)
            {
                Graphics.Blit(source, destination);
                enabled = false;
                return;
            }

            FXMaterial.SetMatrix(m_FrustumCornersID, FrustumCorners());
            FXMaterial.SetVector(m_CameraPositionID, m_CameraTransform.position);

            FXMaterial.SetFloat(m_SunMiePhaseDepthMultiplierID , m_SunMiePhaseDepthMultiplier);
            FXMaterial.SetFloat(m_MoonMiePhaseDepthMultiplierID , m_MoonMiePhaseDepthMultiplier);
            FXMaterial.SetFloat(m_RayleighDepthMultiplierID, m_RayleighDepthMultiplier);
            FXMaterial.SetFloat(m_FogHazinessID, m_Haziness);
            FXMaterial.SetFloat(m_FogSunMiePhaseMultID, m_SunMiePhaseMult);
            FXMaterial.SetFloat(m_FogMoonMiePhaseMultID, m_MoonMiePhaseMult);

            int pass = 0;

            switch(m_FogMode)
            {
                case FogMode.Linear:

                    pass = 0;

                    Vector2 lp;
                    lp.x = m_StartDistance; 
                    lp.y = m_EndDistance; 
                    FXMaterial.SetVector("lsky_LinearParams", lp);

                break;

                case FogMode.Exponential:

                    pass = 1;

                    float densityExp = m_Density * 1.4426950408f;
                    FXMaterial.SetFloat("lsky_DensityExp", densityExp);

                break;

                case FogMode.ExponentialSquared:

                    pass = 2;

                    float densityExp2 = m_Density * 1.2011224087f;
                    FXMaterial.SetFloat("lsky_DensityExp", densityExp2);

                break;

            }

            CustomBlit(source, destination, FXMaterial, pass);
        }
    }
}