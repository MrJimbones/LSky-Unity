/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Time Of Day
///----------------------------------------------
/// Time Of Day Manager.
///----------------------------------------------
/// Description: Time OF Day Manager.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using LSky.Utility;

namespace LSky
{
    [RequireComponent(typeof(LSky_Dome)), ExecuteInEditMode]
    public class LSky_TimeOfDay : LSky_DateTimeManager
    {
        [SerializeField] private LSky_Dome m_Dome = null;
        [SerializeField] private LSky_PlanetaryPositions m_Planetary = new LSky_PlanetaryPositions();

        /// <summary></summary>
        public override bool IsDay
        {
            get{ return m_Dome.IsDay; }
        }

        /// <summary></summary>
        public Quaternion OuterSpaceRotation
        {
            get
            {
                return Quaternion.Euler(90 - m_Planetary.latitude, 0.0f, 0.0f) * 
                Quaternion.Euler(0.0f, m_Planetary.longitude, 0.0f) * 
                Quaternion.Euler(0.0f, m_Planetary.LocalSideralTime * Mathf.Rad2Deg, 0.0f);
            }
        }

        protected override void Awake()
        {
            base.Awake();
            m_Dome = GetComponent<LSky_Dome>();
        }

        protected override void Update()
        {
            base.Update();
            m_Planetary.dateTime = LSky_DateTime.DateTimeToRGKDateTime(SystemDateTime);

            m_Planetary.ComputeSunCoords();
            m_Planetary.ComputeMoonCoords();

            m_Dome.SunCoords  = m_Planetary.SunCoords;
            m_Dome.MoonCoords = m_Planetary.MoonCoords;

            m_Dome.SetOuterSpaceRotation(OuterSpaceRotation);
        }

    }
}