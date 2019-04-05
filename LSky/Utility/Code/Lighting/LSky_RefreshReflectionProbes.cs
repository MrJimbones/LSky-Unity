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
  
    public class LSky_RefreshRefelctionProbes : LSky_Refresh
    {

        [SerializeField] private ReflectionProbe[] m_Probes = new ReflectionProbe[0];

        private void Awake()
        {
            for(int i = 0; i < m_Probes.Length; i++)
            {
                m_Probes[i].mode        = ReflectionProbeMode.Realtime;
                m_Probes[i].refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            }
        }

        protected override void Refresh()
        {
            for(int i = 0; i < m_Probes.Length; i++)
            {
                m_Probes[i].RenderProbe();
            }
        }

    }
}