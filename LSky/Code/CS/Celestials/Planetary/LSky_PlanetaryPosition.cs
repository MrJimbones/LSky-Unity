////////////////////////////////////////////////////////
/// LSky
///-----------------------------------------------------
/// Planetary Positions
///-----------------------------------------------------
/// Description: Planetary Positions Calculations.
/// All calculations are based on Paul Schlyter papers.
/// See: http://www.stjarnhimlen.se/comp/ppcomp.html
/// See: http://stjarnhimlen.se/comp/tutorial.html
////////////////////////////////////////////////////////

using System;
using UnityEngine;
using Rallec.LSky.Utility;

namespace Rallec.LSky
{

    /// <summary></summary>
    public partial class LSky_PlanetaryPositions 
    {
        
        // Date time.
        //---------------------------------------------------
        /// <summary></summary>
        public LSky_DateTime dateTime{ get; set; }

        // Location.
        //---------------------------------------------------
        [Range(-90f, 90f)] public float latitude;

        /// <summary> Latitude in radiants. </summary>
        public float Latitude_Rad
        {
            get{ return Mathf.Deg2Rad * latitude; } 
        }
      
        [Range(-180f, 180f)] public float longitude;

        [Range(-12f, 12f)] public float uTC;

        /// <summary> Total hours - UTC. </summary>
        public float TotalHours_UTC
        {
            get{ return dateTime.totalHours - uTC; }
        }

        // Coordinates.
        //---------------------------------------------------
        /// <summary> Sun distance(r). </summary>
        public float SunDistance{ get; private set; }

        /// <summary> True sun longitude. </summary>
        public float TrueSunLongitude{ get; private set; }

        /// <summary> Mean sun longitude. </summary>
        public float MeanSunLongitude{ get; private set; }

        /// <summary> Sideral Time. </summary>
        public float SideralTime{ get; private set; }

        /// <summary> Local sideral time. </summary>
        public float LocalSideralTime{ get; private set; }

        /// <summary> Time Scale (d). </summary>
        public float TimeScale
        {
            get
            {
                return (367 * dateTime.year - (7 * (dateTime.year + ((dateTime.month + 9) / 12))) / 4 +
                    (275 * dateTime.month) / 9 + dateTime.day - 730530) + (float)dateTime.totalHours / 24;
            }
        }

        /// <summary> Obliquity of the ecliptic. </summary>
        public float Oblecl
        { 
            get{ return Mathf.Deg2Rad * (23.4393f - 3.563e-7f * TimeScale); }
        }

        // Compute Coordinates.
        //-------------------------------------------------------------------------------------------------
        public LSky_CelestialsCoords m_SunCoords = LSky_CelestialsCoords.Zero, m_MoonCoords = LSky_CelestialsCoords.Zero;
        /// <summary></summary>
        public LSky_CelestialsCoords SunCoords{ get{ return m_SunCoords; } }

        /// <summary></summary>
        public LSky_CelestialsCoords MoonCoords{ get{ return m_MoonCoords; } }

        // Sun orbital elements.
        private LSky_OrbitalElements m_SunOE;
 
        /// <summary>
        /// Sun Orbital Elements.
        /// Mean Anomaly is Normalized.
        /// </summary>
        public LSky_OrbitalElements SunOE
        {
            get
            {
                // Get sun orbital elelemts.
                m_SunOE.GetEPOrbitalElements(0, TimeScale);

                // Normalize Mean Anomaly.
                m_SunOE.M = LSky_Mathf.NormalizeDegrees(m_SunOE.M);

                return m_SunOE;
            }
        }

        // Moon orbitel elements.
        private LSky_OrbitalElements m_MoonOE;

