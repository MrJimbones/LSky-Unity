/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Time Of Day.
///----------------------------------------------
/// Date Time.
///----------------------------------------------
/// Description: Date Time Structure.
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace LSky
{

    /// <summary>
    /// Date Time.
    /// Total hours, minutes, seconds, milliseconds, day, month, year.
    /// </summary>
    public partial struct LSky_DateTime
    {

        public float totalHours;
        public int hour, minute, second, millisecond;
        public int day, month, year;

        public LSky_DateTime(float _totalHours, int _hour, int _minute, int _second, int _millisecond, int _day, int _month, int _year)
        {

            this.totalHours  = _totalHours;
            this.hour        = _hour;
            this.minute      = _minute;
            this.second      = _second;
            this.millisecond = _millisecond;
            this.day         = _day;
            this.month       = _month;
            this.year        = _year;
        }

        /// <summary>Get System.DateTime</summary>
        /// <param name="dateTime">System.DateTime</param>
        public void SetSystemDateTime(System.DateTime dateTime)
        {
            totalHours  = (float)dateTime.TimeOfDay.TotalHours;
            hour        = dateTime.Hour;
            minute      = dateTime.Minute;
            second      = dateTime.Second;
            millisecond = dateTime.Millisecond;
            day         = dateTime.Day;
            month       = dateTime.Month;
            year        = dateTime.Year;
        }

        /// <summary> Convert System.DateTime in RGK_DateTime </summary>
        /// <param name="dateTime"> System.DateTime </param>
        public static LSky_DateTime DateTimeToRGKDateTime(System.DateTime dateTime)
        {
            return new LSky_DateTime
            {
                totalHours  = (float)dateTime.TimeOfDay.TotalHours,
                hour        = dateTime.Hour,
                minute      = dateTime.Minute,
                second      = dateTime.Second,
                millisecond = dateTime.Millisecond,
                day         = dateTime.Day,
                month       = dateTime.Month,
                year        = dateTime.Year
            };
        }

        /// <summary> Convert hour in float value. </summary>
        /// <param name="hour"> Hour </param>
        /// <returns> Total hours in float value </returns>
        public static float GetTotalHours(int hour)
        {
            return (float)hour;
        }

        /// <summary> 
        /// Convert hours and minutes in float value.
        /// </summary>
        /// <param name="hour"> Hour </param>
        /// <param name="minute"> Minute </param>
        /// <returns> Total hours in float value </returns>
        public static float GetTotalHours(int hour, int minute)
        {
            return (float)hour + ((float)minute / 60f);
        }

        /// <summary>
        /// Convert hours, minutes and seconds in float value.
        /// </summary>
        /// <param name="hour"> Hour </param>
        /// <param name="minute"> Minute </param>
        /// <param name="second"> Second </param>
        /// <returns> Total hours in float value </returns>
        public static float GetTotalHours(int hour, int minute, int second)
        {
            return (float)hour + ((float)minute / 60f) + ((float)second / 3600f);
        }

        /// <summary>
        /// Convert hours, minutes, seconds and milliseconds in float value.
        /// </summary>
        /// <param name="hour"> Hour </param>
        /// <param name="minute"> Minute </param>
        /// <param name="second"> Second </param>
        /// <param name="millisecond"> Millisecond </param>
        /// <returns> Toltal hours in float value. </returns>
        public static float GetTotalHours(int hour, int minute, int second, int millisecond)
        {
            return (float)hour + (float)minute / 60f + (float)second / 3600f + (float)millisecond / 3600000f;
        }
    }
}