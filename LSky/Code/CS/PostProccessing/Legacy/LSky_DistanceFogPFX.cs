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

        [SerializeField] private float m_Density = 0.01f;
        [SerializeField] private float m_RayleighDepthMultiplier = 1.0f, m_SunMiePhaseDepthMultiplier = 1.0f, m_MoonMiePhaseDepthMultiplier = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float m_Haziness = 0.75f;

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

            float densityExp = m_Density * 1.4426950408f;

            FXMaterial.SetFloat("lsky_DensityExp", densityExp);

            CustomBlit(source, destination, FXMaterial, 0);

        }


    }
}