        /// <summary>
        /// Moon Orbital Elements.
        /// Mean Anomaly is Normalized.
        /// </summary>
        public LSky_OrbitalElements MoonOE
        {
            get
            {
                // Get moon orbital elelemts.
                m_MoonOE.GetEPOrbitalElements(1, TimeScale);

                // Normalize longitude of acending node.
                m_MoonOE.N = LSky_Mathf.NormalizeDegrees(m_MoonOE.N);

                // Normalize argument of perihelion.
                m_MoonOE.w = LSky_Mathf.NormalizeDegrees(m_MoonOE.w);

                // Normalize mean anomaly.
                m_MoonOE.M = LSky_Mathf.NormalizeDegrees(m_MoonOE.M);

                return m_MoonOE;
            }
        }

        /// <summary>
        /// Compute Sun Coordinates.
        /// Get sun azimuth and altitude.
        /// </summary>
        public void ComputeSunCoords()
        {
            #region |Orbital Elements|

            // Mean Anomaly in radians.
            float M_Rad = SunOE.M * Mathf.Deg2Rad;
        
            #endregion

            #region |Eccentric Anomaly|

            // Compute eccentric anomaly.
            float E = SunOE.M + Mathf.Rad2Deg * SunOE.e * Mathf.Sin(M_Rad) * (1 + SunOE.e * Mathf.Cos(M_Rad));

            // Eccentric anomaly to radians.
            float E_Rad = Mathf.Deg2Rad * E;// Debug.Log(E);

            #endregion

            #region |Rectangular Coordinates|

            // Rectangular coordinates of the sun in the plane of the ecliptic.
            float xv = (Mathf.Cos(E_Rad) - SunOE.e);                                 // Debug.Log(xv);
            float yv = (Mathf.Sin(E_Rad) * Mathf.Sqrt(1 - SunOE.e * SunOE.e)); // Debug.Log(yv);

            // Convert to distance and true anomaly(r = radians, v = degrees).
            float r = Mathf.Sqrt(xv * xv + yv * yv);       // Debug.Log(r);
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv); // Debug.Log(v);

            // Get sun distance.
            SunDistance = r;

            #endregion

            #region |True Longitude|

            // True sun longitude.
            float lonsun = v + SunOE.w;

            // Normalize sun longitude
            lonsun = LSky_Mathf.NormalizeDegrees(lonsun); // Debug.Log(lonsun);

            // True sun longitude to radians.
            float lonsun_Rad = Mathf.Deg2Rad * lonsun;

            // Set true sun longitude(radians) for use in others celestials calculations.
            TrueSunLongitude = lonsun_Rad;

            #endregion

            #region |Ecliptic And Equatorial Coordinates|

            // Ecliptic rectangular coordinates(radians):
            float xs = r * Mathf.Cos(lonsun_Rad);
            float ys = r * Mathf.Sin(lonsun_Rad);

            // Ecliptic rectangular coordinates rotate these to equatorial coordinates(radians).
            float oblecl_Cos = Mathf.Cos(Oblecl);
            float oblecl_Sin = Mathf.Sin(Oblecl);

            float xe = xs;
            float ye = ys * oblecl_Cos - 0.0f * oblecl_Sin;
            float ze = ys * oblecl_Sin + 0.0f * oblecl_Cos;

            #endregion

            #region |Ascension And Declination|

            // Right ascension(degrees):
            float RA = Mathf.Rad2Deg * Mathf.Atan2(ye, xe) / 15;

            // Declination(radians).
            float Decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

            #endregion

            #region |Mean Longitude|

            // Mean sun longitude(degrees).
            float L = SunOE.w + SunOE.M;

            // Rev mean sun longitude.
            L = LSky_Mathf.NormalizeDegrees(L);

            // Set mean sun longitude for use in other celestials calculations.
            MeanSunLongitude = L;

            #endregion

            #region |Sideral Time|

            // Sideral time(degrees).
            float GMST0 = /*(L + 180) / 15;*/  ((L / 15) + 12);

            SideralTime = GMST0 + TotalHours_UTC + longitude / 15 + 15/15 ;
            LocalSideralTime = Mathf.Deg2Rad * SideralTime * 15;

