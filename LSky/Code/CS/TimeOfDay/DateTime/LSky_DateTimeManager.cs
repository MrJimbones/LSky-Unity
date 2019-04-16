/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Time Of Day
///----------------------------------------------
/// Date Time Manager.
///----------------------------------------------
/// Description: Date Time Manager.
/// This system may be replaced in the future.
/////////////////////////////////////////////////

using System;
using UnityEngine;
using UnityEngine.Events;
using LSky.Utility;

namespace LSky
{
    [ExecuteInEditMode][AddComponentMenu("LSky/TimeOfDay/DateTime/DateTime Manager")]
    public class LSky_DateTimeManager : MonoBehaviour
    {

        #region [Fields]
        // Time.
        //----------------------------------------------------------------
        [SerializeField] protected bool m_AllowProgressTime = true;
        [SerializeField] protected float m_TotalHours = 7.5f;

        [SerializeField, Range(0, 24)] protected int m_Hour   = 7;
        [SerializeField, Range(0, 60)] protected int m_Minute = 30;
        [SerializeField, Range(0, 60)] protected int m_Second = 0;
        [SerializeField, Range(0, 1000)] protected int m_Millisecond = 0;

        // Length.
        //----------------------------------------------------------------
        // Use to set the length of the day and night.
        [SerializeField] protected bool m_EnableDayNightLength = true;
        [SerializeField] protected bool m_CustomDayNightLength = false;

        // Star and end hour of the day.
        [SerializeField] protected Vector2 m_DayRange = new Vector2(4.5f, 19f);

        // Day in minutes.
        [SerializeField] protected float m_DayLength = 15f;

        // Night in minutes.
        [SerializeField] protected float m_NightLength = 7.5f;

        // Date.
        //-----------------------------------------------------------------
        [SerializeField, Range(1, 31)] protected int m_Day    = 10;
        [SerializeField, Range(1, 12)] protected int m_Month  = 3;
        [SerializeField, Range(0, 9999)] protected int m_Year = 2019;

        // System.
        //-----------------------------------------------------------------
        // Synchronize with this system date and time.
        [SerializeField] protected bool m_SyncWithThisSystem = false;

        public const int k_TotalHours = 24;

        #endregion

        #region [Properties]

        /// <summary> Get System DateTime. </summary>
        /// <returns> A: This system DateTime, B: System DateTime. </returns>
        public DateTime SystemDateTime
        {
            get
            {
                if(m_SyncWithThisSystem)
                {

                    // Get this system DateTime.
                    DateTime dateTime = DateTime.Now;

                    // Get total hours.
                    if(m_AllowProgressTime)
                        m_TotalHours = (float)dateTime.TimeOfDay.TotalHours;
                    
                    // Get date.
                    m_Year  = dateTime.Year;
                    m_Month = dateTime.Month;
                    m_Day   = dateTime.Day;

                    // Get time.
                    m_Hour        = dateTime.Hour;
                    m_Minute      = dateTime.Minute;
                    m_Second      = dateTime.Second;
                    m_Millisecond = dateTime.Millisecond;

                    return dateTime;
                }
                else
                {
                    DateTime dateTime = new DateTime(0, DateTimeKind.Utc);

                    // Repeat full date cycle for prevent exccess.
                    RepeatDateTimeCycle();

                    // Set date and time in System.DateTime
                    dateTime = dateTime.AddYears(m_Year - 1).AddMonths(m_Month - 1).AddDays(m_Day -1).AddHours(m_TotalHours);

                    // Get Date.
                    m_Year  = dateTime.Year;
                    m_Month = dateTime.Month;
                    m_Day   = dateTime.Day;

                    // Get total hours.
                    m_TotalHours = (float)dateTime.Hour + (float)dateTime.Minute / 60f + (float)dateTime.Second / 3600f + (float)dateTime.Millisecond / 3600000f;

                    return dateTime;
                }
            }
        }

        protected bool m_IsDay;

        /// <summary> Indicates if it is day based on System.DateTime. </summary>
        public virtual bool IsDay
        {
            get
            {
                if(!m_CustomDayNightLength)
                    m_IsDay = (m_TotalHours >= m_DayRange.x && m_TotalHours < m_DayRange.y);

                return m_IsDay;
            }
            set
            {   
                if(m_CustomDayNightLength)
                    m_IsDay = value;
            }
        }

