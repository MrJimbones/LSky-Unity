/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Fog Post FX
///----------------------------------------------
/// Description: Fog
/////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;

namespace Rallec.LSky
{
    [ExecuteInEditMode, ImageEffectAllowedInSceneView]
    public class LSky_DistanceFogPFX : LSky_PostProcessingBase
    {

        [SerializeField] private FogMode m_FogMode = FogMode.Exponential;

        [SerializeField] private float m_Density = 0.01f;
        [SerializeField] private float m_StartDistance = 0.0f, m_EndDistance = 300f;
        [SerializeField] private float m_RayleighDepthMultiplier = 1.0f, m_SunMiePhaseDepthMultiplier = 1.0f, m_MoonMiePhaseDepthMultiplier = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_Haziness = 0.75f;

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

            FXMaterial.SetMatrix("lsky_FrustumCorners", FrustumCorners());
            FXMaterial.SetVector("lsky_CameraPosition", m_CameraTransform.position);

            FXMaterial.SetFloat("lsky_SunMiePhaseDepthMultiplier" , m_SunMiePhaseDepthMultiplier);
            FXMaterial.SetFloat("lsky_MoonMiePhaseDepthMultiplier" , m_MoonMiePhaseDepthMultiplier);
            FXMaterial.SetFloat("lsky_RayleighDepthMultiplier", m_RayleighDepthMultiplier);
            FXMaterial.SetFloat("lsky_FogHaziness", m_Haziness);

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