             // Hour angle(degrees).
            float HA = (SideralTime - RA) * 15; // Debug.Log(HA);

            // Hour angle in radians.
            float HA_Rad = Mathf.Deg2Rad * HA;  // Debug.Log(HA);

            #endregion

            #region |Hour Angle And Declination In Rectangular Coordinates|

            // HA anf Decl in rectangular coordinates(radians).
            float Decl_Cos = Mathf.Cos(Decl);

            // X axis points to the celestial equator in the south.
            float x = Mathf.Cos(HA_Rad) * Decl_Cos;// Debug.Log(x);

            // Y axis points to the horizon in the west.
            float y = Mathf.Sin(HA_Rad) * Decl_Cos; // Debug.Log(y);

            // Z axis points to the north celestial pole.
            float z = Mathf.Sin(Decl);// Debug.Log(z);

            // Rotate the rectangualar coordinates system along of the Y axis(radians).
            float sinLatitude = Mathf.Sin(Latitude_Rad);
            float cosLatitude = Mathf.Cos(Latitude_Rad);

            float xhor = x * sinLatitude - z * cosLatitude; // Debug.Log(xhor);
            float yhor = y;
            float zhor = x * cosLatitude + z * sinLatitude; // Debug.Log(zhor);

            #endregion

            #region Azimuth, Altitude And Zenith[Radians].

            m_SunCoords.azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.PI; // Azimuth.
            m_SunCoords.altitude = LSky_Mathf.k_HalfPI - Mathf.Atan2 (zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor)); // Altitude.

