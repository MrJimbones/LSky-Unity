////////////////////////////////////////////////////////
/// LSky
///-----------------------------------------------------
/// Celestial Orbital Elements
///-----------------------------------------------------
/// Description: Orbital Elements for celestials
/// calculations.
/// All calculations are based on Paul Schlyter papers.
/// See: http://www.stjarnhimlen.se/comp/ppcomp.html
/// See: http://stjarnhimlen.se/comp/tutorial.html
////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Rallec.LSky
{

    /// <summary></summary>
    public partial struct LSky_OrbitalElements
    {

        /// <summary>
        /// Longitude of the ascending node.
        /// </summary>
        public float N;

        /// <summary>
        /// The Inclination to the ecliptic.
        /// </summary>
        public float i;

        /// <summary>
        /// Argument of perihelion.
        /// </summary>
        public float w;

        /// <summary>
        /// Semi-major axis, or mean distance from sun.
        /// </summary>
        public float a;

        /// <summary>
        /// Eccentricity.
        /// </summary>
        public float e;

        /// <summary>
        /// Mean anomaly.
        /// </summary>
        public float M;

        public LSky_OrbitalElements(float _N, float _i, float _w, float _a, float _e, float _M)
        {
            this.N = _N;
            this.i = _i;
            this.w = _w;
            this.a = _a;
            this.e = _e;
            this.M = _M;
        }

        /// <summary>
        /// Get Earth perspective orbital elements.
        /// </summary>
        /// <param name="index"> 
        /// Celestial Index:
        /// Sun = 0, Moon = 1.
        /// </param>
        /// <param name="timeScale"> Time Scale(d) </param>
        public void GetEPOrbitalElements(int index, float timeScale)
        {
            switch(index)
            {
                // Sun.
                //---------------------------------------------------
                case 0: 

                this.N = 0.0f;
                this.i = 0.0f;
                this.w = 282.9404f + 4.70935e-5f   * timeScale;
                this.a = 0.0f;
                this.e = 0.016709f - 1.151e-9f     * timeScale;
                this.M = 356.0470f + 0.9856002585f * timeScale;

                break;

                // Moon.
                //---------------------------------------------------
                case 1:

                this.N = 125.1228f - 0.0529538083f * timeScale;
                this.i = 5.1454f;
                this.w = 318.0634f + 0.1643573223f * timeScale;
                this.a = 60.2666f; 
                this.e = 0.054900f;  
                this.M = 115.3654f + 13.0649929509f * timeScale;

                break;
            }

        }

    }
}