        public float DurationCycle
        {
            get
            {
                if(m_EnableDayNightLength)
                    return IsDay ? 60 * m_DayLength * 2 : 60 * m_NightLength * 2;
                else
                    return m_DayLength * 60;
            }
        }

        #endregion

        #region [Events]

        [SerializeField] protected LSky_EventType m_CheckEventsType = LSky_EventType.Unity;

        // Unity Events.
        //-------------------------------------------------------------------------------

        // They are triggered when the values of time and date change.
        public UnityEvent unity_OnHourChanged,
        unity_OnMinuteChanged, unity_OnDayChanged,
        unity_OnMonthChanged,  unity_OnYearChanged;

        // Delegate Events.
        //-------------------------------------------------------------------------------
        public delegate void OnDateTimeChanged();
        public static event OnDateTimeChanged OnHourChanged, OnMinuteChanged, OnDayChanged, OnMonthChanged, OnYearChanged;

        // They are used to trigger events.
        //--------------------------------------------------------------------------------
        protected int m_LastHour, m_LastMinute, m_LastDay, m_LastMonth, m_LastYear;

        #endregion

        #region [Methods|Initialized]

        protected virtual void Awake()
        {
            Init();
        }

        private void Init()
        {

            // Initialize timeline.
            //------------------------------------------------

            m_TotalHours = m_SyncWithThisSystem ? (float)SystemDateTime.TimeOfDay.TotalHours : m_TotalHours;

            // Initialize TotalHours.
            //-----------------------------------------------------------------------------------------------
            // SetHour(SystemDateTime.Hour);
            // SetMinute(SystemDateTime.Minute);
            // SetSecond(SystemDateTime.Second);
            // SetMillisecond(SystemDateTime.Millisecond);
            SetTotalHours(SystemDateTime);

            // Initialize Last Date.
            //-----------------------------------------------------------------------------------------------
            m_LastYear   = SystemDateTime.Year;
            m_LastMonth  = SystemDateTime.Month;
            m_LastDay    = SystemDateTime.Day;
            m_LastHour   = SystemDateTime.Hour;
            m_LastMinute = SystemDateTime.Minute;
        }

        #endregion


        #region [Methods|Update]

        protected virtual void Update(){ InternalUpdate(); }

        public void InternalUpdate()
        {

            // Pregress time.
            if(m_AllowProgressTime && !m_SyncWithThisSystem)
            {
                m_TotalHours += (DurationCycle != 0 && Application.isPlaying) ? 
                    (Time.deltaTime / DurationCycle) * k_TotalHours : 0.0f;
            }
            
            CheckEvents();
        }

        #endregion

        #region [Methods|SetDateTime]

        /// <summary> Set Hour to Timeline. </summary>
        /// <param name= "value"> Hour </param>
        public void SetHour(int value)
        {
            if(value >= 0 && value < 25)
                m_TotalHours = LSky_DateTime.GetTotalHours(value, SystemDateTime.Minute, SystemDateTime.Second, SystemDateTime.Millisecond);
        }

        /// <summary> Set Minute to Timeline. </summary>
        /// <param name= "value"> Minute </param>
        public void SetMinute(int value)
        {
            if(value >= 0 && value < 61)
                m_TotalHours = LSky_DateTime.GetTotalHours(SystemDateTime.Hour, value, SystemDateTime.Second, SystemDateTime.Millisecond);
        }

        /// <summary> Set Second to Timeline. </summary>
        /// <param name= "value"> Second </param>
        public void SetSecond(int value)
        {
            if (value >= 0 && value < 61)
                m_TotalHours = LSky_DateTime.GetTotalHours(SystemDateTime.Hour, SystemDateTime.Minute, value, SystemDateTime.Millisecond);
        }

        /// <summary> Set Millisecond to Timeline. </summary>
        /// <param name="value"> Millisecond </param>
        public void SetMillisecond(int value)
        {
            if(value >= 0 && value < 1001)
                m_TotalHours = LSky_DateTime.GetTotalHours(SystemDateTime.Hour, SystemDateTime.Minute, SystemDateTime.Second, value);
        }

