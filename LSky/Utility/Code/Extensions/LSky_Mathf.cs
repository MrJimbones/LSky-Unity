/////////////////////////////////////////////////
/// LSky.
///----------------------------------------------
/// Utility.
///----------------------------------------------
/// Mathf.
///----------------------------------------------
/// Description: Extensions for math.
/////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rallec.LSky.Utility
{

    public partial struct LSky_Mathf
    {

        #region [PI]

        /// <summary> PI/2 </summary>
        public const float k_HalfPI = 1.570796f;

        /// <summary> 1 / (PI/2) </summary>
        public const float k_InvHalfPI = 0.636619f;

        /// <summary> PI*2 </summary>
        public const float k_Tau = 6.283185f;

        /// <summary> 1/(PI*2) </summary>
        public const float k_InvTau = 0.159154f;

        /// <summary> PI*4 </summary>
        public const float k_PI4 = 12.566370f;

        /// <summary> 1/(PI*4) </summary>
        public const float k_InvPI4 = 0.079577f;

        /// <summary> 3/(PI*8) </summary>
        public const float k_3PIE = 0.119366f;

        /// <summary> 3/(PI*16) </summary>
        public const float k_3PI16 = 0.059683f;

        #endregion

        #region [Generic]

        /// <summary></summary>
        public static float Saturate(float x)
        {
            return Mathf.Max(0.0f, Mathf.Min(1.0f, x));
        }

        /// <summary></summary>
        public static Vector3 Saturate(Vector3 vec)
        {
            Vector3 r;
            r.x = Saturate(vec.x);
            r.y = Saturate(vec.y);
            r.z = Saturate(vec.z);
            return r;
        }

        /// <summary> Prevents excesses defrees. </summary>
        /// <param name="x"> Degrees </param>
        public static float NormalizeDegrees(float x)
        {
            return x - Mathf.Floor(x/360f) * 360f;
        }

        #endregion

        #region [Coordinate System]

        /// <summary> Convert spherical coordinates to cartesian coordinates. </summary>
        /// <param name="theta"> Theta. </param>
        /// <param name="pi"> PI. </param>
        /// <returns> XYZ coordinates. </returs>
        /// See: https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
        public static Vector3 SphericalToCartesian(float theta, float pi)
        {
            Vector3 r;

            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            float sinPI    = Mathf.Sin(pi);
            float cosPI    = Mathf.Cos(pi);

            r.x = sinTheta * sinPI;
            r.y = cosTheta;
            r.z = sinTheta * cosPI;

            return r;
        }

        /// <summary> Convert spherical coordinates to cartesian coordinates. </summary>
        /// <param name="theta"> Theta. </param>
        /// <param name="pi"> PI. </param>
        /// <param name="rad"> Radius. </param>
        /// <returns> XYZ coordinates. </returs>
        /// See: https://en.wikipedia.org/wiki/Spherical_coordinate_system#Cartesian_coordinates
        public static Vector3 SphericalToCartesian(float theta, float pi, float rad)
        {
            Vector3 r; rad = Mathf.Max(0.5f, rad);

            float sinTheta = Mathf.Sin(theta);
            float cosTheta = Mathf.Cos(theta);
            float sinPI    = Mathf.Sin(pi);
            float cosPI    = Mathf.Cos(pi);

            r.x = rad * sinTheta * sinPI;
            r.y = rad * cosTheta;
            r.z = rad * sinTheta * cosPI;

            return r;
        }
        #endregion
    }

}
