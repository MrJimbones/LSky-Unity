/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Camera
///----------------------------------------------
/// Description: LSKy Camera
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    [ExecuteInEditMode]
    public class LSky_Camera : MonoBehaviour
    {

        private Camera m_Cam = null;
        private Transform m_Transform = null;

        [SerializeField] private LSky_Dome m_Dome = null;

        private void Awake()
        {   
            m_Cam = GetComponent<Camera>();
            m_Transform = this.transform;
            m_Cam.clearFlags = CameraClearFlags.SolidColor;
            m_Cam.backgroundColor = Color.black;
        }

        private void LateUpdate()
        {

            if(m_Dome != null)
            {
                m_Dome._Transform.position = m_Transform.position + m_Transform.rotation * Vector3.one;
            }

        }

    }

}

