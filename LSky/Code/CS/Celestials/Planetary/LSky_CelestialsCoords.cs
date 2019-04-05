/////////////////////////////////////////////////
/// LSky
///----------------------------------------------
/// Celestials
///----------------------------------------------
/// Description: Celestials Coordinates Structure
/////////////////////////////////////////////////

using System;
using UnityEngine;

namespace Rallec.LSky
{

    /// <summary></summary>
    [Serializable] public struct LSky_CelestialsCoords
    {
        public float azimuth;  // set pi
        public float altitude; // set theta

        static readonly LSky_CelestialsCoords m_Zero = new LSky_CelestialsCoords(0.0f, 0.0f);
        
        public static LSky_CelestialsCoords Zero{ get{ return m_Zero; } }

        public LSky_CelestialsCoords(float _azimuth, float _altitude)
        {
            this.azimuth  = _azimuth;
            this.altitude = _altitude;
        }
    }
}