            #endregion
        }

        /// <summary>
        /// Compute Moon Coordinates without perturbations.
        /// Get moon azimuth and altitude.
        /// </summary>
        public void ComputeMoonCoords()
        {

            #region |Orbital Elements|

            // Orbital elements in radians.
            float N_Rad = Mathf.Deg2Rad * MoonOE.N;
            float i_Rad = Mathf.Deg2Rad * MoonOE.i;
            float M_Rad = Mathf.Deg2Rad * MoonOE.M;

            #endregion

            #region |Eccentric Anomaly|

            // Compute eccentric anomaly.
            float E = MoonOE.M + Mathf.Rad2Deg * MoonOE.e * Mathf.Sin(M_Rad) * (1 + SunOE.e * Mathf.Cos(M_Rad));

            // Eccentric anomaly to radians.
            float E_Rad = Mathf.Deg2Rad * E;

            #endregion

            #region |Rectangular Coordinates|

            // Rectangular coordinates of the sun in the plane of the ecliptic.
            float xv = MoonOE.a * (Mathf.Cos(E_Rad) - MoonOE.e); // Debug.Log(xv);
            float yv = MoonOE.a * (Mathf.Sin(E_Rad) * Mathf.Sqrt(1 - MoonOE.e * MoonOE.e)) * Mathf.Sin(E_Rad); // Debug.Log(yv);

            // Convert to distance and true anomaly(r = radians, v = degrees).
            float r = Mathf.Sqrt(xv * xv + yv * yv);         // Debug.Log(r);
            float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);   // Debug.Log(v);

            v = LSky_Mathf.NormalizeDegrees(v);

            // Longitude in radians.
            float l = Mathf.Deg2Rad * (v + MoonOE.w);

            float Cos_l = Mathf.Cos(l);
            float Sin_l = Mathf.Sin(l);
            float Cos_N_Rad = Mathf.Cos(N_Rad);
            float Sin_N_Rad = Mathf.Sin(N_Rad);
            float Cos_i_Rad = Mathf.Cos(i_Rad);

            float xeclip = r * (Cos_N_Rad * Cos_l - Sin_N_Rad * Sin_l * Cos_i_Rad);
            float yeclip = r * (Sin_N_Rad * Cos_l + Cos_N_Rad * Sin_l * Cos_i_Rad);
            float zeclip = r * (Sin_l * Mathf.Sin(i_Rad));

            #endregion

            #region Geocentric Coordinates.

            // Geocentric position for the moon and Heliocentric position for the planets.
            float lonecl = Mathf.Rad2Deg * Mathf.Atan2(yeclip, xeclip);

            // Rev lonecl
            lonecl = LSky_Mathf.NormalizeDegrees(lonecl);     // Debug.Log(lonecl);

            float latecl = Mathf.Rad2Deg * Mathf.Atan2(zeclip, Mathf.Sqrt(xeclip * xeclip + yeclip * yeclip));   // Debug.Log(latecl);

            // Get true sun longitude.
            // float lonSun = TrueSunLongitude;

            // Ecliptic longitude and latitude in radians.
            float lonecl_Rad = Mathf.Deg2Rad * lonecl;
            float latecl_Rad = Mathf.Deg2Rad * latecl;

            float nr = 1.0f;
            float xh = nr * Mathf.Cos(lonecl_Rad) * Mathf.Cos(latecl_Rad);
            float yh = nr * Mathf.Sin(lonecl_Rad) * Mathf.Cos(latecl_Rad);
            float zh = nr * Mathf.Sin(latecl_Rad);

            // Geocentric posisition.
            float xs = 0.0f;
            float ys = 0.0f;

            // Convert the geocentric position to heliocentric position.
            float xg = xh + xs;
            float yg = yh + ys;
            float zg = zh;

            #endregion

            #region |Equatorial Coordinates|

            // Convert xg, yg in equatorial coordinates.
            float oblecl_Cos = Mathf.Cos(Oblecl);
            float oblecl_Sin = Mathf.Sin(Oblecl);

            float xe = xg;
            float ye = yg * oblecl_Cos - zg * oblecl_Sin;
            float ze = yg * oblecl_Sin + zg * oblecl_Cos;

            #endregion

            #region |Ascension, Declination And Hour Angle|

            // Right ascension.
            float RA = Mathf.Rad2Deg * Mathf.Atan2(ye, xe); //Debug.Log(RA);

            // Normalize right ascension.
            RA = LSky_Mathf.NormalizeDegrees(RA);  //Debug.Log(RA);

            // Declination.
            float Decl = Mathf.Rad2Deg * Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

            // Declination in radians.
            float Decl_Rad = Mathf.Deg2Rad * Decl;

            // Hour angle.
            float HA = ((SideralTime * 15) - RA); //Debug.Log(HA);

            // Rev hour angle.
            HA = LSky_Mathf.NormalizeDegrees(HA);     //Debug.Log(HA);

            // Hour angle in radians.
            float HA_Rad = Mathf.Deg2Rad * HA;

            #endregion

            #region |Declination in rectangular coordinates|

            // HA y Decl in rectangular coordinates.
            float Decl_Cos = Mathf.Cos(Decl_Rad);
            float xr = Mathf.Cos(HA_Rad) * Decl_Cos;
            float yr = Mathf.Sin(HA_Rad) * Decl_Cos;
            float zr = Mathf.Sin(Decl_Rad);

            // Rotate the rectangualar coordinates system along of the Y axis(radians).
            float sinLatitude = Mathf.Sin(Latitude_Rad);
            float cosLatitude = Mathf.Cos(Latitude_Rad);

            float xhor = xr * sinLatitude - zr * cosLatitude;
            float yhor = yr;
            float zhor = xr * cosLatitude + zr * sinLatitude;

            #endregion

            #region |Azimuth, Altitude And Zenith[Radians]|

            m_MoonCoords.azimuth  = Mathf.Atan2(yhor, xhor) + Mathf.PI;
            m_MoonCoords.altitude = LSky_Mathf.k_HalfPI - Mathf.Atan2 (zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor)); // Altitude.

            #endregion
        }
    
    }
    
}
