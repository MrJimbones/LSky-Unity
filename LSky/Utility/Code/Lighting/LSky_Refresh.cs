/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Refresh base.
///----------------------------------------------
/// Description: Refresh function in defined time.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace LSky.Utility
{

    public abstract class LSky_Refresh : MonoBehaviour
    {

        [SerializeField] protected float m_RefreshTime = 0.5f;

        protected float m_Timer = 0.0f;

        protected virtual void Update()
        {
            m_Timer += Time.deltaTime;
            
            if(m_Timer >= m_RefreshTime)
            {
                Refresh();
                m_Timer = 0;
            }
           
        }

        /// <summary> Refresh in defined time </summary>
        protected abstract void Refresh();

        /// <summary> Refresh Time. </summary>
        public float RefreshTime
        {
            get{ return m_RefreshTime;  }
            set{ m_RefreshTime = value; }

        }

    }

}