        /// <summary> Set time to timeline(totalHours). </summary>
        /// <param name="hour"> Hour </param>
        /// <param name="minute"> Minute </param>
        /// <param name="second"> Second </param>
        public void SetTotalHours(int hour, int minute, int second)
        {
            m_TotalHours = LSky_DateTime.GetTotalHours(hour, minute, second, SystemDateTime.Millisecond);
        }

        /// <summary> Set time to timeline(totalHours). </summary>
        /// <param name="hour"> Hour </param>
        /// <param name="minute"> Minute </param>
        /// <param name="second"> Second </param>
        /// <param name="milliSecond"> Millisecond </param>
        public void SetTotalHours(int hour, int minute, int second, int millisecond)
        {
            m_TotalHours = LSky_DateTime.GetTotalHours(hour, minute, second, millisecond);
        }

        /// <summary> Set System DateTime to timeline(totalHours). </summary>
        /// <param name="dateTime"> System DateTime </param>
        public void SetTotalHours(System.DateTime dateTime)
        {
            m_TotalHours = LSky_DateTime.GetTotalHours(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
        }

        #endregion

        #region [Methods|Cycle]

        private void RepeatDateTimeCycle()
        {

            // Fordward.
            //---------------------------------
            if(m_Year == 9999 && m_Month == 12 && m_Day == 31 && m_TotalHours >= 23.999999f)
            {
                m_Year = 1; m_Month = 1; m_Day = 1; m_TotalHours = 0.0f;
            }

            // Backward.
            //---------------------------------
            if(m_Year == 1 && m_Month == 1 && m_Day == 1 && m_TotalHours < 0.0f)
            {
                m_Year = 9999; m_Month = 12; m_Day = 31; m_TotalHours = 23.999999f;
            }
        } 
        #endregion

        #region [Methods|Events]

        private void CheckEvents()
        {
            switch(m_CheckEventsType)
            {
                case LSky_EventType.Unity:     CheckUnityEvents();    break;
                case LSky_EventType.Delegates: CheckDelegateEvents(); break;
                case LSky_EventType.Both:
                CheckUnityEvents();
                CheckDelegateEvents();
                break;
            }
        }

        private void CheckUnityEvents()
        {

            if(m_LastHour != SystemDateTime.Hour)
            {
                unity_OnHourChanged.Invoke();   // Debug.Log("OnHour");
                m_LastHour = SystemDateTime.Hour;
            }
           
            if(m_LastMinute != SystemDateTime.Minute)
            {
                unity_OnMinuteChanged.Invoke(); // Debug.Log("OnMinute");
                m_LastMinute = SystemDateTime.Minute;
            }

            if(m_LastDay != SystemDateTime.Day)
            {
                unity_OnDayChanged.Invoke(); //Debug.Log("OnDay");
                m_LastDay = SystemDateTime.Day;
            }

            if(m_LastMonth != SystemDateTime.Month)
            {
                unity_OnMonthChanged.Invoke(); //Debug.Log("OnMonth");
                m_LastMonth = SystemDateTime.Month;
            }

            if(m_LastYear != SystemDateTime.Year)
            {
                unity_OnYearChanged.Invoke(); //Debug.Log("OnYear");
                m_LastYear = SystemDateTime.Year;
            }

        }

        private void CheckDelegateEvents()
        {

            if(m_LastHour != SystemDateTime.Hour)
            {
                OnHourChanged();   // Debug.Log("OnHour");
                m_LastHour = SystemDateTime.Hour;
            }
           
            if(m_LastMinute != SystemDateTime.Minute)
            {
                OnMinuteChanged(); // Debug.Log("OnMinute");
                m_LastMinute = SystemDateTime.Minute;
            }

            if(m_LastDay != SystemDateTime.Day)
            {
                OnDayChanged(); //Debug.Log("OnDay");
                m_LastDay = SystemDateTime.Day;
            }

            if(m_LastMonth != SystemDateTime.Month)
            {
                OnMonthChanged(); //Debug.Log("OnMonth");
                m_LastMonth = SystemDateTime.Month;
            }

            if(m_LastYear != SystemDateTime.Year)
            {
                OnYearChanged(); //Debug.Log("OnYear");
                m_LastYear = SystemDateTime.Year;
            }

        }

        #endregion

        #region [Properties|Accessors]

        // Timeline.
        //---------------------------------------------
        /// <summary> Allow progress time. </summary>
        public bool AllowProgressTime
        { 
            get{ return m_AllowProgressTime;  }
            set{ m_AllowProgressTime = value; }
        }

        /// <summary> Time in float value. </summary>
        public float TotalHours
        {
            get{ return m_TotalHours; }
            set
            {
                if(value > 0.0f && value < 24.000001f && !m_SyncWithThisSystem)
                    m_TotalHours = value;
            }
        }

        /// <summary> Hour of day</summary>
        public int Hour
        {
            get{ return SystemDateTime.Hour; }
            set
            {
                if(value < 25 && value > 0)
                {
                    m_Hour = value;
                    SetHour(m_Hour);
                }
            }
        }

        /// <summary> Minute of day </summary>
        public int Minute
        {
            get{ return SystemDateTime.Minute; }
            set
            {
                if(value > 0 && value < 61)
                {
                    m_Minute = value;
                    SetMinute(m_Minute);
                }
            }
        }

        /// <summary> Second of day </summary>
        public int Second
        {
            get{ return SystemDateTime.Second; }
            set
            {
                if(value > 0 && value < 61)
                {
                    m_Second = value;
                    SetSecond(m_Second);
                }
            }
        }

        /// <summary> Millisecond </summary>
        public int Millisecond
        {
            get{ return SystemDateTime.Millisecond; }
            set
            {
                if(value > 0 && value < 1001)
                {
                    m_Millisecond = value;
                    SetMillisecond(m_Millisecond);
                }
            }
        }

        // Length
        //---------------------------------------------------------------------------
        /// <summary> Enable day/night length. </summary>
        public bool EnableDayNightLength
        {
            get{ return m_EnableDayNightLength;  }
            set{ m_EnableDayNightLength = value; }
        }
	
        /// <summary></summary>
        public Vector2 DayRange
        {
            get{ return m_DayRange; }
            set{ m_DayRange = value; }
        }      

        /// <summary> Duration day in minutes. </summary>
        public float DayLength
        {
            get{ return m_DayLength; }
            set{ m_DayLength = value; }
        }

        /// <summary> Duration night in minutes. </summary>
        public float NightLength
        {
            get{ return m_NightLength; }
            set{ m_NightLength = value; }
        }

        // Date.
        //---------------------------------------------
        /// <summary> Day of the year Range: [1-31] </summary>
        public int Day
        {
            get{ return m_Day; }
            set
            {
                if(value > 0 && value < 32 && !m_SyncWithThisSystem)
                    m_Day = value;
            }
        }

        /// <summary> Month Range: [1-12] </summary>
        public int Month
        {
            get{ return m_Month; }
            set
            {
                if(value > 0 && value < 13 && !m_SyncWithThisSystem)
                    m_Month = value;
            }
        }

        /// <summary> Year Range: [1-9999]. </summary>
        public int Year
        {
            get{ return m_Year; }
            set
            {
                if(value > 0 && value < 10000 && !m_SyncWithThisSystem)
                    m_Year = value;
            }
        }

        // System.
        //---------------------------------------------
        /// <summary> Syncronize with the system date time. </summary>
        public bool SyncWithThisSystem
        {
            get{ return m_SyncWithThisSystem; }
            set{ m_SyncWithThisSystem = value; }
        }

        // Events.
        //---------------------------------------------
        /// <summary></summary>
        public int LastHour{get{return m_LastHour;}}

        /// <summary></summary>
        public int LastMinute{get{return m_LastMinute;}}

        /// <summary></summary>
        public int LastDay{get{return m_LastDay;}}

        /// <summary></summary>
        public int LastMonth{get{return m_LastMonth;}}

        /// <summary></summary>
        public int LastYear{get{return m_LastYear;}}

        #endregion
    }
}
